using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;



    public void PlayAudio(int index)
    {
        if (index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }

    public void StopAllSounds()
    {
        audioSource.Stop();
    }
}
