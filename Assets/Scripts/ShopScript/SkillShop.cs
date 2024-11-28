using UnityEngine;
using UnityEngine.UI;

public class SkillShop : MonoBehaviour
{
    public static SkillShop Instance; // Singleton para acesso global
    public Text cyanText;
    public Text greenText;
    public Text purpleText;

    // Referência ao Player para desbloquear habilidades
    public PlayerSkills player;

    // Custo das habilidades
    public int dashCostCyan = 10;
    public int dashCostGreen = 5;
    public int dashCostPurple = 0;

    public int HeartCostCyan = 15;
    public int HeartCostGreen = 10;
    public int HeartCostPurple = 5;

    public int AttackCostCyan = 20;
    public int AttackCostGreen = 15;
    public int AttackCostPurple = 10;

    // Referência aos contadores de itens
    private int cyanCount;
    private int greenCount;
    private int purpleCount;

    private void Awake()
    {
        // Configura o singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Se player não foi atribuído no Inspector, encontra automaticamente
        if (player == null)
        {
            player = FindObjectOfType<PlayerSkills>();
        }

        if (player == null)
        {
            Debug.LogError("PlayerSkills não encontrado! Certifique-se de que o script PlayerSkills esteja atribuído corretamente.");
            return; // Saímos do método para evitar que o código continue sem a referência correta
        }

        int cyanCount = PlayerPrefs.GetInt("CyanCount", 0);
        int greenCount = PlayerPrefs.GetInt("GreenCount", 0);
        int purpleCount = PlayerPrefs.GetInt("PurpleCount", 0);

        SetCounts(cyanCount, greenCount, purpleCount);
    }



    public void SetCounts(int cyan, int green, int purple)
    {
        cyanCount = cyan;
        greenCount = green;
        purpleCount = purple;
        UpdateUI();
    }

    private void UpdateUI()
    {
        cyanText.text = cyanCount.ToString();
        greenText.text = greenCount.ToString();
        purpleText.text = purpleCount.ToString();
    }

    // Função para comprar Dash
    public void BuyDash()
    {
        if (player.hasDash)
        {
            Debug.Log("Dash já desbloqueado!");
            return;
        }

        if (cyanCount >= dashCostCyan && greenCount >= dashCostGreen && purpleCount >= dashCostPurple)
        {
            cyanCount -= dashCostCyan;
            greenCount -= dashCostGreen;
            purpleCount -= dashCostPurple;

            player.UnlockDash(); // Desbloqueia e salva no PlayerPrefs
            Debug.Log("Dash desbloqueado!");

            SaveCounts(); // Salva os valores nos PlayerPrefs
            UpdateUI();
        }
        else
        {
            Debug.Log("Não há itens suficientes para comprar Dash.");
        }
    }

    // Função para comprar Pulo Duplo
    public void BuyHeart()
    {
        if (player.hasHeart)
        {
            Debug.Log("Pulo Duplo já desbloqueado!");
            return; // Interrompe a execução
        }

        if (cyanCount >= HeartCostCyan && greenCount >= HeartCostGreen && purpleCount >= HeartCostPurple)
        {
            cyanCount -= HeartCostCyan;
            greenCount -= HeartCostGreen;
            purpleCount -= HeartCostPurple;

            player.hasHeart = true; // Desbloqueia a habilidade
            Debug.Log("Pulo Duplo desbloqueado!");

            SaveCounts(); // Salva os valores nos PlayerPrefs
            UpdateUI(); // Atualiza a UI
        }
        else
        {
            Debug.Log("Não há itens suficientes para comprar Pulo Duplo.");
        }
    }

    // Função para comprar Escudo
    public void BuyAttack()
    {
        if (player.hasAttack)
        {
            Debug.Log("Escudo já desbloqueado!");
            return; // Interrompe a execução
        }

        if (cyanCount >= AttackCostCyan && greenCount >= AttackCostGreen && purpleCount >= AttackCostPurple)
        {
            cyanCount -= AttackCostCyan;
            greenCount -= AttackCostGreen;
            purpleCount -= AttackCostPurple;

            player.hasAttack = true; // Desbloqueia a habilidade
            Debug.Log("Escudo desbloqueado!");

            SaveCounts(); // Salva os valores nos PlayerPrefs
            UpdateUI(); // Atualiza a UI
        }
        else
        {
            Debug.Log("Não há itens suficientes para comprar Escudo.");
        }
    }

    // Função para salvar os contadores nos PlayerPrefs
    private void SaveCounts()
    {
        PlayerPrefs.SetInt("CyanCount", cyanCount);
        PlayerPrefs.SetInt("GreenCount", greenCount);
        PlayerPrefs.SetInt("PurpleCount", purpleCount);
        PlayerPrefs.Save();
    }

}
