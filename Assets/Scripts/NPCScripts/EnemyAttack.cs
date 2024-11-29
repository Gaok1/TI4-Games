using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 1;  // Quantidade de dano que o inimigo causa
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto colidido tem a tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            
            // Acessa o script PlayerHealth e aplica dano
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                animator.SetTrigger("Atk");
                playerHealth.TakeDamage(damage); // Aplica o dano
            }
        }
    }
}
