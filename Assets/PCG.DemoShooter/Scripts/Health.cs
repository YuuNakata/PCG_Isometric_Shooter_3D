using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Events")]
    public Action<float, float> OnHealthChanged; // current, max
    public Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        OnDeath?.Invoke();
        
        // Default behavior: destroy the object
        // Enemies will just be destroyed
        // Player could trigger game over instead
        if (CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        else if (CompareTag("Player"))
        {
            Debug.Log("Player Died! Game Over.");
            // For now just log, later can show UI
        }
    }
}
