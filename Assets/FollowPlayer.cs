using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject playerTransform; // Assign this to your player's transform in the inspector
    public float speed = 5.0f; // Speed at which the enemy moves towards the player
    public float maxVelocity = 10.0f; // Maximum velocity of the enemy
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component from this GameObject
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (playerTransform.transform.position - transform.position).normalized;

            // Apply a controlled force towards the player
            // Using ForceMode.Impulse for an instant push with the given magnitude
            rb.AddForce(directionToPlayer * speed, ForceMode.Impulse);

            // Clamp the velocity to ensure it doesn't exceed the maximum value
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

            // Optionally, make the enemy face the player
            transform.LookAt(playerTransform.transform);
        }
    }
}