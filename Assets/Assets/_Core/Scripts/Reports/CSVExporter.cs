using System.IO;
using System.Text;
using UnityEngine;

namespace VirtualPhysicalLab.Core.Reports
{
    public static class CSVExporter
    {
        public static string ExportToCSV(LabReport report, string[] headers, string[][] rowData)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Talaba;" + report.studentName);
            sb.AppendLine("Guruh;" + report.groupName);
            sb.AppendLine("Sana;" + report.timestamp);
            sb.AppendLine($"Laboratoriya; {report.labId}-ish");
            sb.AppendLine($"O'rtacha Qiymat; {report.finalResult:F4}");
            sb.AppendLine($"Absolyut Xatolik; {report.absoluteError:F4}");
            sb.AppendLine($"Nisbiy Xatolik; {report.relativeError:F2}%");
            sb.AppendLine();

            sb.AppendLine(string.Join(";", headers));
            foreach (var row in rowData)
            {
                sb.AppendLine(string.Join(";", row));
            }

            string fileName = $"Hisobot_{report.studentName.Replace(" ", "_")}_Lab{report.labId}.csv";
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(fullPath, sb.ToString(), Encoding.UTF8);
            return fullPath;
        }
    }
}