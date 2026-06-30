using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileUploadApp.Models;
using FileUploadApp.Services;

namespace FileUploadApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly FileUploadService _fileUploadService;
    private readonly INotificationService _notificationService;
    private CancellationTokenSource? _cts;

    [ObservableProperty] private string? selectedFilePath;
    [ObservableProperty] private string fileNameDisplay = "No file selected";
    [ObservableProperty] private string fileTypeDisplay = string.Empty;
    [ObservableProperty] private string fileSizeDisplay = string.Empty;
    [ObservableProperty] private int uploadProgress;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isUploading;
    [ObservableProperty] private bool hasSelectedFile;

    public ObservableCollection<UploadedFile> RecentUploads { get; } = new();

    public MainWindowViewModel()
        : this(new FileUploadService(), NotificationServiceFactory.Create()) { }

    public MainWindowViewModel(FileUploadService fileUploadService, INotificationService notificationService)
    {
        _fileUploadService = fileUploadService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task BrowseAsync(Window window)
    {
        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a file to upload",
            AllowMultiple = false
        });
        if (files.Count == 0) return;
        SetSelectedFile(files[0].Path.LocalPath);
    }

    public void HandleDrop(DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles();
            if (files is null) return;
            foreach (var file in files)
            {
                SetSelectedFile(file.Path.LocalPath);
                break; // only first file
            }
        }
    }

    private void SetSelectedFile(string path)
    {
        var info = new FileInfo(path);
        SelectedFilePath = path;
        FileNameDisplay = info.Name;
        FileTypeDisplay = info.Extension.TrimStart('.').ToUpperInvariant() + " Document";
        FileSizeDisplay = FormatSize(info.Length);
        StatusMessage = string.Empty;
        UploadProgress = 0;
        HasSelectedFile = true;
    }

    [RelayCommand]
    private void ClearFile()
    {
        SelectedFilePath = null;
        FileNameDisplay = "No file selected";
        FileTypeDisplay = string.Empty;
        FileSizeDisplay = string.Empty;
        StatusMessage = string.Empty;
        UploadProgress = 0;
        HasSelectedFile = false;
    }

    [RelayCommand(CanExecute = nameof(CanUpload))]
    private async Task UploadAsync()
    {
        if (SelectedFilePath is null) return;

        _cts = new CancellationTokenSource();
        IsUploading = true;
        StatusMessage = "Uploading...";
        UploadProgress = 0;

        var progress = new Progress<int>(value => UploadProgress = value);

        try
        {
            await _fileUploadService.UploadFileAsync(SelectedFilePath, progress, _cts.Token);
            StatusMessage = "Completed";
            _notificationService.SendUploadCompleteNotification(FileNameDisplay);
            RecentUploads.Insert(0, new UploadedFile
            {
                FileName = FileNameDisplay,
                SizeInBytes = new FileInfo(SelectedFilePath).Length,
                Timestamp = DateTime.Now,
                Status = "Completed"
            });
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Cancelled";
            UploadProgress = 0;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            _notificationService.SendUploadErrorNotification(FileNameDisplay, ex.Message);
            RecentUploads.Insert(0, new UploadedFile
            {
                FileName = FileNameDisplay,
                SizeInBytes = SelectedFilePath is not null ? new FileInfo(SelectedFilePath).Length : 0,
                Timestamp = DateTime.Now,
                Status = "Error"
            });
        }
        finally
        {
            IsUploading = false;
        }
    }

    [RelayCommand]
    private void Cancel() => _cts?.Cancel();

    private bool CanUpload() => SelectedFilePath is not null && !IsUploading;

    partial void OnSelectedFilePathChanged(string? value) => UploadCommand.NotifyCanExecuteChanged();
    partial void OnIsUploadingChanged(bool value) => UploadCommand.NotifyCanExecuteChanged();

    private static string FormatSize(long bytes)
    {
        if (bytes >= 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F2} MB";
        if (bytes >= 1024) return $"{bytes / 1024.0:F2} KB";
        return $"{bytes} B";
    }
}