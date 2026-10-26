using UnityEngine;
using System.Collections;

public class shieldPowerUp : MonoBehaviour
{
    AudioSource _source;
    [SerializeField] private AudioClip powerUpSFX;
    public void StartShield(frogger frog, float duration)
    {
        StartCoroutine(ShieldPowerUp(frog, duration));
    }

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private IEnumerator ShieldPowerUp(frogger frog, float duration)
    {
        frog.SetPowerUpActive(true);
        _source.PlayOneShot(powerUpSFX);
        frog.shieldSprite.enabled = true;
        
        float blinkDuration = Mathf.Min(3f, duration);
        float normalDuration = duration - blinkDuration;

        if (normalDuration > 0f)
            yield return new WaitForSeconds(normalDuration);
        float elapsed = 0f;
    
        while (elapsed < blinkDuration)
        {
            float progress = elapsed / blinkDuration;
            float blinkInterval = Mathf.Lerp(0.5f, 0.1f, progress);
            frog.shieldSprite.enabled = !frog.shieldSprite.enabled;
            float wait = Mathf.Min(blinkInterval, blinkDuration - elapsed);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }
        frog.shieldSprite.enabled = false;
        frog.SetPowerUpActive(false);
    }
}
