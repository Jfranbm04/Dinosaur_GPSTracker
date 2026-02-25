using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Canvas canvasHTP;
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Jugar()
    {
        SceneManager.LoadScene("GPSScene");
    }

    public void HTP()
    {
        canvasHTP.gameObject.SetActive(true);
    }

    public void closeHTP()
    {
        canvasHTP.gameObject.SetActive(false);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
