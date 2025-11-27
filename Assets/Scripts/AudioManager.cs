using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Music")]
    public AudioClip gameMusic;
    public AudioClip titleMusic;

    [Header("SFX")]
    public AudioClip coinPickup;
    public AudioClip powerUpPickup;
    public AudioClip deathSound;
    public AudioClip gameOverSound;
    public AudioClip checkPointSound;
    public AudioClip jumpSound;
    public AudioClip wallSound;
    public AudioClip walkSound;
    public AudioClip dashSound;

    public AudioClip cockroachKill;
    public AudioClip armourBreak;
    public AudioClip robotBreak;

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.loop = true;
    }
}

