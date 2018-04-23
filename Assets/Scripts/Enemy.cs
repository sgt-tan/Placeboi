using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public float moveSpeed = 2f;		// The speed the enemy moves at.
	public int HP = 2;					// How many times the enemy can be hit before it dies.
	public Sprite deadEnemy;			// A sprite of the enemy when it's dead.
	public Sprite damagedEnemy;			// An optional sprite of the enemy when it's damaged.
	public AudioClip[] deathClips;		// An array of audioclips that can play when the enemy dies.
	public float deathSpinMin = -100f;			// A value to give the minimum amount of Torque when dying
	public float deathSpinMax = 100f;			// A value to give the maximum amount of Torque when dying
    public bool hasGun = false;
    public float shootingRate = 0.5f;
    public Rigidbody2D shotPrefab;
    public float enemyType = 1; //different enemy types 1=run at you, 2 =single shot, 3= triple shot, 4=fly at you, 5=shield/shoot
    private Transform player;
    public Rigidbody2D slimePickup;
    public int maxHp;
    public bool type5NoDMG;
    private SpriteRenderer ren;			// Reference to the sprite renderer.
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.
	private bool dead = false;			// Whether or not the enemy is dead.
    private float lastShotTime;
    private SpriteRenderer type5Shield;
    void Awake()
	{
        // Setting up the references.
        ren = transform.Find("body").GetComponent<SpriteRenderer>();
        maxHp = HP;
        if (enemyType == 5) {
            type5Shield = transform.Find("body").GetComponent<SpriteRenderer>();
}
        frontCheck = transform.Find("frontCheck").transform;
        lastShotTime = Time.time-0.5F;
        player = GameObject.Find("hero").transform;
        type5NoDMG = true;
        Physics.IgnoreLayerCollision(10, 10);
    }
    private void Update()
    {
        if (Time.time > lastShotTime + shootingRate && hasGun == true)
        {
            if (enemyType == 5)
            {
                type5DMGSwitch();
                type5Switch();
                Invoke("Attack",0.5F);
                Invoke("type5Switch", 1F);
                Invoke("type5DMGSwitch", 1F);
            }
            else
                Attack();
            lastShotTime = Time.time;
        }
    }
    void FixedUpdate ()
	{
		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

		// Check each of the colliders.
		foreach(Collider2D c in frontHits)
		{
            // If any of the colliders is an Obstacle...
            if (c.tag == "Obstacle" || c.tag == "Hazard")
			{
                // ... Flip the enemy and stop checking the other colliders.
				Flip ();
				break;
			}
		}
        if (enemyType == 4)
        {
            transform.LookAt(player.position);
            transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation


            //move towards the player
            if (Vector3.Distance(transform.position, player.position) > 1f)
            {//move if distance from target is greater than 1
                transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
            }
        }
        else // Set the enemy's velocity to moveSpeed in the x direction.
            GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * moveSpeed, GetComponent<Rigidbody2D>().velocity.y);	

		// If the enemy has one hit point left and has a damagedEnemy sprite...
		if(HP == 1 && damagedEnemy != null)
			// ... set the sprite renderer's sprite to be the damagedEnemy sprite.
			ren.sprite = damagedEnemy;
			
		// If the enemy has zero or fewer hit points and isn't dead yet...
		if(HP <= 0 && !dead)
			// ... call the death function.
			Death ();
	}
	
	public void Hurt()
	{
		// Reduce the number of hit points by one.
		HP--;
	}
	
	public void Death()
	{
		// Find all of the sprite renderers on this object and it's children.
		SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

		// Disable all of them sprite renderers.
		foreach(SpriteRenderer s in otherRenderers)
		{
			s.enabled = false;
		}

		// Re-enable the main sprite renderer and set it's sprite to the deadEnemy sprite.
		ren.enabled = true;
		ren.sprite = deadEnemy;

		// Set dead to true.
		dead = true;

		// Allow the enemy to rotate and spin it by adding a torque.
		GetComponent<Rigidbody2D>().AddTorque(Random.Range(deathSpinMin,deathSpinMax));

		// Find all of the colliders on the gameobject and set them all to be triggers.
		Collider2D[] cols = GetComponents<Collider2D>();
		foreach(Collider2D c in cols)
		{
			c.isTrigger = true;
		}

		// Play a random audioclip from the deathClips array.
		int i = Random.Range(0, deathClips.Length);
		AudioSource.PlayClipAtPoint(deathClips[i], transform.position);
        int slimeAmount = maxHp;
        for (int s = 0; s <= slimeAmount; s++)
        {
            Rigidbody2D slime = Instantiate(slimePickup, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
            int j = Random.Range(-1,1);
            slime.velocity = new Vector2(j, 0);
        }
        Destroy(gameObject, 0.5F);
	}


	public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

    public void Attack()
    {
        if (enemyType == 2 || enemyType == 5)
        {
            Rigidbody2D bulletInstance = Instantiate(shotPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
            if (gameObject.transform.localScale.x < 0)
                bulletInstance.velocity = new Vector2(-10, 0);
            else
                bulletInstance.velocity = new Vector2(10, 0);
        }
        else if(enemyType == 3)
        {
            if (gameObject.transform.localScale.x < 0)
            {
                Rigidbody2D bulletInstance = Instantiate(shotPrefab, transform.position + new Vector3(-0.5F, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance1 = Instantiate(shotPrefab, transform.position + new Vector3(-0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance2 = Instantiate(shotPrefab, transform.position - new Vector3(0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(-10, 0);
                bulletInstance1.velocity = new Vector2(-10, 3);
                bulletInstance2.velocity = new Vector2(-10,-3);
            }
            else
            {
                Rigidbody2D bulletInstance = Instantiate(shotPrefab, transform.position + new Vector3(0.5F, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance1 = Instantiate(shotPrefab, transform.position + new Vector3(0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance2 = Instantiate(shotPrefab, transform.position - new Vector3(-0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(10, 0);
                bulletInstance1.velocity = new Vector2(10, 3);
                bulletInstance2.velocity = new Vector2(10,-3);
            }
        }
    }
    public void type5Switch()
    {
        type5Shield.enabled = !type5Shield.enabled;
    }
    public void type5DMGSwitch()
    {
        type5NoDMG = !type5NoDMG;
    }
}
