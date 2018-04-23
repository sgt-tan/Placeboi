using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class consumeCheck : MonoBehaviour {
    PlayerControl playerControl;
    PlayerHealth playerHealth;
    void Start () {
        playerControl = GameObject.Find("hero").GetComponent<PlayerControl>();
        playerHealth = GameObject.Find("hero").GetComponent<PlayerHealth>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerStay2D(Collider2D col)
    {
        if (playerControl.checkbConsume() == true) {
            if (col.tag == "Enemy")
            {
                Destroy(col.gameObject);
                addHealth(col.GetComponent<Enemy>().maxHp*8);
                // Update the health bar.
                playerHealth.UpdateHealthBar();
            }
            else if (col.tag == "Bossman")
            {
                col.GetComponent<BossHealth>().HP -= 10;
                addHealth(50);
                playerHealth.UpdateHealthBar();
            }
        }

    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (playerControl.checkbConsume() == true)
        {
            if (col.tag == "Enemy")
            {
                Destroy(col.gameObject);
                addHealth(col.GetComponent<Enemy>().maxHp*8);
                // Update the health bar.
                playerHealth.UpdateHealthBar();
            }
            else if (col.tag == "Bossman")
            {
                col.GetComponent<BossHealth>().HP -= 10;
                addHealth(50);
                playerHealth.UpdateHealthBar();
            }
        }

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (playerControl.checkbConsume() == true)
        {
            if (col.tag == "Enemy")
            {
                Destroy(col.gameObject);
                addHealth(col.GetComponent<Enemy>().maxHp*8);
                // Update the health bar.
                playerHealth.UpdateHealthBar();
            }
            else if (col.tag == "Bossman")
            {
                col.GetComponent<BossHealth>().HP -= 10;
                addHealth(50);
                playerHealth.UpdateHealthBar();
            }
        }

    }
    void addHealth(int enemyHP)
    {
        // Increasse the player's health by the health bonus but clamp it at 100.
        playerHealth.health += enemyHP*5;
        playerHealth.health = Mathf.Clamp(playerHealth.health, 0f, 300f);
        BoxCollider2D playerSize = playerHealth.gameObject.GetComponent<BoxCollider2D>();
        if (playerHealth.health > 100 && playerHealth.health <= 200)
        {
            playerSize.size = new Vector2(2F, 1.9F);
            playerSize.offset = new Vector2(0, 0);
            playerControl.abilityCooldown = 15;
            playerHealth.gameObject.transform.GetChild(1).localPosition = new Vector3(0, -1.5F, 0);
            playerHealth.spriteRenderer.sprite = playerHealth.sprite2;
            GameObject.Find("hero").GetComponent<PlayerControl>().jumpForce = 1200f;
            GameObject.Find("hero").GetComponent<PlayerControl>().moveForce = 425f;
            GameObject.Find("hero").GetComponent<PlayerControl>().maxSpeed = 7f;

        }
        else if (playerHealth.health > 200)
        {
            playerSize.size = new Vector2(3F, 2.7F);
            playerSize.offset = new Vector2(0, -0.4F);
            playerControl.abilityCooldown = 7;
            playerHealth.gameObject.transform.GetChild(1).localPosition = new Vector3(0, -2.3F, 0);
            playerHealth.spriteRenderer.sprite = playerHealth.sprite3;
            GameObject.Find("hero").GetComponent<PlayerControl>().jumpForce = 1100f;
            GameObject.Find("hero").GetComponent<PlayerControl>().moveForce = 365f;
            GameObject.Find("hero").GetComponent<PlayerControl>().maxSpeed = 6f;
        }
    }
}
