using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMPro.TextMeshProUGUI textoGPS;

    void Awake() { Instance = this; }

    public void MostrarMensaje(string mensaje)
    {
        textoGPS.text = mensaje;
    }
}
