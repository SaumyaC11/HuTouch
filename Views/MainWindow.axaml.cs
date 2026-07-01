using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using FileUploadApp.ViewModels;
using System.Linq;

namespace FileUploadApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DragDrop.AddDragOverHandler(DropZone, DropZone_DragOver);
        DragDrop.AddDropHandler(DropZone, DropZone_Drop);
    }

    private void DropZone_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer.Contains(DataFormat.File))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private void DropZone_Drop(object? sender, DragEventArgs e)
    {
        var files = e.DataTransfer.TryGetFiles();

        var file = files?.FirstOrDefault();
        if (file == null)
            return;

        var filePath = file.TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        if (DataContext is MainWindowViewModel vm)
        {
            vm.SetSelectedFile(filePath);
        }

        e.Handled = true;
    }
}