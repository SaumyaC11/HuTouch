# File Upload App

A cross-platform desktop file upload application built with **Avalonia UI** and **.NET 8**.  
The app lets users select a local file, validates the file size, copies it into the application's local storage folder, shows upload progress, and triggers native OS notifications after a successful upload.

> In this assignment, "upload" means copying the selected file into the app's local uploads directory. No backend or API upload is required.

---

## Features

- Browse and select a file from the desktop or Drag & Drop available too
- Display selected file name, type, and size
- Validate maximum file size of **500 MB**
- Copy the file into local application storage
- Show upload progress from **0% to 100%**
- Keep the UI responsive during upload
- Show upload status:
  - Idle
  - Uploading
  - Completed
  - Error
- Trigger native OS notification after upload completes
- Use an abstraction layer for notification logic
- Keep OS-specific notification code out of ViewModels
- Support pluggable notification services for Windows and macOS
- Recent uploads list with:
  - File name
  - File size
  - Upload timestamp
  - Upload status
- Cross-platform-safe local storage path

---

## Tech Stack

- **.NET 8**
- **Avalonia UI**
- **C#**
- **MVVM pattern**
- **CommunityToolkit.Mvvm**
- Native OS notification abstraction

---

## Project Structure

```text
FileUploadApp/
├── Assets/
│   ├── Icons/
│   │   ├── excel.svg
│   │   ├── file.svg
│   │   ├── image.svg
│   │   ├── pdf.svg
│   │   ├── powerpoint.svg
│   │   ├── word.svg
│   │   └── zip.svg
│   └── avalonia-logo.ico
├── Models/
│   └── UploadedFiles.cs
├── Services/
│   ├── FileUploadService.cs
│   ├── INotificationServices.cs
│   ├── MacNotificationService.cs
│   ├── NotificationServiceFactory.cs
│   ├── NullNotificationService.cs
│   └── WindowsNotificationService.cs
├── ViewModels/
│   ├── MainWindowViewModel.cs
│   └── ViewModelBase.cs
├── Views/
│   ├── MainWindow.axaml
│   └── MainWindow.axaml.cs
├── .gitignore
├── App.axaml
├── App.axaml.cs
├── app.manifest
├── FileUploadApp.csproj
├── FileUploadApp.sln
├── Program.cs
└── ViewLocator.cs
```

---

## Architecture

The application follows the **MVVM** pattern.

```text
MainWindow.axaml
      |
      v
MainWindowViewModel
      |
      |-- FileUploadService
      |
      |-- INotificationServices
              |
              |-- WindowsNotificationService
              |-- MacNotificationService
              |-- NullNotificationService
```

### Responsibilities

| Layer | Responsibility |
|---|---|
| View | Displays UI and binds to ViewModel properties/commands |
| ViewModel | Handles UI state, commands, validation flow, progress state, and upload status |
| FileUploadService | Copies selected files into the local uploads directory |
| INotificationServices | Defines a common notification contract |
| WindowsNotificationService | Handles Windows toast notifications |
| MacNotificationService | Handles macOS Notification Center notifications |
| NotificationServiceFactory | Selects the correct notification service at runtime |
| Models | Store recent uploaded file metadata |

No OS-specific notification logic is placed inside the ViewModel.

---

## Upload Destination

Uploaded files are copied to:

```text
Environment.SpecialFolder.LocalApplicationData/FileUploadApp/uploads/
```

Typical resolved locations:

### Windows

```text
C:\Users\<username>\AppData\Local\FileUploadApp\uploads
```

### macOS

```text
/Users/<username>/Library/Application Support/FileUploadApp/uploads
```
The app creates the upload directory automatically when needed.

---

## File Validation

The maximum supported file size is:

```text
500 MB
```

Validation happens before upload starts.

If the selected file is larger than 500 MB:

- The upload is blocked
- The file is not copied
- The progress bar remains at 0
- The status changes to an error state
- An error message is shown to the user

---

## How Upload Works

1. User clicks **Browse File**
2. User selects a file
3. App reads file metadata:
   - File name
   - File extension/type
   - File size
4. App validates the file size
5. User clicks **Upload**
6. File is copied into the local uploads directory
7. Progress bar updates from 0 to 100
8. Recent uploads list is updated
9. Native OS notification is triggered
10. Status changes to **Completed**

---

## Native Notifications

The app uses an abstraction layer:

```csharp
public interface INotificationServices
{
    Task ShowNotificationAsync(string title, string message);
}
```

Runtime selection is handled by a factory:

```csharp
INotificationServices notificationService = NotificationServiceFactory.Create();
```

This keeps the ViewModel platform-independent.

### Windows

Windows uses a toast notification implementation.

Expected behavior:

- Notification appears in Windows Action Center
- Triggered after upload completes

### macOS

macOS uses a Notification Center implementation.

Expected behavior:

- Notification appears in macOS Notification Center
- Triggered after upload completes

### Unsupported OS

For unsupported platforms, the app can fall back to a no-op notification service so the application still runs.

---

## Prerequisites

Install **.NET 8 SDK**.

Check your installed SDKs:

```bash
dotnet --list-sdks
```

You should see a version starting with:

```text
8.0
```

---

## Run Instructions

From the project root:

```bash
dotnet restore
dotnet build
```

Run the correct framework for your OS.

### Windows

```bash
dotnet run --framework net8.0-windows10.0.19041.0
```

### macOS

```bash
dotnet run --framework net8.0
```

---

## Build Instructions

### Windows Build

```bash
dotnet publish -c Release -f net8.0-windows10.0.19041.0 -r win-x64 --self-contained false
```

Output will be created under:

```text
bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/
```

### macOS Build - Apple Silicon

```bash
dotnet publish -c Release -f net8.0 -r osx-arm64 --self-contained false
```

### macOS Build - Intel

```bash
dotnet publish -c Release -f net8.0 -r osx-x64 --self-contained false
```

Output will be created under:

```text
bin/Release/net8.0/<runtime>/publish/
```

---


If the notification does not appear:

- Open **System Settings**
- Go to **Notifications**
- Find the app or terminal host
- Allow notifications
- Run the app again and upload another file

---



## Troubleshooting

### `dotnet` command not found

Install the .NET 8 SDK and restart your terminal.

### App does not start on Windows

Try running with the Windows target framework:

```bash
dotnet run --framework net8.0-windows10.0.19041.0
```

### App does not start on macOS

Try running with the cross-platform target framework:

```bash
dotnet run --framework net8.0
```

### Notification does not appear

Check OS notification permissions.

On macOS, notifications may be blocked for the terminal or app host until manually allowed.

On Windows, make sure Focus Assist / Do Not Disturb is disabled.

### Uploaded file is not visible

Check the local uploads directory:

```text
Environment.SpecialFolder.LocalApplicationData/FileUploadApp/uploads/
```

The exact folder depends on the operating system.

---


