using System.Collections;
using UnityEngine;

public class EnemyAttackOnPlayer : MonoBehaviour
{
    public Transform[] pontos;  // Array de pontos que o inimigo irá seguir
    public float velocidade = 3f;  // Velocidade de movimento
    public float tempoDeEspera = 1f;  // Tempo de espera em cada ponto

    private int pontoAtual = 0;  // Índice do ponto atual
    private bool movendo = true;  // Flag para verificar se está se movendo
    private bool playerInRange = false;  // Flag para verificar se o player está na área de ataque

    private Transform player;  // Referência ao transform do player
    private float timeBtwShots;  // Tempo entre os tiros
    public float startTimeBtwShots = 1f;  // Tempo inicial entre os tiros
    public GameObject projectile;  // O prefab do projétil
    public Transform shotPoint;  // O ponto de onde os projéteis serão disparados
    public float projectileSpeed = 10f;  // Velocidade do projétil

    private Animator animator;  // Referência ao Animator

    void Start()
    {
        // Pegando o transform do player
        player = GameObject.FindWithTag("Player").transform;

        // Pegando o Animator do inimigo
        animator = GetComponent<Animator>();

        if (pontos.Length > 0)
        {
            StartCoroutine(MoverEntrePontosCoroutine());  // Começa o movimento entre pontos
        }
        else
        {
            Debug.LogError("Nenhum ponto foi atribuído ao array.");
        }
    }

    void Update()
    {
        if (playerInRange) // Se o player estiver dentro do alcance
        {
            if (timeBtwShots <= 0)
            {
                AttackPlayer(); // Ataca o player
                timeBtwShots = startTimeBtwShots; // Reseta o tempo entre os ataques
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
    }

    IEnumerator MoverEntrePontosCoroutine()
    {
        while (true)
        {
            if (movendo)
            {
                MoveParaPonto(pontos[pontoAtual]); // Move o inimigo até o ponto atual

                // Se o inimigo chegar ao ponto, espera e depois vai para o próximo ponto
                if (Vector3.Distance(transform.position, pontos[pontoAtual].position) < 0.1f)
                {
                    movendo = false;
                    yield return new WaitForSeconds(tempoDeEspera);
                    pontoAtual = (pontoAtual + 1) % pontos.Length;
                    movendo = true;
                }
            }

            yield return null;
        }
    }

    void MoveParaPonto(Transform ponto)
    {
        transform.position = Vector3.MoveTowards(transform.position, ponto.position, velocidade * Time.deltaTime);
        AtualizarAnimator(); // Atualiza a animação do inimigo baseado no movimento
    }

    void AtualizarAnimator()
    {
        Vector3 direction = pontos[pontoAtual].position - transform.position;

        if (animator != null)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Movimento horizontal
            {
                if (direction.x > 0)
                    animator.SetInteger("Direction", 2); // Direita
                else
                    animator.SetInteger("Direction", 3); // Esquerda
            }
            else // Movimento vertical
            {
                if (direction.y > 0)
                    animator.SetInteger("Direction", 1); // Cima
                else
                    animator.SetInteger("Direction", 0); // Baixo
            }

            animator.SetBool("IsMoving", true); // Está se movendo
        }
    }

    // Função que é chamada quando o inimigo deve atacar o player
    void AttackPlayer()
    {
        if (player != null)
        {
            // Instancia o projétil e configura sua direção
            GameObject newProjectile = Instantiate(projectile, shotPoint.position, Quaternion.identity);
            Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 directionToPlayer = (player.position - shotPoint.position).normalized;
                rb.velocity = directionToPlayer * projectileSpeed;
            }

            // Toca a animação de ataque, caso tenha
            animator.SetTrigger("Attack");
        }
    }

    // Detecta quando o player entra na área de ataque
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // Player entrou na área de ataque
        }
    }

    // Detecta quando o player sai da área de ataque
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Player saiu da área de ataque
        }
    }
}
