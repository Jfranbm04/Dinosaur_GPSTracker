using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int vidas = 3;
    public int puntuacion = 20;
    public bool isGrounded;
    public TextMeshProUGUI LivesText;
    public TextMeshProUGUI ScoreText;
    public GameObject LosePanel;
    public GameObject WinPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LivesText.text = "Vidas: " + vidas;
        ScoreText.text = "Cangreburgers restantes: " + puntuacion;
        LosePanel.SetActive(false);
        WinPanel.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            vidas--;
            LivesText.text = "Vidas: " + vidas;
            Destroy(other.gameObject);
            
            if (vidas <= 0)
            {
                Time.timeScale = 0;
                LosePanel.SetActive(true);
            }
        }

        if (other.CompareTag("Collectible"))
        {
            puntuacion -= 1;
            ScoreText.text = "Cangreburgers restantes: " + puntuacion;
            Destroy(other.gameObject);

            if (puntuacion < 1)
            {
                Time.timeScale = 0;
                WinPanel.SetActive(true);
            }
        }
    }



    public void ReiniciarJuego()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
