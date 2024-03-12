using UnityEngine;
using System.Collections;

public class DroppedObject : MonoBehaviour
{
    public Vector3 Velocity { get; set; }

    public float MinRotationSpeed = 0.0f;
    public float MaxRotationSpeed = 0.0f;
    public bool HurtsIdlePlayer = true;
    public bool Deflectable = false;

    public bool Dropped { get; set; }
    public bool Armed { get; set; }
    public Vector3 RotationVelocity { get; set; }

    public GameObject m_ExplosionPrefab;

    void Awake()
    {
        float rotationSpeed = Random.Range (MinRotationSpeed, MaxRotationSpeed);

        if (Random.Range (0, 2) == 1)
        {
            rotationSpeed = -rotationSpeed;
        }

        RotationVelocity = new Vector3 (0.0f, 0.0f, rotationSpeed);
    }

    void Update()
    {
        if (Dropped)
        {
            transform.position += Velocity * Time.deltaTime;
            transform.eulerAngles += RotationVelocity * Time.deltaTime;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.name == "GameCamera")
        {
            Destroy (gameObject);
        }
    }

    public void Bounce(int direction=-1)
    {
        Rigidbody2D rigidbody2d = GetComponent<Rigidbody2D> ();
        rigidbody2d.isKinematic = false;

        float randomX = Random.Range (100.0f, 200.0f);
        float randomY = Random.Range (250.0f, 300.0f);

        if (direction == -1) {
            if (Random.Range (0, 2) == 1)
                randomX = -randomX;
        }
        else if (direction == 0)
        {
            randomX = -randomX;
        }

        rigidbody2d.AddForce (new Vector2 (randomX, randomY));

        Armed = false;
    }

    public void Explode()
    {
        if (m_ExplosionPrefab != null)
        {
            GameObject explosionObject = GameObject.Instantiate(m_ExplosionPrefab);
            explosionObject.transform.position = gameObject.transform.position;
            explosionObject.transform.SetParent(GameVariables.instance.Board.transform);
        }
        Destroy (gameObject);
    }
}
