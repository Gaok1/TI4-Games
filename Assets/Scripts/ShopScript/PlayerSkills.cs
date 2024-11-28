using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance;  // Instância estática para o Singleton
    public bool hasDash = false;          // Habilidade de Dash
    public bool hasHeart = false;         // Habilidade de pulo duplo
    public bool hasAttack = false;        // Habilidade de escudo

    // Você pode adicionar outras habilidades aqui futuramente!

    private void Awake()
    {
        // Implementação do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Garantir que exista apenas uma instância
            return;
        }

        Instance = this; // Atribui a instância caso ainda não tenha sido criada
        DontDestroyOnLoad(gameObject); // Garante que o PlayerSkills não seja destruído entre cenas
    }
}