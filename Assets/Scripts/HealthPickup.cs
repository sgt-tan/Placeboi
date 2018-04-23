using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
	public float healthBonus;				// How much health the crate gives the player.
	public AudioClip collect;				// The sound of the crate being collected.
	private bool landed;                    // Whether or not the crate has landed.
    private SpriteRenderer spriteRenderer;
    private PlayerControl playerControl;

    void Awake ()
	{
		// Setting up the references.
        playerControl = GameObject.Find("hero").GetComponent<PlayerControl>();
        GameObject spriteHolder = GameObject.Find("hero").transform.Find("body").gameObject;
        spriteRenderer = spriteHolder.GetComponent<SpriteRenderer>(); // we are accessing the SpriteRenderer that is attached to the Player
    }


	void OnTriggerEnter2D (Collider2D other)
	{
		// If the player enters the trigger zone...
		if(other.tag == "Player")
		{            
            // Get a reference to the player health script.
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (spriteRenderer.sprite == null) // if the sprite on spriteRenderer is null then
                spriteRenderer.sprite = playerHealth.sprite1; // set the sprite to sprite1
            // Increasse the player's health by the health bonus but clamp it at 100.
            addHealth(playerHealth);

			// Update the health bar.
			playerHealth.UpdateHealthBar();

			// Play the collection sound.
			AudioSource.PlayClipAtPoint(collect,transform.position);

			// Destroy the crate.
			Destroy(transform.root.gameObject);
		}
	}
    void addHealth(PlayerHealth playerHealth) {
        // Increasse the player's health by the health bonus but clamp it at 100.
        playerHealth.health += healthBonus;
        playerHealth.health = Mathf.Clamp(playerHealth.health, 0f, 300f);
        BoxCollider2D playerSize = playerHealth.gameObject.GetComponent<BoxCollider2D>();
        if (playerHealth.health > 100 && playerHealth.health <= 200)
        {
            playerSize.size = new Vector2(2F, 1.9F);
            playerSize.offset = new Vector2(0, 0);
            playerControl.abilityCooldown = 15;
            playerControl.abilityStart = -15;
            playerHealth.gameObject.transform.GetChild(1).localPosition = new Vector3(0, -1.5F, 0);
            GameObject.Find("Gun").transform.localPosition = new Vector3(0.8F, 0, 0);
            spriteRenderer.sprite = playerHealth.sprite2;
            GameObject.Find("hero").GetComponent<PlayerControl>().jumpForce = 1400f;
            GameObject.Find("hero").GetComponent<PlayerControl>().moveForce = 425f;
            GameObject.Find("hero").GetComponent<PlayerControl>().maxSpeed = 7f;
            GameObject.Find("AbilityIndicator").GetComponent<BlinkEffect>().startBlinking = false;
            GameObject.Find("AbilityIndicator").GetComponent<SpriteRenderer>().enabled = true;

        }
        else if (playerHealth.health > 200)
        {
            playerSize.size = new Vector2(3F, 2.7F);
            playerSize.offset = new Vector2(0, -0.4F);
            playerControl.abilityCooldown = 7;
            playerControl.abilityStart = -15;
            playerHealth.gameObject.transform.GetChild(1).localPosition = new Vector3(0, -2.2F, 0);
            GameObject.Find("Gun").transform.localPosition = new Vector3(1.5F, -0.7F, 0);
            spriteRenderer.sprite = playerHealth.sprite3;
            GameObject.Find("hero").GetComponent<PlayerControl>().jumpForce = 1300f;
            GameObject.Find("hero").GetComponent<PlayerControl>().moveForce = 365f;
            GameObject.Find("hero").GetComponent<PlayerControl>().maxSpeed = 6f;
            GameObject.Find("AbilityIndicator").GetComponent<BlinkEffect>().startBlinking = false;
            GameObject.Find("AbilityIndicator").GetComponent<SpriteRenderer>().enabled = true;
        }
    }

}
