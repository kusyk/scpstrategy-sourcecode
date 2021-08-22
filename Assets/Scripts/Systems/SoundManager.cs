using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public void PlayClickSound()
    {
        SoundSystem.Instance.PlayClickSound();
    }
    public void UnmuteMusic()
    {
        SoundSystem.Instance.UnmuteMusic();
    }
}