using UnityEngine;
using System.Collections;

public class frogger : MonoBehaviour
{
   AudioSource _source;
   private SpriteRenderer spriteRenderer;
   public Sprite idleSprite;
   public Sprite leapSprite;
   public Sprite deadSprite;
   private Vector3 spawnPoint;
   private float farthestRow;
   [SerializeField] private AudioClip moveSFX;
   [SerializeField] private AudioClip dieSFX;

   private void Awake()
   {
	  _source = GetComponent<AudioSource>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      spawnPoint = transform.position;
   }
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
      {
         transform.rotation = Quaternion.Euler(0f, 0f, 0f);
         Move(Vector3.up);
      }
      else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
      {
         transform.rotation = Quaternion.Euler(0f, 0f, 180f);
         Move(Vector3.down);
      }
      else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
      {
         transform.rotation = Quaternion.Euler(0f, 0f, 90f);
         Move(Vector3.left);
      }
      else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
      {
         transform.rotation = Quaternion.Euler(0f, 0f, -90f);
         Move(Vector3.right);
      }
   }

   private void Move(Vector3 direction)
   {
      Vector3 destination = transform.position + direction;
      Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("barrier"));
      Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("platform"));
      Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("obstacle"));
      if (barrier != null)
      {
         return;
      }

      if (platform != null)
      {
         transform.SetParent(platform.transform);
      }
      else
      {
         transform.SetParent(null);
      }

      if (obstacle != null && platform == null)
      {
         transform.position = destination;
         death();
      }
      else
      {
         if (destination.y > farthestRow)
         {
            farthestRow = destination.y;
            Object.FindAnyObjectByType<GameManager>().newRow();
         }
         StartCoroutine(Leap(destination));  
	    _source.pitch = Random.Range(0.8f, 1.2f);
		_source.PlayOneShot(moveSFX);
      }
	
   }

   private IEnumerator Leap(Vector3 destination)
   {
      Vector3 startPosition = transform.position;
      float elapsed = 0f;
      float duration = 0.125f;
      
      spriteRenderer.sprite = leapSprite;

      while (elapsed < duration)
      {
         float t = elapsed/duration;
         transform.position = Vector3.Lerp(startPosition, destination, t);
         elapsed += Time.deltaTime;
         yield return null;
      }
      transform.position = destination;
      spriteRenderer.sprite = idleSprite; 
   }

   public void death()
   {
      StopAllCoroutines();
      transform.rotation = Quaternion.identity;
      spriteRenderer.sprite = deadSprite;
      enabled  = false;
      
      Object.FindAnyObjectByType<GameManager>().Died();
	  _source.PlayOneShot(dieSFX);
   }

   public void Respawn()
   {
      StopAllCoroutines();
      transform.rotation = Quaternion.identity;
      transform.position = spawnPoint;
      farthestRow = spawnPoint.y;
      spriteRenderer.sprite = idleSprite;
      gameObject.SetActive(true);
      enabled = true;
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (enabled && other.gameObject.layer == LayerMask.NameToLayer("obstacle") && transform.parent == null)
      {
         death();
      }
   }
}
