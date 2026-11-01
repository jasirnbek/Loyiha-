using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace VirtualPhysicalLab.Core.Reports
{
    public static class CSVExporter
    {
        public static string ExportToCSV(LabReport report, string[] headers, string[][] rowData)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var sb = new StringBuilder();
            sb.AppendLine("Maydon;Qiymat");
            AppendRow(sb, "Talaba", report.studentName);
            AppendRow(sb, "Guruh", report.groupName);
            AppendRow(sb, "Sana", report.timestamp);
            AppendRow(sb, "Laboratoriya", $"{report.labId}-ish");
            AppendRow(sb, "O'rtacha qiymat", report.finalResult.ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
            AppendRow(sb, "Mutlaq xatolik", report.absoluteError.ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
            AppendRow(sb, "Nisbiy xatolik", report.relativeError.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "%");
            sb.AppendLine();

            sb.AppendLine(string.Join(";", Array.ConvertAll(headers, EscapeCell)));
            if (rowData != null)
            {
                foreach (string[] row in rowData)
                    sb.AppendLine(row == null ? string.Empty : string.Join(";", Array.ConvertAll(row, EscapeCell)));
            }

            string safeStudent = MakeSafeFileName(report.studentName);
            string fileName = $"Hisobot_{safeStudent}_Lab{report.labId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(fullPath, "\uFEFF" + sb.ToString(), new UTF8Encoding(false));
            return fullPath;
        }

        private static void AppendRow(StringBuilder sb, string key, string value)
        {
            sb.Append(EscapeCell(key)).Append(';').AppendLine(EscapeCell(value));
        }

        private static string EscapeCell(string value)
        {
            value = value ?? string.Empty;
            // Prevent spreadsheet formula injection in text cells.
            string trimmed = value.TrimStart();
            if (trimmed.StartsWith("=") || trimmed.StartsWith("+") || trimmed.StartsWith("-") || trimmed.StartsWith("@"))
                value = "'" + value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static string MakeSafeFileName(string value)
        {
            string name = string.IsNullOrWhiteSpace(value) ? "Talaba" : value.Trim();
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            name = name.Replace('/', '_').Replace('\\', '_').Replace(' ', '_');
            return string.IsNullOrEmpty(name) ? "Talaba" : name;
        }
    }
}