using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public Vector2 direction;
    public Rigidbody2D rigidbody2D;
    [Range(1.0f, 30.0f)]
    public float force;
    public Vector3 offset;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Activate();
    }

    public void Activate()
    {
        Vector3 playerPosition = FindObjectOfType<PlayerBehaviour>().transform.position + offset; 
        direction = (playerPosition - transform.position).normalized;
        Rotate();
        Move();
        Invoke("DestroyYourself", 2.0f);
    }

    // Update is called once per frame
    private void Rotate()
    {
        rigidbody2D.AddTorque(Random.Range(5.0f, 15.0f) * direction.x * -1.0f, ForceMode2D.Impulse);
    }

    private void Move()
    {
        rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void DestroyYourself()
    {
        if (gameObject.activeInHierarchy)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player")
         || other.gameObject.CompareTag("Ground")
         || other.gameObject.CompareTag("Prop"))
        {
            DestroyYourself();
        }
    }
}
