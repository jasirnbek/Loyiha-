using UnityEngine;
using System;
using VirtualPhysicalLab.Core.Reports;

namespace VirtualPhysicalLab.Core
{
    public class LabManager : MonoBehaviour
    {
        public static LabManager Instance { get; private set; }

        [Header("Xizmatlar")]
        [SerializeField] private PDFReportGenerator pdfGenerator;
        private ReportStorageService storageService;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                storageService = new ReportStorageService();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void FinalizeLabReport(int labId, string student, string group, double result, double absError, double relError)
        {
            LabReport report = new LabReport
            {
                id = Guid.NewGuid().ToString(),
                labId = labId,
                studentName = student,
                groupName = group,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                finalResult = result,
                absoluteError = absError,
                relativeError = relError,
                isPassed = relError < 10.0
            };

            storageService.SaveReport(report);
            pdfGenerator.GeneratePDFReport(report);
            Debug.Log($"Laboratoriya {labId} muvaffaqiyatli topshirildi va hisobot yaratildi!");
        }
    }
}