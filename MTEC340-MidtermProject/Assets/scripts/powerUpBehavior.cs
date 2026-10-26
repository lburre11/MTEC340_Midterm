using UnityEngine;
using System.Collections;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField, Range(1.0f, 100.0f)] private float _rotateSpeed = 20.0f;
    private void Update()
    {
        transform.Rotate(0.0f, 0.0f, _rotateSpeed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        frogger frog = other.GetComponent<frogger>();
        GameManager gameManager = Object.FindAnyObjectByType<GameManager>();
        if (frog == null)
            return;
        shieldPowerUp shieldActivate = Object.FindAnyObjectByType<shieldPowerUp>();
        gameManager.powerUpPickup();
        shieldActivate.StartShield(frog, 10f);
        Destroy(gameObject);
    }
}