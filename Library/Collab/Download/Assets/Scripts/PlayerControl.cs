using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	public float moveForce = 360f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1300f;         // Amount of force added when the player jumps.
    public bool grounded = false;           // Whether or not the player is grounded.
    public bool canMove;

    private Transform groundCheck;			// A position marking where to check if the player is grounded.
    [HideInInspector]
    public float abilityCooldown = 4;
    public bool dashToggle = false;
    [HideInInspector]
    public float abilityStart=-15;
    private bool isJumping;
    private bool jumpKeyHeld;
    private bool stillInfShot = false;
    private bool buttonConsume;
    private Vector2 counterJumpForce;
    public GameObject camera;

    [SerializeField]
    public GameObject GameOverMenu;
    void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
        counterJumpForce = new Vector2(0f, -jumpForce/3);
        //anim = GetComponent<Animator>();
        canMove = true;
        buttonConsume = false;
    }


	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump"))
        {
            jumpKeyHeld = true;
            if (grounded)
            {
                isJumping = true;
                AudioSource.PlayClipAtPoint(jumpClips[0], transform.position);
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
            }
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpKeyHeld = false;
        }
    }


	void FixedUpdate ()
	{
        if (canMove == true)
        {
            // Cache the horizontal input.
            float h = Input.GetAxis("Horizontal");

            // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
            if (h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
                // ... add a force to the player.
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

            // If the player's horizontal velocity is greater than the maxSpeed...
            if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
                // ... set the player's velocity to the maxSpeed in the x axis.
                GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (h > 0 && !facingRight)
                // ... flip the player.
                Flip();
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (h < 0 && facingRight)
                // ... flip the player.
                Flip();

            if (isJumping)
            {
                if (!jumpKeyHeld && Vector2.Dot(GetComponent<Rigidbody2D>().velocity, Vector2.up) > 0)
                {
                    GetComponent<Rigidbody2D>().AddForce(counterJumpForce);
                }
            }
            if (stillInfShot == true)
            {
                if (Time.time > abilityStart + 2.2)
                    stillInfShot = false;
            }

            if (Input.GetButtonDown("Fire2") && Time.time > abilityStart+abilityCooldown)
            {
                abilityStart = Time.time;
                
                if (abilityCooldown == 4)
                {
                    //print("reached dash toggle\n");
                    //dash mechanic
                    dashToggle = !dashToggle;
                    if (dashToggle)
                    {                     
                        maxSpeed = 20f;
                        Invoke("dashOff", 1F);
                    }
                }
                else if (abilityCooldown == 15)
                {//infinite shot
                    stillInfShot = true;
                }
                else if (abilityCooldown == 7)
                {//Consume enemy
                    //print("eating\n");
                    buttonConsume = true;
                    Invoke("flipbConsume",0.1F);
                }
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "fallDetector")
        {
            Debug.Log("PITFALL DEATH!");
            camera.GetComponent<FollowPlayer>().enabled = false;
            GetComponent<PlayerHealth>().health = 0;
            GameOverMenu.SetActive(true);
        }
    }

    void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
    public bool checkInfShot()
    {
        return stillInfShot;
    }
    public bool checkbConsume()
    {
        return buttonConsume;
    }
    public void flipbConsume()
    {
        buttonConsume = false;
    }
    public void dashOff()
    {
        //print("dash off invoked");
        dashToggle = false;
        maxSpeed = 9f;
    }
 }
