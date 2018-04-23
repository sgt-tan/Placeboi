using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;		// Prefab of explosion effect.


	void Start () 
	{
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 2);
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Enemy")
		{
            // ... find the Enemy script and call the Hurt function.
            if (col.GetComponent<Enemy>().enemyType == 5)
            {
                if (col.GetComponent<Enemy>().type5NoDMG == false)
                    col.GetComponent<Enemy>().Hurt();
            }
            else
                col.GetComponent<Enemy>().Hurt();

            // Destroy the rocket.
            Destroy (gameObject);
		}
        else if(col.tag == "Bossman")
        {
            col.GetComponent<BossHealth>().Hurt();
            Destroy(gameObject);
        }
		// Otherwise if the player manages to shoot himself...
		else if(col.gameObject.tag != "Player" && col.gameObject.tag != "MainCamera" && col.gameObject.tag != "Spawner" && col.gameObject.tag != "EnemyShot" && col.gameObject.tag != "consumeCheck" && col.gameObject.tag != "Crate")
		{
			// Instantiate the explosion and destroy the rocket.
			Destroy (gameObject);
		}
	}
}
