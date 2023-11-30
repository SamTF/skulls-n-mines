using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : MonoBehaviour
{
    [Header("Normal Follow")]
    [Range(0f, 5f)]
    [SerializeField]
    private float speed = 2f;
    [Range(0f, 2f)]
    [SerializeField]
    private float followDelay = 1f;
    [SerializeField]
    [Range(0f, 30f)]
    private float speedUpDelay = 3f;
    [SerializeField]
    [Range(0f, 0.2f)]
    private float speedUpStep = 1f;
    private bool speedUp = false;

    [Header("Delayed Follow Vars")]
    [SerializeField]
    private float followForce = 40f;
    [SerializeField]
    [Range(0f, 5f)]
    private float maxVel = 2f;
    [SerializeField]
    private Vector2 velocity;

    // Components
    private SpriteRenderer  spriteRenderer = null;
    private Rigidbody2D     rigidBody = null;

    private Vector3 target;             // the target that the Swarmer will follow
    private Vector3 targetRandom;       // where the Swarmer goes after the Player has died

    [SerializeField]
    private bool followPlayer = false;

    private bool isAlive = true;
    

    private void Awake()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        rigidBody       = GetComponent<Rigidbody2D>();

        targetRandom = RandomPointOnCircleEdge(3f);
        //print(targetRandom);
    }

    private void Start()
    {
        GameManager.instance.onPlayerDeath += StopFollowing;

        if(GameManager.instance.IsPlayerAlive)
        {
            target = GameManager.instance.PlayerPosition;
            StartCoroutine(FollowPlayer());
        }
        else
        {
            target = targetRandom;
        }
    }

    private void Update()
    {
        // Flipping stuff
        float angleBetween = GameManager.instance.GetAngleToPlayer(transform.position);
        if (angleBetween >= 90 || angleBetween <= -90)  spriteRenderer.flipX = true;
        else                                            spriteRenderer.flipX = false;

        // Follow stuff
        if (followPlayer)           target = GameManager.instance.PlayerPosition;
        else                        target = targetRandom;

        if(followDelay > 0)         return;

        // Moves smoothly towards the target location
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, target, step);
        //print(target);
    }

    private void FixedUpdate() {
        if(speedUp)     followForce += speedUpStep;

        followForce = Mathf.Clamp(followForce, 0f, 200f);
    }


    private Vector3 RandomPointOnCircleEdge(float radius)
    {
        var vector2 = Random.insideUnitCircle.normalized * radius;
        return vector2;
    }

    private IEnumerator FollowPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        followPlayer = true;

        //StartCoroutine(FollowMethod1());
        StartCoroutine(FollowMethod2());

        StartCoroutine(SpeedUp());
    }

    private void StopFollowing() {
        print("Stopped Following");
        followPlayer = false;
        targetRandom = RandomPointOnCircleEdge(10f);
    }

    public void Die()
    {
        if(!isAlive)    return;

        isAlive = false;

        GameManager.instance.onPlayerDeath -= StopFollowing;
        GameManager.instance.OnSwarmerDeath();
        AudioManager.instance.Play(SoundClips.CrackedSkull);
        Destroy(gameObject);
    }


    // Method 1 - using 0.1s delay
    private IEnumerator FollowMethod1()
    {
        while (followDelay > 0)
        {
            Vector2 vector = (GameManager.instance.PlayerPosition - transform.position);
            vector = vector.normalized * 5f;

            rigidBody.AddForce(vector);
            
            Vector2 v = rigidBody.velocity;
            velocity = new Vector2(Mathf.Clamp(v.x, -maxVel, maxVel), Mathf.Clamp(v.y, -maxVel, maxVel));

            rigidBody.velocity = velocity;

            yield return new WaitForSeconds(followDelay);
            // target = GameManager.instance.PlayerPosition;
        }

    }

    // Method 2 - using 0.2s delay
    private IEnumerator FollowMethod2()
    {
        while (followDelay > 0)
        {
            Vector2 vector = (target - transform.position);
            vector = vector.normalized * followForce;

            rigidBody.AddForce(vector);
            
            velocity = rigidBody.velocity;
            //velocity = new Vector2(Mathf.Clamp(v.x, -maxVel, maxVel), Mathf.Clamp(v.y, -maxVel, maxVel));

            //rigidBody.velocity = velocity;

            yield return new WaitForSeconds(followDelay);

            rigidBody.velocity = rigidBody.velocity/2;
        }
    }

    private IEnumerator SpeedUp()
    {
        yield return new WaitForSeconds(speedUpDelay);

        speedUp = true;
    }
}
