using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour {

    //public int enemyHealth;

    //public GameObject deathEffect;
    public GameObject bossPrefab;
    public float minSize;

    public GameObject player = null;
    public float triggerDistance = 10.0f;
    public float smoothTime = 5.0f;
    private Vector3 smoothVelocity = Vector3.zero;

    //===============================================
    public float moveSpeed = 2f;        // The speed the enemy moves at.
    public int HP = 10;                  // How many times the enemy can be hit before it dies.
    public Sprite deadEnemy;            // A sprite of the enemy when it's dead.
    public Sprite damagedEnemy;         // An optional sprite of the enemy when it's damaged.
    public AudioClip[] deathClips;      // An array of audioclips that can play when the enemy dies.
    public GameObject hundredPointsUI;  // A prefab of 100 that appears when the enemy dies.
    public float deathSpinMin = -100f;          // A value to give the minimum amount of Torque when dying
    public float deathSpinMax = 100f;			// A value to give the maximum amount of Torque when dying
    public bool hasGun = false;
    public float shootingRate = 0.5f;
    public Rigidbody2D shotPrefab;
    public float enemyType = 2; //different enemy types 1=run at you, 2 =single shot, 3= triple shot, 4=fly at player, 5=shield and shoot
    public Rigidbody2D slimePickup;

    private SpriteRenderer ren;         // Reference to the sprite renderer.
    private Transform frontCheck;       // Reference to the position of the gameobject used for checking if something is in front.
    private bool dead = false;			// Whether or not the enemy is dead.
    private float lastShotTime;
    private int splitCount = 0;
    /*private Transform groundCheck;  //groundcheck for boss jump
    public bool grounded = false;
    public float jumpForce = 1100f;
    public bool jump = false;*/
    //===============================================
    private int maxHp;
    void Awake()
    {
        //Setting up the references.
        ren = transform.Find("body").GetComponent<SpriteRenderer>();
        maxHp = HP;
        frontCheck = transform.Find("frontCheck").transform;
        lastShotTime = Time.time;
        player = GameObject.Find("hero");
        /*groundCheck = transform.Find("groundCheck");*/
    }

    // Update is called once per frame
    void Update () {
        if (Time.time > lastShotTime + shootingRate && hasGun == true)
        {
            Attack();
            lastShotTime = Time.time;
            /*grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
            float jumpRandom = UnityEngine.Random.Range(0, 2);
            if (jumpRandom < 1 && grounded)
                jump = true;*/
        }
        if(player == null)
            player = GameObject.Find("hero");
    }

    void FixedUpdate()
    {
        // Create an array of all the colliders in front of the enemy.
        Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

        // Check each of the colliders.
        foreach (Collider2D c in frontHits)
        {
            // If any of the colliders is an Obstacle...
            if (c.tag == "Obstacle")
            {
                // ... Flip the enemy and stop checking the other colliders.
                Flip();
                break;
            }
        }

        // Set the enemy's velocity to moveSpeed in the x direction.
        GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * moveSpeed, GetComponent<Rigidbody2D>().velocity.y);


        float distance = Vector3.Distance(transform.position, player.transform.position);
        //float flipDist = transform.position.x - player.transform.position.x;

        
        //Debug.Log(flipDist);
        //Debug.Log(transform.localScale.x);
        if ((player.transform.position.x > transform.position.x) && transform.localScale.x < 0)
        {
            Flip();
        }
        if ((player.transform.position.x < transform.position.x) && transform.localScale.x > 0)
        {
            Flip();
        }
        
        if (distance < triggerDistance)
        {
            transform.position = Vector3.SmoothDamp(transform.position, player.transform.position, ref smoothVelocity, smoothTime);
        }


        //Jumping stuff
        /*if (jump)
        {
            // Play a random jump audio clip.
            //int i = Random.Range(0, jumpClips.Length);
            //AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

            // Add a vertical force to the player.
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
            //GetComponent<Rigidbody2D>().gravityScale -= (.1f * Time.fixedDeltaTime);

            // Make sure the player can't jump again until the jump conditions from Update are satisfied.
            jump = false;
        }*/

        // If the enemy has one hit point left and has a damagedEnemy sprite...
        if (HP == 1 && damagedEnemy != null)
            // ... set the sprite renderer's sprite to be the damagedEnemy sprite.
            ren.sprite = damagedEnemy;

        // If the enemy has zero or fewer hit points and isn't dead yet...
        if (HP <= 0 && !dead)
        {
            if (transform.localScale.y > minSize)
            //if (splitCount < 3)
            {
                GameObject clone1 = Instantiate(bossPrefab, new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z), transform.rotation) as GameObject;
                GameObject clone2 = Instantiate(bossPrefab, new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z), transform.rotation) as GameObject;

                clone1.transform.localScale = new Vector3(transform.localScale.y * 0.5f, transform.localScale.y * 0.5f, transform.localScale.z);
                clone2.transform.localScale = new Vector3(transform.localScale.y * 0.5f, transform.localScale.y * 0.5f, transform.localScale.z);
                clone1.GetComponent<BossHealth>().splitCount = 1;
                clone2.GetComponent<BossHealth>().splitCount = 1;
                splitCount++;
                if (splitCount == 1)
                {
                    maxHp = 6;
                    clone1.GetComponent<BossHealth>().HP = 6;
                    clone2.GetComponent<BossHealth>().HP = 6;
                }else if(splitCount == 2){
                    maxHp = 4;
                    clone1.GetComponent<BossHealth>().HP = 4;
                    clone2.GetComponent<BossHealth>().HP = 4;
                }
                int slimeAmount = maxHp;
                for (int s = 0; s <= slimeAmount; s++)
                {
                    Rigidbody2D slime = Instantiate(slimePickup, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                    int j = Random.Range(-1, 1);
                    slime.velocity = new Vector2(j, 0);
                }

            }
            Death();
        }
    }

    public void Hurt()
    {
        // Reduce the number of hit points by one.
        HP--;
    }

    void Death()
    {
        // Find all of the sprite renderers on this object and it's children.
        SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Disable all of them sprite renderers.
        foreach (SpriteRenderer s in otherRenderers)
        {
            s.enabled = false;
        }

        // Re-enable the main sprite renderer and set it's sprite to the deadEnemy sprite.
        ren.enabled = true;
        ren.sprite = deadEnemy;

        // Set dead to true.
        dead = true;

        // Allow the enemy to rotate and spin it by adding a torque.
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(deathSpinMin, deathSpinMax));

        // Find all of the colliders on the gameobject and set them all to be triggers.
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (Collider2D c in cols)
        {
            c.isTrigger = true;
        }

        // Play a random audioclip from the deathClips array.
        //int i = Random.Range(0, deathClips.Length);
        //AudioSource.PlayClipAtPoint(deathClips[i], transform.position);
        Destroy(gameObject, 1.5F);
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
        if (enemyType == 2)
        {
            Rigidbody2D bulletInstance = Instantiate(shotPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
            if (gameObject.transform.localScale.x < 0)
                bulletInstance.velocity = new Vector2(-10, 0);
            else
                bulletInstance.velocity = new Vector2(10, 0);
        }
        else if (enemyType == 3)
        {
            if (gameObject.transform.localScale.x < 0)
            {
                Rigidbody2D bulletInstance = Instantiate(shotPrefab, transform.position + new Vector3(-0.5F, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance1 = Instantiate(shotPrefab, transform.position + new Vector3(-0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance2 = Instantiate(shotPrefab, transform.position - new Vector3(0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(-10, 0);
                bulletInstance1.velocity = new Vector2(-10, 3);
                bulletInstance2.velocity = new Vector2(-10, -3);
            }
            else
            {
                Rigidbody2D bulletInstance = Instantiate(shotPrefab, transform.position + new Vector3(0.5F, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance1 = Instantiate(shotPrefab, transform.position + new Vector3(0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                Rigidbody2D bulletInstance2 = Instantiate(shotPrefab, transform.position - new Vector3(-0.5F, 0.5F, 0), Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(10, 0);
                bulletInstance1.velocity = new Vector2(10, 3);
                bulletInstance2.velocity = new Vector2(10, -3);
            }
        }
    }
}
