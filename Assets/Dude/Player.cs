using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    #pragma warning disable 0414

    // Movement Vars
    [SerializeField]
    [Header("Stats")]
    private float movementSpeed = 4f;
    private float movementX     = 0f;
    private float movementY     = 0f;

    // slower start test
    [SerializeField]
    private bool usingNewMovement = true;
    private float movementAcceleration = 0.15f;
    private float startTime = 0f;
    private float s = 0f;
    private bool isMoving = false;
    private float timeSinceLastKeyPress = 10f;

    [SerializeField]
    [Header("Mines")]
    private GameObject mine = null;
    [SerializeField]
    private Transform mineHolder = null;

    private int minesPlaced = 0;

    // Components
    private Rigidbody2D     rb              = null;
    private SpriteRenderer  spriteRenderer  = null;
    private Animator        animator        = null;
   

    // Input
    [Header("Attributes")]
    [SerializeField]
    private bool    usingController = false;
    private string inputHorizontal  = "Horizontal";
    private string inputVertical    = "Vertical";
    private string inputFire1       = "Fire1";
    private string inputFire2       = "Fire2";
    private string inputJump        = "Jump";

    // Statuses
    private bool isAlive        = true;
    private bool isFacingRight  = true;
    private bool isKnockedBack  = false;
    private bool isGrounded     = true;

    /// Events
    public event Action onMineDetonation;

    /// Singleton thing
    private static Player _instance;
    public  static Player instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)  _instance = this;
        else                    Destroy(gameObject);

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (usingController)
        {
            inputHorizontal += " Controller";
            inputVertical += " Controller";
            inputFire1 += " Controller";
            inputFire2 += " Controller";
            inputJump += " Controller";
        }        
    }

    private void Start() {
        GameManager.instance.onPlayerDeath += Die;
    }

    /// Just detects input
    private void Update()
    {
        if(!isAlive)    return;

        // Movement Input
        movementX = Input.GetAxisRaw(inputHorizontal);
        movementY = Input.GetAxisRaw(inputVertical);

        if(Input.GetButtonDown(inputFire1))
        {
            Instantiate(mine, transform.position, Quaternion.identity, mineHolder);
            minesPlaced++;
        }
        if(Input.GetButtonDown(inputFire2) && minesPlaced > 0)
        {
            onMineDetonation();
        }

        ////////////////////
        if(!usingNewMovement)   return;

        // Slower start test
        if(movementX == 0 && movementY == 0)
        {
            s = 0;
            isMoving = false;
            timeSinceLastKeyPress += Time.deltaTime;
        }

        if((movementX != 0 || movementY != 0) && !isMoving && timeSinceLastKeyPress > 0.1)
        {
            startTime = Time.time;
            isMoving = true;
            //print("IF :" + (timeSinceLastKeyPress > 0.2));

            print(timeSinceLastKeyPress);
        }

        if(movementX != 0 || movementY != 0)
        {
            //print(">>> MOVING");
            float t = (Time.time - startTime) / movementAcceleration;

            //s = Mathf.SmoothStep(0f, movementSpeed, t);
            s = Mathf.SmoothStep(movementSpeed / 2f, movementSpeed, t);

            //print(s);

            timeSinceLastKeyPress = 0;
        }      
    }

    private void FixedUpdate()
    {
        if (!isKnockedBack && isGrounded)
        {
            Vector2 playerVelocity = new Vector2(movementX, movementY);
            /// new movement test
            float speed = usingNewMovement ? s : movementSpeed;
            rb.velocity = playerVelocity.normalized * speed;
            //rb.velocity = playerVelocity.normalized * movementSpeed;

            if(movementX > 0 && !isFacingRight)     Flip();
            if(movementX < 0 && isFacingRight)      Flip();
        }
        
        animator.SetFloat("MovementSpeed", rb.velocity.magnitude);
    }

    private void Flip()
    {
        // Flips the Player
        Vector3 rotation = transform.localEulerAngles;
        rotation.y += 180f;
        transform.localEulerAngles = rotation;

        isFacingRight = !isFacingRight;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Ground") && isGrounded) {
            isGrounded = false;
            StartCoroutine(Fall("ground"));

            //if (isAlive) AudioManager.instance.Play("Fall");
        }

        // print(other.tag);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Hole") && isGrounded)
        {
            // intersection test
            CircleCollider2D myCollider = GetComponent<CircleCollider2D>();
            if(myCollider.bounds.Intersects(other.bounds))
            {
                print("BOUNDS THING");
                print(myCollider.bounds.ToString());

                isGrounded = false;
                Transform hole = other.GetComponent<Transform>();
                StartCoroutine(Fall("hole", hole));
            }

            //if(other.bounds.Contains(myCollider.bounds.extents))
        }

        else if (other.CompareTag("Enemy") && isAlive && isGrounded)
        {
            Vector2 collision = (transform.position - other.transform.position).normalized * 300f;
            Knockback(collision);

            KindaDie();
        }

        else if (other.CompareTag("Explosion") && isAlive && isGrounded)
        {
            Vector2 collision = (transform.position - other.transform.position).normalized * 300f;
            Knockback(collision);

            KindaDie();
        }
    }

    private void KindaDie()
    {
        // not the actuak death function cause we don't want the slomo or destruction yet
        AudioManager.instance.Play(SoundClips.PlayerDeath);
        animator.SetTrigger("Death");
        isAlive = false;
        //GameManager.instance.OnPlayerDeath();
        // Die();
    }



    private IEnumerator Fall(string FallType, Transform hole = null)
    {
        print("FALLING");
        while (transform.localScale.x >= 0.1)
        {
            Vector3 scale = transform.localScale;

            transform.localScale = new Vector3(scale.x - 0.05f, scale.y - 0.05f, 1);
            transform.Rotate(new Vector3(0, 0, 10));

            if(FallType == "hole")
            {
                // Makes the Player fall towards the center of the hole
                float step = 1f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, hole.transform.position, step);
                movementX = 0f;
                movementY = 0f;
                print(hole.transform.position);
            }
            else
            {
                rb.gravityScale = 1f;
                movementX = movementX / 2f;
            }
            

            yield return new WaitForSeconds(0.02f);
        }

        Vector2 playerVelocity = Vector3.zero;

        if (transform.localScale.x <= 0.1)
        {
            GameManager.instance.OnPlayerDeath();
        }
    }

    public void Die()
    {
        if(!isAlive)    return;
        AudioManager.instance.Play(SoundClips.PlayerDeath);
        isAlive = false;

        // AudioManager.instance.Play("Death");
        CameraController.instance.TriggerShake(0.3f, 0.3f);
        // GameManager.instance.OnPlayerDeath();
        
        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        Time.timeScale = 0.3f; // Slows the game down
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    private void Knockback(Vector2 knockback)
    {
        print("knockback: " + knockback);
        isKnockedBack = true;
        rb.AddForce(knockback);
    }


    public bool IsAlive => isAlive;

}
