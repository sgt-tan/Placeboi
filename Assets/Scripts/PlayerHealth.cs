using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{	
	public float health = 100f;					// The player's health.
	public float repeatDamagePeriod = 2f;		// How frequently the player can be damaged.
	public AudioClip[] ouchClips;				// Array of clips to play when the player is damaged.
	public float hurtForce = 10f;				// The force with which the player is pushed when hurt.
	public float damageAmount = 10f;            // The amount of damage to take when enemies touch the player
    public SpriteRenderer spriteRenderer;

    private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
	public float lastHitTime;					// The time at which the player was last hit.
	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).
	private PlayerControl playerControl;        // Reference to the PlayerControl script.
    public Sprite sprite1; // Drag your first sprite here, Collision size = 1.3, 1.1, offset = 0, -0.1, groundcheck y= -1
    public Sprite sprite2; // Drag your second sprite here, Collision size = 2, 1.9, offset = 0, 0, groundcheck y= -1.5
    public Sprite sprite3; // Drag your third sprite here, Collision size = 3, 2.7, offset = 0, -0.4, groundcheck y= -2.3
    private BlinkEffect blink;


    [SerializeField]
    public GameObject GameOverMenu;

    void Awake ()
	{
		// Setting up references.
		playerControl = GetComponent<PlayerControl>();
        blink = GetComponent<BlinkEffect>();
		healthBar = GameObject.Find("HealthBar").GetComponent<SpriteRenderer>();
        //anim = GetComponent<Animator>();
        GameObject spriteHolder = gameObject.transform.Find("body").gameObject;
        spriteRenderer = spriteHolder.GetComponent<SpriteRenderer>();
        // Getting the intial scale of the healthbar (whilst the player has full health).
        healthScale = healthBar.transform.localScale;
    }


    public void OnCollisionEnter2D (Collision2D col)
	{
        // If the colliding gameobject is an Enemy...
        if ((col.gameObject.tag == "Enemy" || col.gameObject.tag == "Hazard" || col.gameObject.tag == "EnemyShot" || col.gameObject.tag == "Bossman") && !playerControl.dashToggle)
        {
            //print("collision\n");
            // ... and if the time exceeds the time of the last hit plus the time between hits...
            if (Time.time > lastHitTime + repeatDamagePeriod)
            {
                // ... and if the player still has health...
                if (health - damageAmount > 0f)
                {
                    // ... take damage and reset the lastHitTime.
                    playerControl.canMove = false;
                    TakeDamage(col.transform);
                    UpdateSprite();

                    lastHitTime = Time.time;
                    playerControl.canMove = true;
                }
                // If the player doesn't have health, do some stuff, let him fall into the river to reload the level.
                else
                {
                    health -= damageAmount;
                    UpdateHealthBar();
                    // Move all sprite parts of the player to the front
                    SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer s in spr)
                    {
                        s.sortingLayerName = "UI";
                    }

                    // ... disable user Player Control script
                    GetComponent<PlayerControl>().enabled = false;

                    // ... disable the Gun script to stop a dead guy shooting a nonexistant bazooka
                    GetComponentInChildren<Gun>().enabled = false;

                    GameOverMenu.SetActive(true);

                }
            }
        }
        else if ((col.gameObject.tag == "Enemy") && playerControl.dashToggle)
        {
            col.gameObject.GetComponent<Enemy>().Hurt();
        }
        else if ((col.gameObject.tag == "Bossman") && playerControl.dashToggle)
            col.gameObject.GetComponent<BossHealth>().Hurt();
    }

    void OnCollisionStay2D(Collision2D col) {
        OnCollisionEnter2D(col);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        {
            // If the colliding gameobject is an EnemyShot...
            if (col.gameObject.tag == "EnemyShot" && !playerControl.dashToggle)
            {
                // ... and if the time exceeds the time of the last hit plus the time between hits...
                if (Time.time > lastHitTime + repeatDamagePeriod)
                {
                    // ... and if the player still has health...
                    if (health - damageAmount > 0f)
                    {
                        // ... take damage and reset the lastHitTime.
                        playerControl.canMove = false;
                        TakeDamage(col.transform);
                        UpdateSprite();

                        lastHitTime = Time.time;
                        playerControl.canMove = true;
                    }
                    // If the player doesn't have health, do some stuff, let him fall into the river to reload the level.
                    else
                    {
                        health -= damageAmount;
                        UpdateHealthBar();
                        // Move all sprite parts of the player to the front
                        SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer s in spr)
                        {
                            s.sortingLayerName = "UI";
                        }

                        // ... disable user Player Control script
                        GetComponent<PlayerControl>().enabled = false;

                        // ... disable the Gun script to stop a dead guy shooting a nonexistant bazooka
                        GetComponentInChildren<Gun>().enabled = false;

                        GameOverMenu.SetActive(true);

                    }
                }
            }
        }
    }

    public void TakeDamage (Transform enemy)
	{
            // Make sure the player can't jump.
            playerControl.jump = false;
            Vector3 hurtVector;
            // Create a vector that's from the enemy to the player with an upwards boost.
            if (enemy.tag == "Hazard")
                if (GetComponent<Rigidbody2D>().velocity.x >= 0)
                    hurtVector = new Vector3(-1.5F, 5, 0);
                else
                    hurtVector = new Vector3(1.5F, 5, 0);
            else
                hurtVector = gameObject.transform.position - enemy.position + Vector3.up * 5f;
            // Add a force to the player in the direction of the vector and multiply by the hurtForce.
            GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);

            // Reduce the player's health
            if(playerControl.abilityCooldown == 7)
                health -= damageAmount/2;
            else
                health -= damageAmount;
            blink.startBlinking = true;

            // Update what the health bar looks like.
            UpdateHealthBar();

            // Play a random clip of the player getting hurt.
            int i = Random.Range(0, ouchClips.Length);
            AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);
	}

    public void UpdateHealthBar ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);

        // Set the scale of the health bar to be proportional to the player's health.
        if (health <= 0)
        {
            healthBar.transform.localScale = new Vector3(healthScale.x * 0 * 0.01f, 1, 1);
        }
        else
        {
            healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1, 1);
        }
	}
    public void UpdateSprite()
    {
        BoxCollider2D playerSize = gameObject.GetComponent<BoxCollider2D>();
        if (health <= 100)
        {
            playerSize.size = new Vector2(1.3F, 1.1F);
            playerSize.offset = new Vector2(0, -0.1F);
            gameObject.transform.GetChild(1).localPosition = new Vector3(0, -1, 0);
            playerControl.abilityCooldown = 4;
            playerControl.abilityStart = -15;
            spriteRenderer.sprite = sprite1;
            GameObject.Find("Gun").transform.localPosition = new Vector3(0.8F, 0, 0);
            GameObject.Find("hero").GetComponent<PlayerControl>().jumpForce = 1500f;
            GameObject.Find("hero").GetComponent<PlayerControl>().moveForce = 500f;
            GameObject.Find("hero").GetComponent<PlayerControl>().maxSpeed = 9f;
            GameObject.Find("AbilityIndicator").GetComponent<BlinkEffect>().startBlinking = false;
            GameObject.Find("AbilityIndicator").GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (health > 100 && health <= 200)
        {
            playerSize.size = new Vector2(2F, 1.9F);
            playerSize.offset = new Vector2(0, 0);
            gameObject.transform.GetChild(1).localPosition = new Vector3(0, -1.5F, 0);
            playerControl.abilityCooldown = 15;
            playerControl.abilityStart = -15;
            spriteRenderer.sprite = sprite2;
            GameObject.Find("Gun").transform.localPosition = new Vector3(0.8F, 0, 0);
            GameObject.Find("hero").GetComponent<PlayerControl>().jumpForce = 1400f;
            GameObject.Find("hero").GetComponent<PlayerControl>().moveForce = 425f;
            GameObject.Find("hero").GetComponent<PlayerControl>().maxSpeed = 7f;
            GameObject.Find("AbilityIndicator").GetComponent<BlinkEffect>().startBlinking = false;
            GameObject.Find("AbilityIndicator").GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
