using UnityEngine;
using System;
using VirtualPhysicalLab.Core.Reports;

namespace VirtualPhysicalLab.Core
{
    public class LabManager : MonoBehaviour
    {
        public static LabManager Instance { get; private set; }
        [SerializeField] private PDFReportGenerator pdfGenerator;
        private ReportStorageService storageService;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            storageService = new ReportStorageService();
        }

        public void FinalizeLabReport(int labId, string student, string group,
            double result, double absError, double relError)
        {
            if (storageService == null) storageService = new ReportStorageService();
            var report = new LabReport
            {
                id = Guid.NewGuid().ToString("N"),
                labId = labId,
                studentName = student ?? string.Empty,
                groupName = group ?? string.Empty,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                finalResult = result,
                absoluteError = absError,
                relativeError = relError,
                isPassed = !double.IsNaN(relError) && !double.IsInfinity(relError) && relError >= 0d && relError < 10d
            };

            string savedPath = storageService.SaveReport(report);
            if (pdfGenerator != null) pdfGenerator.GeneratePDFReport(report);
            else Debug.LogWarning("PDFReportGenerator biriktirilmagan. JSON hisobot saqlandi: " + savedPath, this);
            Debug.Log($"Laboratoriya {labId} hisoboti saqlandi: {savedPath}", this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}