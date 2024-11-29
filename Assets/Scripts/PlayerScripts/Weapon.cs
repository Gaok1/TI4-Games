using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float offset;

    public GameObject projectile;
    public Transform shotPoint;

    public float projectileSpeed = 10f;

    private float timeBtwShots;
    public float startTimeBtwShots;

    private int direction; // Direction: 0 = Down, 1 = Up, 2 = Right, 3 = Left

    void Update()
    {
        // Get the direction from the character controller (or your movement system)
        direction = Cainos.PixelArtTopDown_Basic.TopDownCharacterController.direction;

        // Update weapon position and rotation based on the direction
        switch (direction)
        {
            case 0: // Down
                transform.localPosition = new Vector2(0, -1);
                transform.rotation = Quaternion.Euler(0f, 0f, 180f + offset);
                break;
            case 1: // Up
                transform.localPosition = new Vector2(0, 1);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f + offset);
                break;
            case 2: // Right
                transform.localPosition = new Vector2(1, 0);
                transform.rotation = Quaternion.Euler(0f, 0f, -90f + offset);
                break;
            case 3: // Left
                transform.localPosition = new Vector2(-1, 0);
                transform.rotation = Quaternion.Euler(0f, 0f, 90f + offset);
                break;
        }

        if (timeBtwShots <= 0)
        {
            if (Input.GetKey(KeyCode.L)) // Replace with your shooting input
            {
                // Instantiate the projectile
                GameObject newProjectile = Instantiate(projectile, shotPoint.position, transform.rotation);

                // Set the projectile's velocity
                Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = transform.right * projectileSpeed; // Moves in the weapon's forward direction
                }

                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }
}
