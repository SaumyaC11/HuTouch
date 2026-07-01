using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
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
    private CancellationTokenSource? _resetCts;
    private static readonly TimeSpan ResetDelay = TimeSpan.FromSeconds(7);

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
        : this(new FileUploadService(), NotificationServiceFactory.Create())
    {
    }

    public MainWindowViewModel(
        FileUploadService fileUploadService,
        INotificationService notificationService)
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

        if (files.Count == 0)
            return;

        var path = files[0].TryGetLocalPath();

        if (string.IsNullOrWhiteSpace(path))
            return;

        SetSelectedFile(path);
    }

    public void HandleDrop(DragEventArgs e)
    {
        var files = e.DataTransfer.TryGetFiles();

        if (files is null)
            return;

        foreach (var file in files)
        {
            var path = file.TryGetLocalPath();

            if (!string.IsNullOrWhiteSpace(path))
            {
                SetSelectedFile(path);
                break;
            }
        }

        e.Handled = true;
    }

    public void SetSelectedFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        CancelPendingReset();

        var info = new FileInfo(path);

        SelectedFilePath = path;
        FileNameDisplay = info.Name;
        FileTypeDisplay = GetFileTypeDisplay(info.Extension);
        FileSizeDisplay = FormatSize(info.Length);

        StatusMessage = string.Empty;
        UploadProgress = 0;
        HasSelectedFile = true;
    }

    [RelayCommand]
    private void ClearFile()
    {
        CancelPendingReset();
        ResetFileDisplay();
    }

    [RelayCommand(CanExecute = nameof(CanUpload))]
    private async Task UploadAsync()
    {
        if (SelectedFilePath is null) return;

        CancelPendingReset();

        _cts = new CancellationTokenSource();
        IsUploading = true;
        StatusMessage = "Uploading...";
        UploadProgress = 0;

        var progress = new Progress<int>(value => UploadProgress = value);

        try
        {
            await _fileUploadService.UploadFileAsync(SelectedFilePath, progress, _cts.Token);

            StatusMessage = "Completed";
            UploadProgress = 100;

            _notificationService.SendUploadCompleteNotification(FileNameDisplay);

            RecentUploads.Insert(0, new UploadedFile
            {
                FileName = FileNameDisplay,
                SizeInBytes = new FileInfo(SelectedFilePath).Length,
                Timestamp = DateTime.Now,
                Status = "Completed"
            });

            ScheduleReset();
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
            _cts?.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _cts?.Cancel();
    }

    private bool CanUpload()
    {
        return SelectedFilePath is not null && !IsUploading;
    }

    partial void OnSelectedFilePathChanged(string? value)
    {
        UploadCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsUploadingChanged(bool value)
    {
        UploadCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// After a successful upload, waits briefly so the user sees the "Completed" state,
    /// then clears the left-panel file card (progress bar, status, file details) so a
    /// finished upload from an earlier session doesn't linger indefinitely.
    /// </summary>
    private void ScheduleReset()
    {
        _resetCts = new CancellationTokenSource();
        var token = _resetCts.Token;
        var uploadedPath = SelectedFilePath;

        _ = ResetAfterDelayAsync(token, uploadedPath);
    }

    private async Task ResetAfterDelayAsync(CancellationToken token, string? uploadedPath)
    {
        try
        {
            await Task.Delay(ResetDelay, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        // Only reset if the user hasn't already picked a different file or cleared it themselves.
        if (token.IsCancellationRequested || SelectedFilePath != uploadedPath)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (SelectedFilePath == uploadedPath)
                ResetFileDisplay();
        });
    }

    private void CancelPendingReset()
    {
        _resetCts?.Cancel();
        _resetCts?.Dispose();
        _resetCts = null;
    }

    private void ResetFileDisplay()
    {
        SelectedFilePath = null;
        FileNameDisplay = "No file selected";
        FileTypeDisplay = string.Empty;
        FileSizeDisplay = string.Empty;
        StatusMessage = string.Empty;
        UploadProgress = 0;
        HasSelectedFile = false;
    }

    private static string FormatSize(long bytes)
    {
        if (bytes >= 1024 * 1024)
            return $"{bytes / (1024.0 * 1024.0):F2} MB";

        if (bytes >= 1024)
            return $"{bytes / 1024.0:F2} KB";

        return $"{bytes} B";
    }

    private static string GetFileTypeDisplay(string extension)
    {
        var ext = extension.TrimStart('.').ToUpperInvariant();

        return ext switch
        {
            "PDF" => "PDF Document",
            "DOC" => "Word Document",
            "DOCX" => "Word Document",
            "XLS" => "Excel Spreadsheet",
            "XLSX" => "Excel Spreadsheet",
            "PPT" => "PowerPoint Presentation",
            "PPTX" => "PowerPoint Presentation",
            "PNG" => "PNG Image",
            "JPG" => "JPG Image",
            "JPEG" => "JPEG Image",
            "ZIP" => "ZIP Archive",
            "MP4" => "MP4 Video",
            _ => string.IsNullOrWhiteSpace(ext) ? "Unknown File" : $"{ext} File"
        };
    }
}