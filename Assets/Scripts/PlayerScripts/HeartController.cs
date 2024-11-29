using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    public Image[] hearts;          // Array das imagens dos corações
    public Sprite fullHeart;        // Sprite do coração cheio
    public Sprite emptyHeart;       // Sprite do coração vazio
    
    private PlayerHealth playerHealth;  // Referência ao script de vida do player
    private PlayerSkills playerSkills;  // Referência ao script de habilidades do player

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerSkills = FindObjectOfType<PlayerSkills>();  // Obtem o script PlayerSkills para verificar a habilidade de pulo duplo
        
        // Verifica se o PlayerHealth e PlayerSkills foram encontrados
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

        // Ajusta o número de corações com base na habilidade
        if (playerSkills != null && playerSkills.hasHeart)
        {
            AdjustHeartsForHeartAbility();
        }
    }

    void Update()
    {
        // Atualiza os corações sempre que a vida do player mudar
        if (playerHealth != null)
        {
            UpdateHearts(playerHealth.currentHealth);
        }
    }

    public void UpdateHearts(int health)
    {
        // Atualiza os corações visíveis na UI
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    private void AdjustHeartsForHeartAbility()
    {
        // Se o jogador tem a habilidade de pulo duplo, altere a quantidade de corações para 4
        if (hearts.Length < 4)
        {
            // Expandir o array de corações se necessário
            Image[] newHearts = new Image[4];
            for (int i = 0; i < hearts.Length; i++)
            {
                newHearts[i] = hearts[i];  // Copiar os corações antigos para o novo array
            }
            hearts = newHearts;  // Substitui o array original de corações
        }

        // Agora o número de corações é 4, então atualize o coração exibido
        UpdateHearts(playerHealth.currentHealth);
    }
}
