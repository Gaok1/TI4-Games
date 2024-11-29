using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    public Image[] hearts;          // Array das imagens dos corações
    public Sprite fullHeart;        // Sprite do coração cheio
    public Sprite emptyHeart;       // Sprite do coração vazio
    
    private PlayerHealth playerHealth;  // Referência ao script de vida do player

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        // Verifica se encontrou o PlayerHealth
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth não encontrado! Certifique-se de que o script está no player.");
        }
        else
        {
            UpdateHearts(playerHealth.currentHealth);
        }
        
        // Verifica se os corações estão atribuídos
        if (hearts.Length == 0)
        {
            Debug.LogError("Array de corações não está preenchido no Inspector.");
        }
    }

    void Update()
    {
        // Atualiza apenas se a referência estiver correta
        if (playerHealth != null)
        {
            UpdateHearts(playerHealth.currentHealth);
        }
    }

    public void UpdateHearts(int health)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
