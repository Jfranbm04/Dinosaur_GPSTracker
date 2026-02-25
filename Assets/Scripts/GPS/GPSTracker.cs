using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GPSTracker : MonoBehaviour
{
    public static GPSTracker Instance;

    [Header("AR - Dinosaurios (3 Prefabs)")]
    public ARRaycastManager raycastManager;
    public GameObject dinosaurio1Prefab; 
    public GameObject dinosaurio2Prefab; 
    public GameObject dinosaurio3Prefab; 
    private GameObject spawnedDinosaur;

    [Header("UI General")]
    public TextMeshProUGUI gpsStatusText;
    public TextMeshProUGUI distanceText;
    public GameObject panelVictoria;

    [Header("UI Minijuego de Captura")]
    public GameObject combatUI; 
    public RectTransform captureArea; 
    public GameObject captureButtonPrefab; 
    public TextMeshProUGUI timerText; 

    [Header("Configuración Minijuego")]
    public int buttonsToSpawn = 5; 
    public float timeToCapture = 5f; 

    private float currentTimer;
    private int buttonsPressed;
    private bool isMinigameActive = false;
    private List<GameObject> activeButtons = new List<GameObject>();

    public double currentLat;
    public double currentLon;
    private bool isSpawned = false;
    private float detectionRadius = 15f;

    public List<Dinosaur> dinosaurs = new List<Dinosaur>();
    public int currentDinosaurIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        panelVictoria.SetActive(false);
        combatUI.SetActive(false);
        StartGPS();
        LoadDinosaur();
    }

    void Update()
    {
        UpdateGPS();

        if (Input.location.status == LocationServiceStatus.Running)
        {
            CheckDinosaurDistance();
        }

        // Lógica del temporizador del minijuego
        if (isMinigameActive)
        {
            currentTimer -= Time.deltaTime;
            timerText.text = $"Tiempo: {currentTimer:F1}s";

            if (currentTimer <= 0)
            {
                LoseMinigame();
            }
        }
    }

    #region GPS

    void StartGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            gpsStatusText.text = "GPS no habilitado";
            return;
        }
        Input.location.Start();
    }

    void UpdateGPS()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            currentLat = Input.location.lastData.latitude;
            currentLon = Input.location.lastData.longitude;
            gpsStatusText.text = $"Lat: {currentLat}\nLon: {currentLon}";
        }
    }

    #endregion

    #region Dinosaurs

    void LoadDinosaur()
    {
        // Solo cargamos 3 dinosaurios para que coincidan con los 3 prefabs
        dinosaurs.Add(new Dinosaur { dinosaurName = "Triceratops", latitude = 37.19222686616466, longitude = -3.616983154096119, defeated = false });
        dinosaurs.Add(new Dinosaur { dinosaurName = "Velociraptor", latitude = 37.191878754572755, longitude = -3.617152208305987, defeated = false });
        dinosaurs.Add(new Dinosaur { dinosaurName = "T-Rex", latitude = 37.1922275014041, longitude = -3.6169823566711927, defeated = false });
    }

    #endregion

    #region Distance

    void CheckDinosaurDistance()
    {
        if (currentDinosaurIndex >= dinosaurs.Count) return;

        Dinosaur currentDinosaur = dinosaurs[currentDinosaurIndex];

        double distance = CalculateDistance(currentLat, currentLon, currentDinosaur.latitude, currentDinosaur.longitude);
        distanceText.text = $"Distancia al {currentDinosaur.dinosaurName}: {distance:F2} metros";

        if (distance <= detectionRadius && !isSpawned)
        {
            SpawnDinosaurInAr();
        }
    }

    double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000;
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Mathf.Sin((float)dLat / 2) * Mathf.Sin((float)dLat / 2) +
                   Mathf.Cos((float)ToRadians(lat1)) * Mathf.Cos((float)ToRadians(lat2)) *
                   Mathf.Sin((float)dLon / 2) * Mathf.Sin((float)dLon / 2);

        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));
        return R * c;
    }

    double ToRadians(double degrees) => degrees * Mathf.PI / 180;

    #endregion

    #region Spawn Dinosaur

    void SpawnDinosaurInAr()
    {
        if (isSpawned) return;

        isSpawned = true;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(
            new Vector2(Screen.width / 2, Screen.height / 2),
            hits,
            UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            Vector3 spawnPosition = hits[0].pose.position;
            Quaternion spawnRotation = hits[0].pose.rotation;

            // Seleccionar el prefab fijo correspondiente al índice
            GameObject prefabToSpawn = null;
            if (currentDinosaurIndex == 0) prefabToSpawn = dinosaurio1Prefab;
            else if (currentDinosaurIndex == 1) prefabToSpawn = dinosaurio2Prefab;
            else if (currentDinosaurIndex == 2) prefabToSpawn = dinosaurio3Prefab;

            if (prefabToSpawn != null)
            {
                spawnedDinosaur = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
                StartCaptureMinigame();
            }
        }
        else
        {
            isSpawned = false;
        }
    }

    #endregion

    #region Minijuego de Captura

    void StartCaptureMinigame()
    {
        combatUI.SetActive(true);
        isMinigameActive = true;
        currentTimer = timeToCapture;
        buttonsPressed = 0;

        ClearButtons();

        // Generar los botones aleatorios
        for (int i = 0; i < buttonsToSpawn; i++)
        {
            SpawnRandomButton();
        }
    }

    void SpawnRandomButton()
    {
        GameObject btnObj = Instantiate(captureButtonPrefab, captureArea);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();

        // Calcular posición aleatoria dentro de los límites del captureArea
        float width = captureArea.rect.width;
        float height = captureArea.rect.height;

        // Se asume que el pivote del captureArea es (0.5, 0.5)
        float randomX = Random.Range(-width / 2f, width / 2f);
        float randomY = Random.Range(-height / 2f, height / 2f);

        btnRect.anchoredPosition = new Vector2(randomX, randomY);

        // Añadir evento de pulsación
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(() => OnCaptureButtonClicked(btnObj));

        activeButtons.Add(btnObj);
    }

    public void OnCaptureButtonClicked(GameObject clickedButton)
    {
        if (!isMinigameActive) return;

        activeButtons.Remove(clickedButton);
        Destroy(clickedButton);
        buttonsPressed++;

        if (buttonsPressed >= buttonsToSpawn)
        {
            WinMinigame();
        }
    }

    void WinMinigame()
    {
        isMinigameActive = false;
        ClearButtons();
        DinosaurDefeated(); // Pasa al siguiente dinosaurio
    }

    void LoseMinigame()
    {
        isMinigameActive = false;
        ClearButtons();

        // Si pierdes, el dinosaurio huye. Se resetea el estado para intentarlo de nuevo.
        combatUI.SetActive(false);
        if (spawnedDinosaur != null)
        {
            Destroy(spawnedDinosaur);
            spawnedDinosaur = null;
        }
        isSpawned = false;
    }

    void ClearButtons()
    {
        foreach (var btn in activeButtons)
        {
            if (btn != null) Destroy(btn);
        }
        activeButtons.Clear();
    }

    public void DinosaurDefeated()
    {
        dinosaurs[currentDinosaurIndex].defeated = true;

        Destroy(spawnedDinosaur);
        spawnedDinosaur = null;

        isSpawned = false;
        combatUI.SetActive(false);

        currentDinosaurIndex++;

        if (currentDinosaurIndex >= dinosaurs.Count)
        {
            panelVictoria.SetActive(true);
        }
    }

    #endregion
}