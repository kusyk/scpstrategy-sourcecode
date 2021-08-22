using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    public static SoundSystem Instance;

    [Header("Audio Clips")]
    public AudioClip menuClip;
    public AudioClip minigameClip;
    public List<AudioClip> gameClips = new List<AudioClip>();

    [Header("Audio Sources")]
    public AudioSource mainAudioSource1;
    public AudioSource mainAudioSource2;
    public AudioSource clickAudioSource;
    public AudioSource notificationAudioSource;
    public AudioSource actionAudioSource;
    public AudioSource attackAudioSource;
    public AudioSource minigameResultAudioSource;

    //changing clips
    private AudioSource tempAudioSource;
    private float maxMusicVolume = 1f;
    private bool change = false;

    public float getMaxMusicVolume
    {
        get
        {
            return maxMusicVolume;
        }
    }

    public void SetUpVolume()
    {
        maxMusicVolume = SettingsManager.settings.music * 0.1f;
        mainAudioSource1.volume = SettingsManager.settings.music * 0.1f;
        clickAudioSource.volume = SettingsManager.settings.ui * 0.1f;
        notificationAudioSource.volume = SettingsManager.settings.ui * 0.1f;
        actionAudioSource.volume = SettingsManager.settings.ui * 0.1f;
        attackAudioSource.volume = SettingsManager.settings.ui * 0.1f;
        minigameResultAudioSource.volume = SettingsManager.settings.ui * 0.1f;

        if (mainAudioSource1.clip == null)
        {
            StopAllCoroutines();
            PlayRandomClip();
        }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameGlobe"
            && mainAudioSource1.clip != menuClip)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeSound(menuClip));
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameGlobe"
             && !gameClips.Contains(mainAudioSource1.clip))
        {
            StopAllCoroutines();
            PlayRandomClip();
        }
    }

    public void MuteMusic()
    {
        maxMusicVolume = 0;
        mainAudioSource1.volume = 0;
        mainAudioSource2.volume = 0;
    }

    public void UnmuteMusic()
    {
        maxMusicVolume = SettingsManager.settings.music * 0.1f;
        mainAudioSource1.volume = 1 * maxMusicVolume;
        mainAudioSource2.volume = 0;
    }

    public void PlayClickSound()
    {
        if (clickAudioSource != null && Time.timeSinceLevelLoad > 1) clickAudioSource.Play();
    }

    public void PlayNotificationSound()
    {
        if (notificationAudioSource != null) notificationAudioSource.Play();
    }

    public void PlayActionSound()
    {
        if (actionAudioSource != null) actionAudioSource.Play();
    }

    public void PlayAttackSound()
    {
        if (attackAudioSource != null) attackAudioSource.Play();
    }

    public void PlayMinigameResultSound()
    {
        if (minigameResultAudioSource != null) minigameResultAudioSource.Play();
    }

    public void PlayMinigameClip()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeSound(minigameClip));
    }


    void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    //private void Start()
    //{
    //    SetUpVolume();
    //}

    void Update()
    {
        if (mainAudioSource1 == null || mainAudioSource2 == null) return;
        if (mainAudioSource1.clip == null) return;

        if (change)
        {
            mainAudioSource1.volume += Time.deltaTime / 1 * maxMusicVolume;
            mainAudioSource2.volume -= Time.deltaTime / 1 * maxMusicVolume;
        }
        else if (mainAudioSource1.time + 6 >= mainAudioSource1.clip.length)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameGlobe")
                StartCoroutine(ChangeSound(menuClip));
            else if (MinigamesController.isAnyMinigameActive)
                StartCoroutine(ChangeSound(minigameClip));
            else
                PlayRandomClip();
        }

        if (Input.GetKeyDown(KeyCode.M) && Debug.isDebugBuild)
        {
            PlayRandomClip();
        }
    }

    private void PlayRandomClip()
    {
        int random;

        do
        {
            random = Random.Range(0, gameClips.Count);
        }
        while (gameClips[random] == mainAudioSource1.clip);

        StartCoroutine(ChangeSound(gameClips[random]));
    }

    public IEnumerator ChangeSound(AudioClip clip)
    {
        mainAudioSource2.Stop();
        mainAudioSource2.volume = 0;
        mainAudioSource2.clip = clip;
        tempAudioSource = mainAudioSource2;
        mainAudioSource2 = mainAudioSource1;
        mainAudioSource1 = tempAudioSource;
        change = true;
        mainAudioSource1.Play();
        yield return new WaitForSeconds(1);
        change = false;
        mainAudioSource1.volume = 1 * maxMusicVolume;
        mainAudioSource2.volume = 0;
    }
}
