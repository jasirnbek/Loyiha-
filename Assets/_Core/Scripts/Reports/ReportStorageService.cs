using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace VirtualPhysicalLab.Core.Reports
{
    public class ReportStorageService
    {
        private readonly string savePath;

        public ReportStorageService()
        {
            savePath = Path.Combine(Application.persistentDataPath, "LabReports");
            Directory.CreateDirectory(savePath);
        }

        public string SaveReport(LabReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (string.IsNullOrWhiteSpace(report.id)) report.id = Guid.NewGuid().ToString("N");
            string filePath = Path.Combine(savePath, $"Lab_{report.labId}_{MakeSafeFileName(report.id)}.json");
            string tempPath = filePath + ".tmp";
            File.WriteAllText(tempPath, JsonUtility.ToJson(report, true));
            if (File.Exists(filePath)) File.Delete(filePath);
            File.Move(tempPath, filePath);
            return filePath;
        }

        public List<LabReport> LoadAllReports()
        {
            var reports = new List<LabReport>();
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            foreach (string file in Directory.GetFiles(savePath, "*.json"))
            {
                try
                {
                    LabReport report = JsonUtility.FromJson<LabReport>(File.ReadAllText(file));
                    if (report != null) reports.Add(report);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Hisobotni o'qib bo'lmadi ({Path.GetFileName(file)}): {ex.Message}");
                }
            }
            return reports;
        }

        private static string MakeSafeFileName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
            return string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value;
        }
    }
}