using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GPSTracker : MonoBehaviour
{
    public static GPSTracker Instance;

    [Header("AR")]
    public ARRaycastManager raycastManager;
    public GameObject[] dinosaurPrefabs;
    private GameObject spawnedDinosaur;

    [Header("UI")]
    public TextMeshProUGUI gpsStatusText;
    public TextMeshProUGUI distanceText;
    public GameObject combatUI;
    public GameObject panelVictoria;

    [Header("Music")]
    public AudioSource combatMusic;
    public AudioSource spawnMusic;

    public double currentLat;
    public double currentLon;
    private bool isSpawned = false;
    private float detectionRadius = 15f;

    public List<Dinosaur> dinosaurs = new List<Dinosaur>();
    public int currentDinosaurIndex = 0;
    private DinosaurController db;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        /*Coordenadas para probar*/
        dinosaurs.Add(new Dinosaur { dinosaurName = "T-Rex", latitude = 37.19222686616466, longitude = -3.616983154096119, health = 100, defeated = false });
        dinosaurs.Add(new Dinosaur { dinosaurName = "Triceratops", latitude = 37.191878754572755, longitude = -3.617152208305987, health = 150, defeated = false });
        dinosaurs.Add(new Dinosaur { dinosaurName = "Velociraptor", latitude = 37.1922275014041, longitude = -3.6169823566711927, health = 130, defeated = false });
        dinosaurs.Add(new Dinosaur { dinosaurName = "Estegosaurio", latitude = 37.192095371646886, longitude = -3.616837225226872, health = 120, defeated = false });
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
        double R = 6371000; // Radio de la Tierra en metros
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

            spawnMusic.Play();

            spawnedDinosaur = Instantiate(
                dinosaurPrefabs[currentDinosaurIndex],
                spawnPosition,
                spawnRotation
            );

            db = spawnedDinosaur.GetComponent<DinosaurController>();
            db.Initialize(dinosaurs[currentDinosaurIndex].health, combatUI.GetComponentInChildren<TextMeshProUGUI>());

            if (combatMusic != null && !combatMusic.isPlaying)
            {
                combatMusic.Play();
            }

            combatUI.SetActive(true);
        }
        else
        {
            isSpawned = false;
        }
    }

    #endregion

    #region Combat

    public void Shoot()
    {
        if (spawnedDinosaur != null)
        {
            spawnedDinosaur.GetComponent<DinosaurController>().TakeDamage(20);
        }
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