using UnityEngine;

public class moveCycle : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 1.0f;
    public int size = 1;
    private GameManager gameManager;
    private Vector3 leftEdge;
    private Vector3 rightEdge;

    private void Start()
    {
        gameManager = Object.FindAnyObjectByType<GameManager>();
        Camera mainCamera = Camera.main;
        leftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero);
        rightEdge = mainCamera.ViewportToWorldPoint(Vector3.right);
    }
    
    private void Update()
    {
        if (direction.x > 0 && (transform.position.x - size) > rightEdge.x)
        {
            Vector3 position = transform.position;
            position.x = leftEdge.x - size;
            transform.position = position;
        }
        else if (direction.x < 0 && (transform.position.x + size) < leftEdge.x)
        {
            Vector3 position = transform.position;
            position.x = rightEdge .x + size;
            transform.position = position;
        }
        else
        {
            transform.Translate(direction * (speed + gameManager.levelNumber / 2f) * Time.deltaTime);
        }
    }

}
