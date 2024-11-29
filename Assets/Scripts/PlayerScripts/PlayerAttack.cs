using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour{

    private float timeBtwAttack;
    public float startTimeBtwAttack;
    private float RtimeBtwAttack;
    public float RstartTimeBtwAttack;

    public Transform attackPos;
    public LayerMask whatIsEnemy;
    public float attackRange;
    public int damage;


    public static int direction;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if(timeBtwAttack <= 0)
        {
            if (Input.GetKey(KeyCode.K))
            {
                animator.SetTrigger("Atk");
                Collider2D[] enimiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemy);
                for(int i = 0; i< enimiesToDamage.Length; i++)
                {
                    enimiesToDamage[i].GetComponent<HealthController>().TakeDamage(damage);
                }
            }
            timeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackRange);
        }
    }

}