using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace VirtualPhysicalLab.Core.Reports
{
    // This class currently exports printable HTML, not a binary PDF file.
    public class PDFReportGenerator : MonoBehaviour
    {
        public string GeneratePDFReport(LabReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            string htmlTemplate = $@"<!doctype html>
<html lang=""uz"">
<head>
<meta charset=""utf-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
<title>Virtual fizika laboratoriyasi hisoboti</title>
<style>
body {{ font-family: Arial, sans-serif; margin: 32px auto; max-width: 900px; padding: 0 20px; color: #222; }}
h1 {{ color: #0C2340; border-bottom: 2px solid #0C2340; padding-bottom: 10px; }}
.results {{ margin-top: 24px; background: #f4f7fa; padding: 16px; }}
@media print {{ body {{ margin: 0; }} }}
</style>
</head>
<body>
<h1>VIRTUAL FIZIKA LABORATORIYASI — HISOBOT</h1>
<section>
<p><b>Talaba:</b> {Escape(report.studentName)}</p>
<p><b>Guruh:</b> {Escape(report.groupName)}</p>
<p><b>Laboratoriya ishi:</b> №{report.labId}</p>
<p><b>Bajarilgan vaqt:</b> {Escape(report.timestamp)}</p>
</section>
<section class=""results"">
<p>Yakuniy qiymat: {report.finalResult:F4}</p>
<p>Mutlaq xatolik: {report.absoluteError:F4}</p>
<p>Nisbiy xatolik: {report.relativeError:F2}%</p>
<p>Xulosa: {(report.isPassed ? "QABUL QILINDI" : "QAYTA TOPSHIRILSIN")}</p>
</section>
<script>window.onload = function () {{ window.print(); }};</script>
</body>
</html>";

            string path = Path.Combine(Application.persistentDataPath, $"Hisobot_Lab{report.labId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.html");
            File.WriteAllText(path, htmlTemplate, System.Text.Encoding.UTF8);
            Debug.Log($"Chop etish uchun HTML hisobot saqlandi: {path}");
            return path;
        }

        private static string Escape(string value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}