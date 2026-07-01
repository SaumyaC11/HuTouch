using System;

namespace FileUploadApp.Models
{
    public class UploadedFile
    {
        public string FileName { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;

        public string DisplaySize
        {
            get
            {
                if (SizeInBytes >= 1024 * 1024)
                    return $"{SizeInBytes / (1024.0 * 1024.0):F2} MB";
                if (SizeInBytes >= 1024)
                    return $"{SizeInBytes / 1024.0:F2} KB";
                return $"{SizeInBytes} B";
            }
        }

        public string StatusBackground => Status == "Completed" ? "#DCFCE7" : "#FEE2E2";
        public string StatusForeground => Status == "Completed" ? "#16A34A" : "#DC2626";
        public string IconPath { get; set; } = "/Assets/Icons/file.svg";
    }


}