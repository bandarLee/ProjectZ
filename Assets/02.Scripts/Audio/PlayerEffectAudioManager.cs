using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectAudioManager : MonoBehaviour
{
    public AudioSource[] audioSources;
    public AudioClip[] audioClips;
    public void PlayAudio(int Index)
    {
        if (Character.LocalPlayerInstance.PhotonView.IsMine)
        {
            if (Index < audioSources.Length && Index < audioClips.Length)
            {
                AudioSource selectedSource = audioSources[Index];
                selectedSource.clip = audioClips[Index];
                selectedSource.Play();
            }
        }
    }

    public void StopAllSounds()
    {
        if (Character.LocalPlayerInstance.PhotonView.IsMine)
        {
            foreach (AudioSource source in audioSources)
            {
                source.Stop();
            }
        }
    }
}
