using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;
    public Image fillImage;
    public Text healthText;
    
    [Header("Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    private Health playerHealth;

    void Start()
    {
        FindPlayerHealth();
    }

    void FindPlayerHealth()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthUI;
                UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
            }
        }
        else
        {
            // Player not spawned yet, try again later
            Invoke(nameof(FindPlayerHealth), 0.5f);
        }
    }

    void UpdateHealthUI(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
        
        // Update color based on health percentage
        if (fillImage != null)
        {
            float percent = current / max;
            if (percent > 0.6f)
                fillImage.color = healthyColor;
            else if (percent > 0.3f)
                fillImage.color = damagedColor;
            else
                fillImage.color = criticalColor;
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }
}
