
using System;
using TMPro;
using UnityEngine;

public class compassController : MonoBehaviour
{
    public double dinosaurLat;
    public double dinosaurLon;
    public RectTransform arrow;

    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
    }

    void Update()
    {
        if (GPSTracker.Instance == null || GPSTracker.Instance.currentDinosaurIndex >= GPSTracker.Instance.dinosaurs.Count)
            return;

        Dinosaur currentDinosaur = GPSTracker.Instance.dinosaurs[GPSTracker.Instance.currentDinosaurIndex];

        float heading = Input.compass.trueHeading;
        float bearing = CalculateBearing(
            GPSTracker.Instance.currentLat,
            GPSTracker.Instance.currentLon,
            currentDinosaur.latitude,
            currentDinosaur.longitude
        );

        float angle = heading - bearing;
        arrow.localRotation = Quaternion.Euler(0, 0, angle);
    }

    float CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        double dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        lat1 *= Mathf.Deg2Rad;
        lat2 *= Mathf.Deg2Rad;

        double y = Math.Sin((float)dLon) * Math.Cos((float)lat2);
        double x = Math.Cos((float)lat1) * Math.Sin((float)lat2) -
                   Math.Sin((float)lat1) * Math.Cos((float)lat2) * Math.Cos((float)dLon);

        double brng = Math.Atan2(y, x);
        brng = brng * Mathf.Rad2Deg;
        return (float)((brng + 360) % 360);
    }
}

//using UnityEngine;

//public class CompassController : MonoBehaviour
//{
//    public RectTransform
//        arrowUI;
//    public GPSTracker gpsTracker;

//    private double monsterLat = 37.192234021601394;
//    private double monsterLon = -3.6166183279322683;

//    void Start()
//    {
//        Input.compass.enabled = true;
//    }
//    void Update()
//    {
//        UIManager.Instance.MostrarMensaje(gpsTracker.currentLat.ToString());
//        float heading = Input.compass.trueHeading;
//        float bearing = CalculateBearing(
//        gpsTracker.currentLat,
//        gpsTracker.currentLon,
//        monsterLat,
//        monsterLon);

//        float angle = heading - bearing;
//        arrowUI.localRotation = Quaternion.Euler(0, 0, angle);
//    }
//    float CalculateBearing(double lat1, double lon1, double lat2, double lon2)
//    {
//        double dLon = (lon2 - lon1) * Mathf.Deg2Rad;
//        lat1 *= Mathf.Deg2Rad;
//        lat2 *= Mathf.Deg2Rad;

//        double y = Mathf.Sin((float)dLon) * Mathf.Cos((float)lat2);
//        double x = Mathf.Cos((float)lat1) * Mathf.Sin((float)lat2) -
//        Mathf.Sin((float)lat1) * Mathf.Cos((float)lat2) *
//        Mathf.Cos((float)dLon);

//        double brng = Mathf.Atan2((float)y, (float)x);
//        brng = brng * Mathf.Rad2Deg;
//        return (float)((brng + 360) % 360);
//    }
//}