import os

# 21 ta Lab va barcha Core tizimlar xaritasi
project_structure = {
    # ==========================================
    # 1. CORE FIZIKA VA INTERFEYSLAR
    # ==========================================
    "Assets/_Core/Scripts/Physics/IMeasurable.cs": '''using UnityEngine;

namespace VirtualPhysicalLab.Core.Physics
{
    public interface IMeasurable
    {
        float GetMeasurement();
        float GetInstrumentError();
    }
}''',

    "Assets/_Core/Scripts/Physics/PhysicsFormulaEvaluator.cs": '''using System;
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
}''',

    # ==========================================
    # 2. GLOBAL LAB MENEJER VA UI TIZIMI
    # ==========================================
    "Assets/_Core/Scripts/LabManager.cs": '''using UnityEngine;
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
}''',

    "Assets/_Core/Scripts/UI/GlassmorphismUIController.cs": '''using UnityEngine;
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
}''',

    # ==========================================
    # 3. HISOBOTLAR VA EKSPORT XIZMATLARI
    # ==========================================
    "Assets/_Core/Scripts/Reports/LabReportModel.cs": '''using System;

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
}''',

    "Assets/_Core/Scripts/Reports/ReportStorageService.cs": '''using UnityEngine;
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
}''',

    "Assets/_Core/Scripts/Reports/CSVExporter.cs": '''using System.IO;
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
}''',

    "Assets/_Core/Scripts/Reports/PDFReportGenerator.cs": '''using System;
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
                    <p>Xulosa: {(report.isPassed ? "TAYYOR / QABUL QILINDI" : "XATOLIK YUQORI / QAYTA TOPSHIRILSIN")}</p>
                </div>
            </body>
            </html>";

            #if UNITY_WEBGL && !UNITY_EDITOR
            WebGL_PrintHTML(htmlTemplate);
            #else
            string path = Path.Combine(Application.persistentDataPath, $"Hisobot_Lab{report.labId}.html");
            File.WriteAllText(path, htmlTemplate);
            Debug.Log($"PDF uchun HTML saqlandi: {path}");
            #endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void WebGL_PrintHTML(string htmlContent);
        #endif
    }
}''',

    # ==========================================
    # 4. INTERAKTIV O'LCHOV ASBOBLARI (3D)
    # ==========================================
    "Assets/SharedAssets/Scripts/Tools/VernierCaliperController.cs": '''using UnityEngine;
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
}''',

    "Assets/SharedAssets/Scripts/Tools/MicrometerController.cs": '''using UnityEngine;
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
}''',

    # ==========================================
    # 5. BARCHA 21 TA LAB FISIKA MODELLARI (C#)
    # ==========================================
    
    # Lab 1: Silindrik jism zichligi
    "Assets/Laboratories/Lab01_Zichlik/Scripts/CylinderDensityPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab01
{
    public class CylinderDensityPhysics : MonoBehaviour
    {
        private List<double> measuredDensities = new List<double>();

        public void AddMeasurement(float dMm, float hMm, float mGram)
        {
            double d = dMm / 1000.0;
            double h = hMm / 1000.0;
            double m = mGram / 1000.0;
            double volume = (System.Math.PI * System.Math.Pow(d, 2) * h) / 4.0;
            measuredDensities.Add(m / volume);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredDensities);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredDensities, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 2: Galiley tarnovi
    "Assets/Laboratories/Lab02_Galiley/Scripts/GalileoTroughPhysics.cs": '''using UnityEngine;
using System.Collections;

namespace VirtualPhysicalLab.Laboratories.Lab02
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
}''',

    # Lab 3: Sirpanish ishqalanish koeffitsienti
    "Assets/Laboratories/Lab03_Ishqalanish/Scripts/FrictionPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab03
{
    public class FrictionPhysics : MonoBehaviour
    {
        [SerializeField] private float normalForceN = 2.0f;
        private List<double> measuredCoefficients = new List<double>();

        public void AddMeasurement(float frictionForceN)
        {
            double mu = frictionForceN / normalForceN;
            measuredCoefficients.Add(mu);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredCoefficients);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredCoefficients, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 4: Prujinali mayatnik
    "Assets/Laboratories/Lab04_Prujina/Scripts/SpringPendulumPhysics.cs": '''using UnityEngine;

namespace VirtualPhysicalLab.Laboratories.Lab04
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
}''',

    # Lab 5: Matematik mayatnik bilan g ni aniqlash
    "Assets/Laboratories/Lab05_Mayatnik/Scripts/PendulumGravityPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab05
{
    public class PendulumGravityPhysics : MonoBehaviour
    {
        [SerializeField] private float threadLengthL = 1.0f;
        private List<double> measuredGravityValues = new List<double>();

        public void AddMeasurement(float totalTimeSec, int oscillationsN)
        {
            double T = (double)totalTimeSec / oscillationsN;
            double g = (4.0 * System.Math.Pow(System.Math.PI, 2) * threadLengthL) / System.Math.Pow(T, 2);
            measuredGravityValues.Add(g);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredGravityValues);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredGravityValues, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 6: Attvud mashinasi
    "Assets/Laboratories/Lab06_Attvud/Scripts/AttwoodMachinePhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab06
{
    public class AttwoodMachinePhysics : MonoBehaviour
    {
        private List<double> measuredGValues = new List<double>();

        public void CalculateGravity(float m1Grams, float m2Grams, float measuredAcceleration)
        {
            double m1 = m1Grams / 1000.0;
            double m2 = m2Grams / 1000.0;
            double g = (measuredAcceleration * (m1 + m2)) / (m2 - m1);
            measuredGValues.Add(g);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredGValues);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredGValues, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 7: Ballistik mayatnik
    "Assets/Laboratories/Lab07_Ballistik/Scripts/BallisticPendulumPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab07
{
    public class BallisticPendulumPhysics : MonoBehaviour
    {
        private List<double> measuredVelocities = new List<double>();

        public void CalculateBulletVelocity(float bulletMassG, float pendulumMassG, float lengthM, float angleDeg)
        {
            double m = bulletMassG / 1000.0;
            double M = pendulumMassG / 1000.0;
            double g = 9.81;
            double rad = angleDeg * Mathf.Deg2Rad;
            double h = lengthM * (1.0 - System.Math.Cos(rad));
            double v = ((m + M) / m) * System.Math.Sqrt(2.0 * g * h);
            measuredVelocities.Add(v);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredVelocities);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredVelocities, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 8: Oberbek mayatniki
    "Assets/Laboratories/Lab08_Oberbek/Scripts/OberbeckPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab08
{
    public class OberbeckPhysics : MonoBehaviour
    {
        private List<double> measuredInertia = new List<double>();

        public void AddMeasurement(float massGrams, float pulleyRadiusM, float angleRad, float timeSec)
        {
            double m = massGrams / 1000.0;
            double g = 9.81;
            double F = m * g;
            double momentM = F * pulleyRadiusM;
            double epsilon = (2.0 * angleRad) / System.Math.Pow(timeSec, 2);
            measuredInertia.Add(momentM / epsilon);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredInertia);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredInertia, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 9: Maksvel mayatniki
    "Assets/Laboratories/Lab09_Maksvel/Scripts/MaxwellPendulumPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab09
{
    public class MaxwellPendulumPhysics : MonoBehaviour
    {
        private List<double> measuredInertia = new List<double>();

        public void AddMeasurement(float massGrams, float shaftRadiusM, float heightM, float timeSec)
        {
            double m = massGrams / 1000.0;
            double g = 9.81;
            double I = m * System.Math.Pow(shaftRadiusM, 2) * (((g * System.Math.Pow(timeSec, 2)) / (2.0 * heightM)) - 1.0);
            measuredInertia.Add(I);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredInertia);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredInertia, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 10: Markazdan qochma kuch
    "Assets/Laboratories/Lab10_MarkazdanQochma/Scripts/CentrifugalForcePhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab10
{
    public class CentrifugalForcePhysics : MonoBehaviour
    {
        private List<double> measuredForces = new List<double>();

        public void AddMeasurement(float massGrams, float radiusM, int rotations, float timeSec)
        {
            double m = massGrams / 1000.0;
            double omega = (2.0 * System.Math.PI * rotations) / timeSec;
            double Fc = m * System.Math.Pow(omega, 2) * radiusM;
            measuredForces.Add(Fc);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredForces);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredForces, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 11: Yung moduli
    "Assets/Laboratories/Lab11_Yung/Scripts/YoungModulusPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab11
{
    public class YoungModulusPhysics : MonoBehaviour
    {
        private List<double> measuredE = new List<double>();

        public void AddMeasurement(float forceN, float initialLengthM, float wireDiameterMm, float deltaLMm)
        {
            double d = wireDiameterMm / 1000.0;
            double deltaL = deltaLMm / 1000.0;
            double S = (System.Math.PI * System.Math.Pow(d, 2)) / 4.0;
            double E = (forceN * initialLengthM) / (S * deltaL);
            measuredE.Add(E);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredE);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredE, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 12: Siljish moduli
    "Assets/Laboratories/Lab12_Siljish/Scripts/ShearModulusPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab12
{
    public class ShearModulusPhysics : MonoBehaviour
    {
        private List<double> measuredG = new List<double>();

        public void AddMeasurement(float torqueM, float lengthM, float radiusMm, float angleDeg)
        {
            double r = radiusMm / 1000.0;
            double alpha = angleDeg * System.Math.PI / 180.0;
            double G = (2.0 * torqueM * lengthM) / (System.Math.PI * System.Math.Pow(r, 4) * alpha);
            measuredG.Add(G);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredG);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredG, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 13: Yuzaki taranglik (Tomchi usuli)
    "Assets/Laboratories/Lab13_YuzakiTaranglik/Scripts/SurfaceTensionPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab13
{
    public class SurfaceTensionPhysics : MonoBehaviour
    {
        private List<double> measuredSigma = new List<double>();

        public void AddMeasurement(float totalMassGrams, int dropCount, float pipetteDiameterMm)
        {
            double m = totalMassGrams / 1000.0;
            double d = pipetteDiameterMm / 1000.0;
            double g = 9.81;
            double sigma = (m * g) / (System.Math.PI * d * dropCount);
            measuredSigma.Add(sigma);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredSigma);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredSigma, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 14: Suyuqlik yopishqoqligi (Stoks usuli)
    "Assets/Laboratories/Lab14_Yopishqoqlik/Scripts/ViscosityStokesPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab14
{
    public class ViscosityStokesPhysics : MonoBehaviour
    {
        private List<double> measuredEta = new List<double>();

        public void AddMeasurement(float ballRadiusMm, float rhoBall, float rhoLiquid, float velocityMS)
        {
            double r = ballRadiusMm / 1000.0;
            double g = 9.81;
            double eta = (2.0 * System.Math.Pow(r, 2) * (rhoBall - rhoLiquid) * g) / (9.0 * velocityMS);
            measuredEta.Add(eta);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredEta);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredEta, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 15: Adiabata koeffitsienti
    "Assets/Laboratories/Lab15_Adiabata/Scripts/AdiabaticPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab15
{
    public class AdiabaticPhysics : MonoBehaviour
    {
        private List<double> measuredGamma = new List<double>();

        public void AddMeasurement(float h1Mm, float h2Mm)
        {
            if (Mathf.Approximately(h1Mm, h2Mm)) return;
            double gamma = (double)h1Mm / (h1Mm - h2Mm);
            measuredGamma.Add(gamma);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredGamma);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredGamma, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 16: Issiqlik sig'imi (Kalorimetriya)
    "Assets/Laboratories/Lab16_Kalorimetriya/Scripts/CalorimetryPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab16
{
    public class CalorimetryPhysics : MonoBehaviour
    {
        private List<double> measuredC = new List<double>();

        public void AddMeasurement(float mSuvG, float mJismG, float tSuv, float tJism, float tAralash, float cWater = 4187f, float cCalorimeter = 50f)
        {
            double mSuv = mSuvG / 1000.0;
            double mJism = mJismG / 1000.0;
            double numerator = (mSuv * cWater + cCalorimeter) * (tAralash - tSuv);
            double denominator = mJism * (tJism - tAralash);
            measuredC.Add(numerator / denominator);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredC);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredC, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 17: Issiqlik o'tkazuvchanlik
    "Assets/Laboratories/Lab17_IssiqlikOtkaruvchanlik/Scripts/ThermalConductivityPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab17
{
    public class ThermalConductivityPhysics : MonoBehaviour
    {
        private List<double> measuredLambda = new List<double>();

        public void AddMeasurement(float heatQ, float thicknessM, float areaS2, float deltaT, float timeSec)
        {
            double lambda = (heatQ * thicknessM) / (areaS2 * deltaT * timeSec);
            measuredLambda.Add(lambda);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredLambda);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredLambda, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 18: Boyl-Mariott qonuni
    "Assets/Laboratories/Lab18_BoylMariott/Scripts/BoyleMariottePhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab18
{
    public class BoyleMariottePhysics : MonoBehaviour
    {
        private List<double> measuredK = new List<double>();

        public void AddMeasurement(float pressurePascal, float volumeM3)
        {
            double K = (double)pressurePascal * volumeM3;
            measuredK.Add(K);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredK);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredK, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 19: Gey-Lyussak qonuni
    "Assets/Laboratories/Lab19_GeyLyussak/Scripts/GayLussacPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab19
{
    public class GayLussacPhysics : MonoBehaviour
    {
        private List<double> measuredAlpha = new List<double>();

        public void AddMeasurement(float volumeInitial, float volumeFinal, float deltaTempCelsius)
        {
            double alpha = (volumeFinal - volumeInitial) / (volumeInitial * deltaTempCelsius);
            measuredAlpha.Add(alpha);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredAlpha);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredAlpha, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 20: Sharl qonuni
    "Assets/Laboratories/Lab20_Sharl/Scripts/CharlesLawPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab20
{
    public class CharlesLawPhysics : MonoBehaviour
    {
        private List<double> measuredBeta = new List<double>();

        public void AddMeasurement(float pressureInitial, float pressureFinal, float deltaTempCelsius)
        {
            double beta = (pressureFinal - pressureInitial) / (pressureInitial * deltaTempCelsius);
            measuredBeta.Add(beta);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredBeta);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredBeta, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}''',

    # Lab 21: Van-der-Vaals real gaz qonuni
    "Assets/Laboratories/Lab21_VanDerWaals/Scripts/VanDerWaalsPhysics.cs": '''using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab21
{
    public class VanDerWaalsPhysics : MonoBehaviour
    {
        private List<double> measuredA = new List<double>();
        private List<double> measuredB = new List<double>();

        public void CalculateConstants(float criticalPressurePascal, float criticalVolumeM3)
        {
            double a = 3.0 * criticalPressurePascal * System.Math.Pow(criticalVolumeM3, 2);
            double b = criticalVolumeM3 / 3.0;
            measuredA.Add(a);
            measuredB.Add(b);
        }

        public void GetFinalResults(out double meanA, out double meanB)
        {
            meanA = PhysicsFormulaEvaluator.CalculateMean(measuredA);
            meanB = PhysicsFormulaEvaluator.CalculateMean(measuredB);
        }
    }
}'''
}

# Papkalar va fayllarni generatsiya qilish
print("==================================================")
print("   VIRTUAL FIZIKA LAB: 21 TALIK TO'LIQ GENERATOR   ")
print("==================================================")

for path, content in project_structure.items():
    directory = os.path.dirname(path)
    if not os.path.exists(directory):
        os.makedirs(directory)
        print(f"[+] Papka yaratildi: {directory}")
    
    with open(path, "w", encoding="utf-8") as file:
        file.write(content)
        print(f"[Fayl yozildi]: {path}")

print("\n==================================================")
print(" TAYYOR! Barcha 21 ta labaratoriya kodi yaratildi! ")
print("==================================================")
