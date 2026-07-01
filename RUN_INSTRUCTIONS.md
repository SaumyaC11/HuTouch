# Run Instructions

Quick setup and run guide for the **File Upload App**.

---

## 1. Check .NET 8

Open a terminal and run:

```bash
dotnet --list-sdks
```

Make sure you see a version starting with:

```text
8.0
```

If not, install the **.NET 8 SDK** first.

---

## 2. Open the Project Folder

Open the folder that contains:

```text
FileUploadApp.csproj
```
---

## 3. Restore Dependencies

```bash
dotnet restore
```

---

## 4. Build the Project

```bash
dotnet build
```

---

## 5. Run the App

Run the command based on your operating system.

### Windows

```bash
dotnet run --framework net8.0-windows10.0.19041.0
```

### macOS

```bash
dotnet run --framework net8.0
```

---


## 3. Common Issues

### `dotnet` command not found

Install the .NET 8 SDK and restart the terminal.

### Windows build fails

Run using the Windows target framework:

```bash
dotnet run --framework net8.0-windows10.0.19041.0
```

### macOS build fails

Run using the cross-platform target framework:

```bash
dotnet run --framework net8.0
```

