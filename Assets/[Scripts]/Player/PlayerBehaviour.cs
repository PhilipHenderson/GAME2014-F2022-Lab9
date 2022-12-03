using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement Properties")]
    public float horizontalForce;
    public float horizontalSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint; // origion of circle
    public float groundRadius; // size of circle
    public LayerMask groundLayerMask; // the stuff we can collide with
    public bool isGrounded;

    [Header("Animations")]
    public Animator animator;
    public PlayerAnimationState playerAnimationState;

    [Header("Health System")]
    public HealthBarController health;
    public LifeCounterScript life;
    public DeathPlaneController deathPlane;

    [Header("Controls")]
    public Joystick leftStick;
    [Range(0.1f, 1.0f)]
    public float verticalThreshhold;

    private Rigidbody2D rb2D;
    private SoundManager soundManager;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = FindObjectOfType<PlayerHealth>().GetComponent<HealthBarController>();
        life = FindObjectOfType<LifeCounterScript>();
        deathPlane = FindObjectOfType<DeathPlaneController>();
        soundManager = FindObjectOfType<SoundManager>();
        leftStick = (Application.isMobilePlatform) ?  GameObject.Find("Left Stick").GetComponent<Joystick>() : null;
    }

    void Update()
    {
        if (health.value <= 0)
        {
            life.LoseLife();

            if (life.value > 0)
            {
                health.ResetHealth();
                deathPlane.ReSpawn(gameObject);
                soundManager.PlaySoundFX(Sound.DEATH, Channel.PLAYER_DEATH_FX);
            }
        }

        if (life.value <= 0)
        {
            SceneManager.LoadScene("End");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var hit = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
        isGrounded = hit;

        Move();
        Jump();
        AirCheck();
    }

    private void Move()
    {
        var x = Input.GetAxisRaw("Horizontal") + ((Application.isMobilePlatform) ? leftStick.Horizontal: 0.0f);
        if (x != 0.0f)
        {
            Flip(x);
            x = (x > 0) ? 1.0f : -1.0f; // sanitizing x

            rb2D.AddForce(Vector2.right * x * horizontalForce * ((isGrounded) ? 1.0f : airFactor));

            //rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, horizontalSpeed);

            var clampedVelocity = Mathf.Clamp(rb2D.velocity.x, -horizontalSpeed, horizontalSpeed);

            rb2D.velocity = new Vector2(clampedVelocity, rb2D.velocity.y);

            ChangeAnimation(PlayerAnimationState.RUN);
        }

        if ((isGrounded) && (x == 0.0f))
        {
            ChangeAnimation(PlayerAnimationState.IDLE);
        }
    }

    private void Jump()
    {
        var y = Input.GetAxis("Jump") + ((Application.isMobilePlatform) ? leftStick.Vertical : 0.0f);

        if ((isGrounded) && (y > verticalThreshhold))
        {
            rb2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
            soundManager.PlaySoundFX(Sound.JUMP, Channel.PLAYER_SOUND_FX);
        }
    }

    private void AirCheck()
    {
        if (!isGrounded)
        {
            ChangeAnimation(PlayerAnimationState.JUMP);
        }
    }

    public void Flip(float x)
    {
        if(x != 0.0f)
        {
            transform.localScale = new Vector3((float)((x > 0.0f) ? 1.0f : -1.0), 1.0f, 1.0f);
        }
    }

    private void ChangeAnimation(PlayerAnimationState animationState)
    {
        playerAnimationState = animationState;
        animator.SetInteger("AnimationState", (int)playerAnimationState);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(20);

            soundManager.PlaySoundFX(Sound.HURT, Channel.PLAYER_HURT_FX);

        }
    }
}
