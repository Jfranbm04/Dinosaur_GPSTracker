using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DinosaurController : MonoBehaviour
{
    public int maxhealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText;

    public void Initialize(int health, TextMeshProUGUI uiText)
    {
        maxhealth = health;
        currentHealth = maxhealth;

        healthText = uiText;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Vida: {currentHealth}/{maxhealth}";
    }

    void Die()
    {
        GPSTracker.Instance.DinosaurDefeated();
        Destroy(gameObject);
    }
}