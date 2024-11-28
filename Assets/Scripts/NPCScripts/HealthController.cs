using System.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 3; // Máximo de vidas
    private int currentHealth;
    private SpriteRenderer sr;

    public GameObject itemPrefab; // Prefab do item a ser dropado

    void Start()
    {
        // Define a vida inicial do inimigo
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    // Função para reduzir a vida do inimigo
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageEffectSequence(sr, Color.red, 0.3f, 0.0f));
        }
    }

    void Die()
    {
        // Dropa o item no local do inimigo
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        // Destroi o inimigo
        Destroy(gameObject);
    }

    IEnumerator DamageEffectSequence(SpriteRenderer sr, Color dmgColor, float duration, float delay)
    {
        // Salva a cor original
        Color originColor = sr.color;

        // Tinge o sprite com a cor de dano
        sr.color = dmgColor;

        // Você pode atrasar a animação
        yield return new WaitForSeconds(delay);

        // Animação de transição de cor com duração definida
        for (float t = 0; t < 1.0f; t += Time.deltaTime / duration)
        {
            sr.color = Color.Lerp(dmgColor, originColor, t);
            yield return null;
        }

        // Restaura a cor original
        sr.color = originColor;
    }
}
