using UnityEngine;

public class SimpleInteractableSoundPlayer : MonoBehaviour, IInteractableSound
{
    [SerializeField] AudioClip sound;
    [SerializeField] AudioSource audioSource;

    public void PlayInteractSound()
    {
        if (sound == null)
        {
            return;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(sound);
            return;
        }

        AudioSource.PlayClipAtPoint(sound, transform.position);
    }

}
