using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 20f;				// The speed the rocket will fire at.


	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
    private PlayerHealth playerHealth;
    public float fireRate;
    private float lastFireTime;
	void Awake()
	{
        // Setting up the references.
        playerCtrl = GameObject.Find("hero").GetComponent<PlayerControl>();
        playerHealth = GameObject.Find("hero").GetComponent<PlayerHealth>();
        fireRate = 0.15f;
        lastFireTime = -1;
    }


	void Update ()
	{
		// If the fire button is pressed...
        if(Input.GetButtonDown("Fire1") && playerHealth.health-2 > 0 && Time.time > lastFireTime + fireRate)
		{
            if (playerCtrl.checkInfShot() == false)
            {
                playerHealth.health -= 2;
                playerHealth.UpdateHealthBar();
                playerHealth.UpdateSprite();
            }
            GetComponent<AudioSource>().Play();

			// If the player is facing right...
			if(playerCtrl.facingRight)
			{
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(speed, 0);
			}
			else
			{
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(-speed, 0);
			}
            lastFireTime = Time.time;
		}
	}
}
