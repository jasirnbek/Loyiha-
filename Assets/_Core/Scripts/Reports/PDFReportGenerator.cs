using System;
using System.IO;
using UnityEngine;

namespace VirtualPhysicalLab.Core.Reports
{
    public class PDFReportGenerator : MonoBehaviour
    {
        public void GeneratePDFReport(LabReport report)
        {
            string htmlTemplate = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 40px; color: #333; }}
                    h1 {{ color: #0C2340; border-bottom: 2px solid #0C2340; padding-bottom: 10px; }}
                    .meta-info {{ margin-bottom: 30px; font-size: 14px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #ddd; padding: 12px; text-align: center; }}
                    th {{ background-color: #0C2340; color: white; }}
                    .results {{ margin-top: 30px; font-weight: bold; background: #f9f9f9; padding: 15px; border-radius: 5px; }}
                </style>
            </head>
            <body>
                <h1>VIRTUAL FIZIKA LABORATORIYASI - HISOBOTI</h1>
                <div class='meta-info'>
                    <p><b>Talaba:</b> {report.studentName}</p>
                    <p><b>Guruh:</b> {report.groupName}</p>
                    <p><b>Laboratoriya ishi:</b> №{report.labId}</p>
                    <p><b>Bajarilgan vaqt:</b> {report.timestamp}</p>
                </div>
                
                <div class='results'>
                    <p>Hisoblangan Yakuniy Qiymat: {report.finalResult:F4}</p>
                    <p>O'rtacha Absolyut Xatolik: {report.absoluteError:F4}</p>
                    <p>Nisbiy Xatolik (E): {report.relativeError:F2}%</p>
                    <p>Xulosa: {(report.isPassed ? ""TAYYOR / QABUL QILINDI"" : ""XATOLIK YUQORI / QAYTA TOPSHIRILSIN"")}</p>
                </div>
            </body>
            </html>";

            #if UNITY_WEBGL && !UNITY_EDITOR
            WebGL_PrintHTML(htmlTemplate);
            #else
            string path = Path.Combine(Application.persistentDataPath, $"Hisobot_Lab{report.labId}.html");
            File.WriteAllText(path, htmlTemplate);
            Debug.Log($"PDF HTML yaratildi: {path}");
            #endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void WebGL_PrintHTML(string htmlContent);
        #endif
    }
}