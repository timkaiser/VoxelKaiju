using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Management;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// The static instance of the SoundManager in the scene.
    /// </summary>
    public static SoundManager Instance { get; private set; } = null;

    #region Variables

    /// <summary>
    /// Reference to the GameManager in the scene.
    /// </summary>
    GameManager manager;

    /// <summary>
    /// The coroutine running at the moment.
    /// </summary>
    Coroutine routineRunning;

    /// <summary>
    /// The AudioSource that will play effects.
    /// </summary>
    [SerializeField][Tooltip("The audio source that will play effects.")]
    private AudioSource efxSource;

    /// <summary>
    /// The AudioSource that will play background music.
    /// </summary>
    [SerializeField]
    [Tooltip("The audio source that will play background music.")]
    private AudioSource musicSource;

    /// <summary>
    /// The AudioSource that will play effects.
    /// </summary>
    [SerializeField]
    [Tooltip("The audio source that will play constant effects e.g. walking sounds.")]
    private AudioSource charSource;

    /// <summary>
    /// The constant value that an effect like steps can minimally be pitched to.
    /// </summary>
    private const float lowPitchRange = 0.9f;

    /// <summary>
    /// The constant value that an effect like steps can maximally be pitched to.
    /// </summary>
    private const float highPitchRange = 1.1f;

    /// <summary>
    /// The list of all sound effect clips that the Sound Manager will be able to play.
    /// </summary>
    [SerializeField]
    [Tooltip("The list of effect clips.")]
    public List<AudioClip> efxClips;

    /// <summary>
    /// The list of all background music clips that the Sound Manager will be able to play.
    /// </summary>
    [SerializeField]
    [Tooltip("The list of background music clips.")]
    public List<AudioClip> musicClips;

    /// <summary>
    /// The list of all character effect clips that the Sound Manager will be able to play.
    /// </summary>
    [SerializeField]
    [Tooltip("The list of character step effect clips.")]
    public List<AudioClip> stepClips;


    /// <summary>
    /// The list of all punch effect clips that the Sound Manager will be able to play.
    /// </summary>
    [SerializeField]
    [Tooltip("The list of punch effect clips.")]
    public List<AudioClip> punchClips;
    #endregion

    #region Basic Functions
    private void Awake()
    {
        if (Instance == null)                       //Check if there is already an instance of SoundManager
            Instance = this;                        //if not, set it to this.
        else if (Instance != this)                  //If instance already exists:
        {
            Destroy(gameObject);                    //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.      
            return;
        }
        DontDestroyOnLoad(gameObject);              //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
    }

    private void Start()
    {
        manager = GameManager.Instance;
        manager.GameStateSwitched.AddListener(OnGameStateChange);
        OnGameStateChange();
    }

    private void OnDisable()
    {
        manager.GameStateSwitched.RemoveListener(OnGameStateChange);
    }
    /// <summary>
    /// The function to be added to the scene controller "After scene load" listener.
    /// </summary>
    private void OnGameStateChange()
    {
        if (routineRunning != null) StopAllCoroutines();
        switch (manager.GameState)
        {
            case GameManager.GameStates.Title:
                routineRunning = StartCoroutine(FadeOutAndStart(1f, "Title"));
                break;
            /*case GameManager.GameStates.Menu:
                routineRunning = StartCoroutine(FadeOutAndIn(2f, "Menu"));
                break;*/
            case GameManager.GameStates.Playing:
                if (LevelManager.Instance.GetActiveLevelTitle().Equals("Demo"))
                    routineRunning = StartCoroutine(FadeOutAndIn(2f, "Beach"));
                else 
                    routineRunning = StartCoroutine(FadeOutAndIn(2f, "City"));
                break;
            case GameManager.GameStates.Paused:
                routineRunning = StartCoroutine(FadeOutAndIn(2f, "Paused"));
                break;
            case GameManager.GameStates.Scores:
                routineRunning = StartCoroutine(FadeOut(1f));
                break;
        }
    }
    #endregion

    #region Public Functions

    /// <summary>
    /// Get the step clip from the list saved in the Sound Manager.
    /// </summary>
    /// <param name="name">The name of the step clip.</param>
    /// <returns>The effect clip if found, else null.</returns>
    public AudioClip GetStep(string name)
    {
        return stepClips.Find(clip => clip.name.Contains(name));
    }

    /// <summary>
    /// Get the effect clip from the list saved in the Sound Manager.
    /// </summary>
    /// <param name="name">The name of the effect clip.</param>
    /// <returns>The effect clip if found, else null.</returns>
    public AudioClip GetEffect(string name)
    {
        return efxClips.Find(clip => clip.name.Contains(name));
    }

    /// <summary>
    /// Get the effect clip from the list saved in the Sound Manager.
    /// </summary>
    /// <param name="pos">The position of the effect clip inside the list of the Sound Manager.</param>
    /// <returns>The effect clip if found, else null.</returns>
    public AudioClip GetEffect(int pos)
    {
        return efxClips[pos];
    }

    /// <summary>
    /// Get the music clip from the list saved in the Sound Manager.
    /// </summary>
    /// <param name="name">The name of the music clip.</param>
    /// <returns>The audio clip if found, else null.</returns>
    public AudioClip GetMusic(string name)
    {
        return musicClips.Find(clip => clip.name.Contains(name));
    }

    /// <summary>
    /// Get the music clip from the list saved in the Sound Manager.
    /// </summary>
    /// <param name="pos">The position of the music clip inside the list of the Sound Manager.</param>
    /// <returns>The audio clip if found, else null.</returns>
    private AudioClip GetMusic(int pos)
    {
        return musicClips[pos];
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Plays a sound effect from the available sound effect list saved in the Sound Manager.
    /// </summary>
    /// <param name="pos">The position of the effect in the effects list saved in the Sound Manager.</param>
    public void PlayEFX(int pos)
    {
        efxSource.PlayOneShot(efxClips[pos]);
    }

    /// <summary>
    /// Plays a sound effect from the available sound effect list saved in the Sound Manager.
    /// </summary>
    /// <param name="name">The name of the effect in the effects list saved in the Sound Manager.</param>
    public void PlayEFX(string name)
    {
        efxSource.PlayOneShot(GetEffect(name));
    }

    /// <summary>
    /// Plays a sound effect that you provide in the effects source.
    /// </summary>
    /// <param name="clip">The audio clip of the effect that you want to play.</param>
    public void PlayEFX(AudioClip clip)
    {
        efxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays a background music from the available music list saved in the Sound Manager.
    /// </summary>
    /// <param name="pos">The position of the effect in the music list saved in the Sound Manager.</param>
    public void PlayMusic(int pos)
    {
        PlayMusic(musicClips[pos]);
    }
    
    /// <summary>
    /// Plays a background music from the available music list saved in the Sound Manager.
    /// </summary>
    /// <param name="name">The name of the effect in the music list saved in the Sound Manager.</param>
    public void PlayMusic(string name)
    {
        PlayMusic(GetMusic(name));
    }
    
    /// <summary>
    /// Plays a background music that you provide from the music source of the Sound Manager.
    /// </summary>
    /// <param name="clip">The audio clip of the music that you want to play.</param>
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;                      //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        musicSource.Play();                           //Play the clip.
    }

    /// <summary>
    /// Plays a step sound from the available list saved in the SoundManager.
    /// </summary>
    /// <param name="name">The name of the step sound effect in the step list saved in the SoundManager.</param>
    public void PlayStep(string name)
    {
        charSource.PlayOneShot(GetStep(name));
    }

    /// <summary>
    /// Plays a random punch at random pitch.
    /// </summary>
    /// <param name="source">The audio source the punch should be played from.</param>
    public void RandomizePunches(AudioSource source)
    {
        RandomizeSfx(source, punchClips);
    }
    
    /// <summary>
    /// RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    /// </summary>
    /// <param name="source">The audio source you want to play this effects off, typically the efx source of the Sound Manager.</param>
    /// <param name="clips">A list of audio clips that you want to randomly choose one from.</param>
    public void RandomizeSfx(AudioSource source, List<AudioClip> clips)
    {
        RandomizePitch(source);    
        source.PlayOneShot(clips[Random.Range(0, clips.Count)]);            //Play the clip at random position.
    }

    /// <summary>
    /// Randomizes the pitch of the source within the values of lowPitch and highPitch range.
    /// </summary>
    /// <param name="source">The source whose pitch has to be adjusted.</param>
    public void RandomizePitch(AudioSource source)
    {
        //Choose a random pitch to play back our clip at between our high and low pitch ranges.  
        source.pitch = Random.Range(lowPitchRange, highPitchRange);
    }

    /// <summary>
    /// Fades in a background music clip from volume 0 to volume 1 with the duration of FadeTime.
    /// </summary>
    /// <param name="FadeTime">The time in seconds as float that the music should take to fade in.</param>
    /// <param name="clip">The music clip that is going to be played and faded in.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeIn(float FadeTime, AudioClip clip)
    {
        musicSource.volume = 0;
        PlayMusic(clip);
        while (musicSource.volume < 1)
        {
            musicSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    /// <summary>
    /// Fades in a background music clip from volume 0 to volume 1 with the duration of FadeTime.
    /// </summary>
    /// <param name="FadeTime">The time in seconds as float that the music should take to fade in.</param>
    /// <param name="name">The name of the music clip that is saved in the list of the Sound Manager.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeIn(float FadeTime, string name)
    {
        musicSource.volume = 0;
        PlayMusic(GetMusic(name));
        while (musicSource.volume < 1)
        {
            musicSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    /// <summary>
    /// Fades in a background music clip from volume 0 to volume 1 with the duration of FadeTime.
    /// </summary>
    /// <param name="FadeTime">The time in seconds as float that the music should take to fade in.</param>
    /// <param name="pos">The position of the music clip that is saved in the list of the Sound Manager.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeIn(float FadeTime, int pos)
    {
        musicSource.volume = 0;
        PlayMusic(GetMusic(pos));
        while (musicSource.volume < 1)
        {
            musicSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
        musicSource.volume = 1.0f;
    }

    /// <summary>
    /// Fades out the currently played background music until its volume reaches 0.0 in the duration of FadeTime.
    /// </summary>
    /// <param name="FadeTime">The time it should take to fade out.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeOut(float FadeTime)
    {
        if (musicSource.isPlaying)
        {
            while (musicSource.volume > 0f)
            {
                musicSource.volume -= Time.deltaTime / FadeTime;
                yield return null;
            }
            musicSource.Stop();
        }
        musicSource.volume = 0.0f;
    }

    /// <summary>
    /// Fades out the currently played background music and fades in a new one.
    /// </summary>
    /// <param name="FadeTime">The time it should take in total to fade out and in again.</param>
    /// <param name="clip">The music clip that should be faded in after the fade out.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeOutAndIn(float FadeTime, AudioClip clip)
    {        
        yield return FadeOut(FadeTime/2);
        yield return FadeIn(FadeTime / 2, clip);
        routineRunning = null;
    }

    /// <summary>
    /// Fades out the currently played background music and fades in a new one.
    /// </summary>
    /// <param name="FadeTime">The time it should take in total to fade out and in again.</param>
    /// <param name="name">The name of the music clip that should be faded in after the fade out which is saved in the list by the Sound Manager.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeOutAndIn(float FadeTime, string name)
    {
        yield return FadeOut(FadeTime / 2);
        yield return FadeIn(FadeTime / 2, name);
        routineRunning = null;
    }

    /// <summary>
    /// Fades out the currently played background music and fades in a new one.
    /// </summary>
    /// <param name="FadeTime">The time it should take in total to fade out and in again.</param>
    /// <param name="pos">The position of the music clip that should be faded in after the fade out which is saved in the list by the Sound Manager.</param>
    /// <returns>Returns an IEnumerator. Please start this as a coroutine.</returns>
    public IEnumerator FadeOutAndIn(float FadeTime, int pos)
    {
        yield return FadeOut(FadeTime / 2);
        yield return FadeIn(FadeTime / 2, pos);
        routineRunning = null;
    }

    public IEnumerator FadeOutAndStart(float FadeTime, string name)
    {
        yield return FadeOut(FadeTime);
        musicSource.volume = 1f;
        musicSource.clip = GetMusic(name);
        musicSource.Play();
    }

    #endregion
}
