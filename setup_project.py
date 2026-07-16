import os

# Loyiha strukturasi va fayllar xaritasi
project_structure = {
    # 1. CORE SCRIPTS
    "Assets/_Core/Scripts/Physics/IMeasurable.cs": """using UnityEngine;

namespace VirtualPhysicalLab.Core.Physics
{
    public interface IMeasurable
    {
        float GetMeasurement();
        float GetInstrumentError();
    }
}""",

    "Assets/_Core/Scripts/Physics/PhysicsFormulaEvaluator.cs": """using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualPhysicalLab.Core.Physics
{
    public static class PhysicsFormulaEvaluator
    {
        public static double CalculateMean(List<double> values)
        {
            if (values == null || values.Count == 0) return 0;
            return values.Average();
        }

        public static List<double> CalculateAbsoluteErrors(List<double> values, double mean)
        {
            return values.Select(v => Math.Abs(mean - v)).ToList();
        }

        public static double CalculateMeanAbsoluteError(List<double> absoluteErrors)
        {
            if (absoluteErrors == null || absoluteErrors.Count == 0) return 0;
            return absoluteErrors.Average();
        }

        public static double CalculateRelativeError(double meanAbsoluteError, double mean)
        {
            if (mean == 0) return 0;
            return (meanAbsoluteError / mean) * 100.0;
        }
    }
}""",

    "Assets/_Core/Scripts/UI/GlassmorphismUIController.cs": """using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace VirtualPhysicalLab.Core.UI
{
    public class GlassmorphismUIController : MonoBehaviour
    {
        [Header("UI Elementlari")]
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TextMeshProUGUI measurementValueText;
        [SerializeField] private TextMeshProUGUI errorRateText;
        [SerializeField] private Image neonIndicator;

        private void Start()
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutBack);
        }

        public void UpdateUI(float measuredValue, float error)
        {
            measurementValueText.text = $"{measuredValue:F2} mm";
            errorRateText.text = $"Asbob xatoligi: ±{error:F2} mm";
            neonIndicator.transform.DOScale(1.1f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }
}""",

    # 2. REPORTS
    "Assets/_Core/Scripts/Reports/LabReportModel.cs": """using System;

namespace VirtualPhysicalLab.Core.Reports
{
    [Serializable]
    public class LabReport
    {
        public string id;
        public int labId;
        public string studentName;
        public string groupName;
        public string timestamp;
        public string rawMeasurementsJson;
        public double finalResult;
        public double absoluteError;
        public double relativeError;
        public bool isPassed;
    }
}""",

    "Assets/_Core/Scripts/Reports/ReportStorageService.cs": """using UnityEngine;
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
}""",

    "Assets/_Core/Scripts/Reports/CSVExporter.cs": """using System.IO;
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
}""",

    "Assets/_Core/Scripts/Reports/PDFReportGenerator.cs": """using System;
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
}""",

    # 3. INTERACTIVE TOOLS
    "Assets/SharedAssets/Scripts/Tools/VernierCaliperController.cs": """using UnityEngine;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Tools
{
    public class VernierCaliperController : MonoBehaviour, IMeasurable
    {
        [SerializeField] private Transform sliderTransform;
        [SerializeField] private float minLimitLocalX = 0f;
        [SerializeField] private float maxLimitLocalX = 0.15f;
        [SerializeField] private float realWorldMaxRangeMm = 150f;
        [SerializeField] private float instrumentError = 0.05f;

        private float currentLocalX = 0f;
        private Camera mainCamera;

        private void Start() => mainCamera = Camera.main;

        private void OnMouseDrag()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 localPoint = transform.InverseTransformPoint(hit.point);
                currentLocalX = Mathf.Clamp(localPoint.x, minLimitLocalX, maxLimitLocalX);
                sliderTransform.localPosition = new Vector3(currentLocalX, sliderTransform.localPosition.y, sliderTransform.localPosition.z);
            }
        }

        public float GetMeasurement()
        {
            float t = (currentLocalX - minLimitLocalX) / (maxLimitLocalX - minLimitLocalX);
            float calculatedValue = t * realWorldMaxRangeMm;
            return Mathf.Round(calculatedValue / instrumentError) * instrumentError;
        }

        public float GetInstrumentError() => instrumentError;
    }
}""",

    "Assets/SharedAssets/Scripts/Tools/MicrometerController.cs": """using UnityEngine;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Tools
{
    public class MicrometerController : MonoBehaviour, IMeasurable
    {
        [SerializeField] private Transform thimbleTransform;
        [SerializeField] private Transform spindleTransform;
        [SerializeField] private float threadPitchMm = 0.5f;
        [SerializeField] private float spindleTravelPerRotation = 0.005f;
        [SerializeField] private float instrumentError = 0.01f;
        
        private float currentRotationAngle = 0f;
        private float maxRotations = 50f;

        private void OnMouseDrag()
        {
            float mouseY = Input.GetAxis("Mouse Y");
            float rotationAmount = mouseY * 150f * Time.deltaTime;
            currentRotationAngle = Mathf.Clamp(currentRotationAngle + rotationAmount, 0f, maxRotations * 360f);
            
            thimbleTransform.localRotation = Quaternion.Euler(currentRotationAngle, 0f, 0f);
            float newSpindleLocalX = (currentRotationAngle / 360f) * spindleTravelPerRotation;
            spindleTransform.localPosition = new Vector3(newSpindleLocalX, spindleTransform.localPosition.y, spindleTransform.localPosition.z);
        }

        public float GetMeasurement()
        {
            float rawValue = (currentRotationAngle / 360f) * threadPitchMm;
            return Mathf.Round(rawValue / instrumentError) * instrumentError;
        }

        public float GetInstrumentError() => instrumentError;
    }
}""",

    # 4. PHYSICS SIMULATIONS
    "Assets/Laboratories/Lab01_Zichlik/Scripts/CylinderDensityPhysics.cs": """using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.LabDensity
{
    public class CylinderDensityPhysics : MonoBehaviour
    {
        [SerializeField] private float targetMassKg = 0.15f;
        private List<double> measuredDensities = new List<double>();

        public void AddMeasurement(float dMm, float hMm, float mGram)
        {
            double d = dMm / 1000.0;
            double h = hMm / 1000.0;
            double m = mGram / 1000.0;

            double volume = (System.Math.PI * System.Math.Pow(d, 2) * h) / 4.0;
            double density = m / volume;
            measuredDensities.Add(density);
        }

        public double GetFinalDensityResults(out double absoluteError, out double relativeError)
        {
            double meanDensity = PhysicsFormulaEvaluator.CalculateMean(measuredDensities);
            List<double> absErrors = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredDensities, meanDensity);
            absoluteError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(absErrors);
            relativeError = PhysicsFormulaEvaluator.CalculateRelativeError(absoluteError, meanDensity);

            return meanDensity;
        }
    }
}""",

    "Assets/Laboratories/Lab02_Galiley/Scripts/GalileoTroughPhysics.cs": """using UnityEngine;
using System.Collections;

namespace VirtualPhysicalLab.Laboratories.LabGalileo
{
    public class GalileoTroughPhysics : MonoBehaviour
    {
        [SerializeField] private Transform ballTransform;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float distanceS = 1.2f;
        [SerializeField] private float accelerationA = 0.5f;

        private float elapsedTime = 0f;
        private bool isRolling = false;

        public delegate void BallReachedEnd(float finalTime);
        public event BallReachedEnd OnBallReachedEnd;

        public void StartSimulation()
        {
            ballTransform.position = startPoint.position;
            elapsedTime = 0f;
            isRolling = true;
            StartCoroutine(RollBallRoutine());
        }

        private IEnumerator RollBallRoutine()
        {
            Vector3 startPos = startPoint.position;
            Vector3 endPos = endPoint.position;

            while (isRolling)
            {
                elapsedTime += Time.deltaTime;
                float currentDistance = 0.5f * accelerationA * Mathf.Pow(elapsedTime, 2);
                float progress = currentDistance / distanceS;

                if (progress >= 1.0f)
                {
                    progress = 1.0f;
                    isRolling = false;
                    ballTransform.position = endPos;
                    OnBallReachedEnd?.Invoke(elapsedTime);
                }
                else
                {
                    ballTransform.position = Vector3.Lerp(startPos, endPos, progress);
                }
                yield return null;
            }
        }
    }
}""",

    "Assets/Laboratories/Lab12_Prujina/Scripts/SpringPendulumPhysics.cs": """using UnityEngine;

namespace VirtualPhysicalLab.Laboratories.LabSpring
{
    public class SpringPendulumPhysics : MonoBehaviour
    {
        [SerializeField] private Transform weightTransform;
        [SerializeField] private LineRenderer springMesh;
        [SerializeField] private float massM = 0.2f;
        [SerializeField] private float stiffnessK = 40f;
        [SerializeField] private float dampingGamma = 0.15f;
        [SerializeField] private float amplitudeA = 0.2f;

        private float time = 0f;
        private Vector3 initialWeightPosition;
        private bool isOscillating = false;

        private void Start() => initialWeightPosition = weightTransform.localPosition;

        public void ToggleOscillation(bool start)
        {
            isOscillating = start;
            if (!start)
            {
                weightTransform.localPosition = initialWeightPosition;
                time = 0f;
            }
        }

        private void Update()
        {
            if (!isOscillating) return;

            time += Time.deltaTime;
            float omegaSqr = (stiffnessK / massM) - Mathf.Pow(dampingGamma, 2);
            if (omegaSqr <= 0) return;
            
            float omega = Mathf.Sqrt(omegaSqr);
            float displacement = amplitudeA * Mathf.Exp(-dampingGamma * time) * Mathf.Cos(omega * time);

            weightTransform.localPosition = initialWeightPosition + new Vector3(0, displacement, 0);
            UpdateSpringVisuals(displacement);
        }

        private void UpdateSpringVisuals(float currentDisplacement)
        {
            int verticesCount = 50;
            springMesh.positionCount = verticesCount;
            float totalLength = Mathf.Abs(initialWeightPosition.y - weightTransform.localPosition.y) + 0.5f;

            for (int i = 0; i < verticesCount; i++)
            {
                float t = (float)i / (verticesCount - 1);
                float x = Mathf.Sin(t * Mathf.PI * 20) * 0.05f;
                float z = Mathf.Cos(t * Mathf.PI * 20) * 0.05f;
                float y = -t * totalLength;

                springMesh.SetPosition(i, new Vector3(x, y, z));
            }
        }
    }
}"""
}

# Papkalar va fayllarni yaratish
print("=== VIRTUAL PHYSICAL LAB - LOYIHA GENERATORI ===")
for path, content in project_structure.items():
    directory = os.path.dirname(path)
    if not os.path.exists(directory):
        os.makedirs(directory)
        print(f"[PAPKA YARATILDI] {directory}")
    
    with open(path, "w", encoding="utf-8") as file:
        file.write(content)
        print(f"[FAYL YOZILDI] {path}")

print("\\n=== TAYYOR! Barcha kodlar va struktura muvaffaqiyatli generatsiya qilindi! ===")
