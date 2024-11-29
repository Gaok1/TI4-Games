using UnityEngine;
using UnityEngine.SceneManagement;  // Para gerenciar as cenas

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 4;       // Vida máxima
    public int currentHealth = 3;   // Vida inicial do jogador

    private HeartController heartController;
    private float damageCooldown = 2f;  // Cooldown de dano em segundos
    private float lastDamageTime = -2f; // Tempo do último dano (inicializado para que o jogador possa tomar dano imediatamente)

    void Start()
    {
        heartController = FindObjectOfType<HeartController>();
        if (heartController != null)
        {
            heartController.UpdateHearts(currentHealth); // Atualiza os corações na inicialização
        }
    }

    void Update()
    {
        // Simula o dano ao pressionar a tecla "H" (teste)
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1); // Aplica 1 ponto de dano
        }
    }

    public void TakeDamage(int damage)
    {
        // Verifica se o cooldown de dano já passou
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (heartController != null)
            {
                heartController.UpdateHearts(currentHealth); // Atualiza os corações na interface
            }

            lastDamageTime = Time.time;  // Atualiza o tempo do último dano

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("Player morreu!");

        // Carrega a cena "GameOver" ou qualquer cena que você queira carregar.
        SceneManager.LoadScene("LobbyScene");  // Substitua "GameOver" pelo nome da sua cena.
    }

}
