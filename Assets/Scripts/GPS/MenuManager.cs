using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("MenuGeolocalizacion");
    }

    public void Jugar()
    {
        SceneManager.LoadScene("Geolocalizacion");
    }

    public void Instrucciones()
    {
        SceneManager.LoadScene("TutorialGeolocalizacion");
    }
}
