using UnityEngine;
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
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }

        public void SaveReport(LabReport report)
        {
            string fileName = $"Lab_{report.labId}_{report.id}.json";
            string filePath = Path.Combine(savePath, fileName);
            string json = JsonUtility.ToJson(report, true);

            File.WriteAllText(filePath, json);
        }

        public List<LabReport> LoadAllReports()
        {
            List<LabReport> reports = new List<LabReport>();
            string[] files = Directory.GetFiles(savePath, "*.json");

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                LabReport report = JsonUtility.FromJson<LabReport>(json);
                reports.Add(report);
            }
            return reports;
        }
    }
}