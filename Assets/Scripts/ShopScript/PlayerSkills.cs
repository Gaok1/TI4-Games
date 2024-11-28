using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public bool hasDash = false;        // Habilidade de Dash
    public bool hasHeart = false;  // Habilidade de pulo duplo
    public bool hasAttack = false;      // Habilidade de escudo

    private const string DashKey = "HasDash";
    private const string HeartKey = "HasHeart";
    private const string AttackKey = "HasAttack";

    void Start()
    {
        // Carrega as habilidades do PlayerPrefs
        hasDash = PlayerPrefs.GetInt(DashKey, 0) == 1;
        hasHeart = PlayerPrefs.GetInt(HeartKey, 0) == 1;
        hasAttack = PlayerPrefs.GetInt(AttackKey, 0) == 1;

        // DontDestroyOnLoad(gameObject);
    }

    public void UnlockDash()
    {
        hasDash = true;
        PlayerPrefs.SetInt(DashKey, 1); // Salva que o Dash foi desbloqueado
        PlayerPrefs.Save();
    }

    public void UnlockHeart()
    {
        hasHeart = true;
        PlayerPrefs.SetInt(HeartKey, 1); // Salva que o Pulo Duplo foi desbloqueado
        PlayerPrefs.Save();
    }

    public void UnlockAttack()
    {
        hasAttack = true;
        PlayerPrefs.SetInt(AttackKey, 1); // Salva que o Escudo foi desbloqueado
        PlayerPrefs.Save();
    }
}
