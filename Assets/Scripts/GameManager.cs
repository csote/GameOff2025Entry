using System.Collections; using System.Collections.Generic;
using TMPro;
using UnityEngine; using UnityEngine.UI; using UnityEngine.Audio; using UnityEngine.SceneManagement; using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    #region Regions
    #region WaitForSeconds
    readonly static WaitForSeconds _waitForSeconds0_01 = new(0.01f);
    readonly static WaitForSeconds _waitForSeconds1 = new(1);
    readonly static WaitForSeconds _waitForSeconds3 = new(3);
    readonly static WaitForSeconds _waitForSeconds5 = new(5);
    readonly static WaitForSeconds _waitForSeconds10 = new(10);
    readonly static WaitForSeconds _waitForSeconds30 = new(30);
    #endregion

    #region General
    Inputs input;
    Menu menuScript;
    #endregion

    #region Variables
    float sleepiness, waveHeight, frequency, wind, waveHeightSpot, frequencySpot, leeway, difficulty, waitTime, maxWHF, windForce;
    int campfireRisk, campfireRiskFloor, factorsCorrect;
    [HideInInspector] public int textSpeed;
    bool waveHeightCorrect, frequencyCorrect, campfireLit, musicPlaying, raining, won, lost, started, speaking, choosing, choice, permitted, fireOut, flag1, musicDown;
    string direction;
    float currentWaveCurrentFrame = 0;
    Animator currentAnimator;
    List<AudioClip> songs;
    #endregion

    #region SerializeField
    [SerializeField] GameObject he;
    [SerializeField] GameObject she;

    [SerializeField] Light2D globalLight;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] GameObject waveDawnLow;
    [SerializeField] GameObject waveDawn;
    [SerializeField] GameObject waveDawnHigh;
    [SerializeField] GameObject waveNoonLow;
    [SerializeField] GameObject waveNoon;
    [SerializeField] GameObject waveNoonHigh;
    [SerializeField] GameObject waveNightLow;
    [SerializeField] GameObject waveNightLowFireless;
    [SerializeField] GameObject waveNight;
    [SerializeField] GameObject waveNightFireless;
    [SerializeField] GameObject waveNightHigh;
    [SerializeField] GameObject waveNightHighFireless;
    [SerializeField] Animator waveDawnLowAnimator;
    [SerializeField] Animator waveDawnAnimator;
    [SerializeField] Animator waveDawnHighAnimator;
    [SerializeField] Animator waveNoonLowAnimator;
    [SerializeField] Animator waveNoonAnimator;
    [SerializeField] Animator waveNoonHighAnimator;
    [SerializeField] Animator waveNightLowAnimator;
    [SerializeField] Animator waveNightLowFirelessAnimator;
    [SerializeField] Animator waveNightAnimator;
    [SerializeField] Animator waveNightFirelessAnimator;
    [SerializeField] Animator waveNightHighAnimator;
    [SerializeField] Animator waveNightHighFirelessAnimator;
    [SerializeField] GameObject campfireLight;
    [SerializeField] GameObject campfireButton;
    [SerializeField] Image whfFill;
    [SerializeField] Image campfireRiskFill;
    [SerializeField] Image windFillLeft;
    [SerializeField] Image windFillRight;
    [SerializeField] GameObject funny;
    [SerializeField] TextMeshProUGUI funnyText;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject choiceButton1;
    [SerializeField] GameObject choiceButton2;
    [SerializeField] GameObject fade;
    [SerializeField] Slider sleepinessBar;
    [SerializeField] TextMeshProUGUI dynamicText;

    [SerializeField] AudioSource waveAudio;
    [SerializeField] AudioSource crackling;
    [SerializeField] AudioSource flint;
    [SerializeField] AudioSource extinguish;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioClip song1;
    [SerializeField] AudioClip song2;
    [SerializeField] AudioClip song3;
    [SerializeField] AudioClip song4;
    [SerializeField] AudioClip song5;
    [SerializeField] AudioClip song6;

    [HideInInspector] public bool paused;
    #endregion
    #endregion

    void Awake()
    {
        input = new();
    }
    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            fade.SetActive(true);
            StartCoroutine(FadeOut(fade, 51));
            Invoke(nameof(Necessary), 0.51f);
            Invoke(nameof(Funny), 7);
            menuScript = GetComponent<Menu>();
            menuScript.ControlImageSwitcher(menuScript.currentPalette);
            songs = new List<AudioClip> { song1, song2, song3, song4, song5, song6 };
            InitValues(PlayerPrefs.GetInt("_level"));
        }
    }
    void Necessary()
    {
        fade.SetActive(false);
    }
    void Funny()
    {
        StartCoroutine(FadeOut(funny, 51));
        StartCoroutine(FadeOut(funnyText.gameObject, 51, 255, false));
        Invoke(nameof(FunnyNecessary), 1);
    }
    void FunnyNecessary()
    {
        funny.SetActive(false);
        funnyText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            MenuCheck();
            WaitTimeChecker();
            WaveHeightCheck();
            FrequencyCheck();
            UpdateUI();
            ChangeWaveType();
            MusicKiller();
            if (!paused && started && permitted)
            {
                FactorCheck();
                FallingAsleep();
            }
        }
    }

    public void ChangeWaveHeight(float value)
    {
        waveHeight = value + wind;
        started = true;
    }
    public void ChangeFrequency(float value)
    {
        frequency = value + wind;
        WaveSpeed(value);
        started = true;
    }

    void InitValues(int index)
    {
        switch (index)
        {
            case 0:
                he.SetActive(true);
                she.SetActive(false);
                waveNightLow.SetActive(true);
                waveNightLowAnimator.speed = 0.6f;
                currentAnimator = waveNightLowAnimator;
                difficulty = 2.5f;
                globalLight.intensity = 0.3f;
                campfireLit = true;
                campfireLight.SetActive(true);
                musicPlaying = false;
                raining = false;
                windForce = 10;
                StartCoroutine(ChangeSpots(50, 50));
                crackling.Play();
                break;
            case 1:
                he.SetActive(false);
                she.SetActive(true);
                waveDawnLow.SetActive(true);
                waveDawnLowAnimator.speed = 0.6f;
                currentAnimator = waveDawnLowAnimator;
                difficulty = 2;
                globalLight.intensity = 1;
                globalLight.color = new Color(255/255f, 213/255f, 213/255f, 1);
                campfireLit = false;
                musicPlaying = true;
                raining = true;
                windForce = 6;
                StartCoroutine(ChangeSpots(Random.Range(10, 101), Random.Range(10, 101), true));
                break;
            case 2:
                he.SetActive(true);
                she.SetActive(true);
                waveNoonLow.SetActive(true);
                waveNoonLowAnimator.speed = 0.6f;
                currentAnimator = waveNoonLowAnimator;
                difficulty = 1.5f;
                globalLight.intensity = 1;
                campfireLit = false;
                musicPlaying = true;
                raining = false;
                windForce = 3;
                StartCoroutine(ChangeSpots(Random.Range(10, 101), Random.Range(10, 101), true));
                break;
            case 3:
                he.SetActive(true);
                she.SetActive(true);
                waveNightLow.SetActive(true);
                waveNightLowAnimator.speed = 0.6f;
                currentAnimator = waveNightLowAnimator;
                difficulty = 1;
                globalLight.intensity = 0.3f;
                campfireLit = true;
                campfireLight.SetActive(true);
                musicPlaying = true;
                raining = true;
                windForce = 10;
                StartCoroutine(ChangeSpots(Random.Range(10, 101), Random.Range(10, 101), true));
                crackling.Play();
                break;
            default:
                break;
        }

        sleepiness = 50;
        sleepinessBar.value = sleepiness;
        waveHeight = 0; frequency = 0; wind = 0; leeway = 10; campfireRisk = 0; campfireRiskFloor = 0; factorsCorrect = 0;
        Time.timeScale = 1;
        waveHeightCorrect = false; frequencyCorrect = false; won = false; lost = false; paused = false; started = false; speaking = false; choosing = false; choice = false; permitted = false; fireOut = false; flag1 = true; musicDown = false;
        if (index != 0)
            StartCoroutine(Wind());
        StartCoroutine(CampfireCheck());
        StartCoroutine(Dialogue(index));
        StartCoroutine(PlayMusic());
    }
    IEnumerator PlayMusic()
    {
        if (songs.Count == 0)
            songs = new List<AudioClip> { song1, song2, song3, song4, song5, song6 };
        int index = Random.Range(0, songs.Count);
        musicPlayer.clip = songs[index];
        yield return _waitForSeconds3;
        musicPlayer.Play();
        songs.Remove(songs[index]);
        yield return new WaitUntil(() => !musicPlayer.isPlaying);
        yield return _waitForSeconds5;
        StartCoroutine(PlayMusic());
    }
    void MenuCheck()
    {
        if (input.Player.Pause.WasPressedThisFrame() && !loseMenu.activeSelf)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
            if (paused)
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                    crackling.Pause();
                waveAudio.Pause();
            }
            else
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                    crackling.UnPause();
                waveAudio.UnPause();
            }
        }
    }
    public void PressPause()
    {
        if (!loseMenu.activeSelf)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
            if (paused)
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                    crackling.Pause();
                waveAudio.Pause();
            }
            else
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                    crackling.UnPause();
                waveAudio.UnPause();
            }
        }
    }
    void UpdateUI()
    {
        maxWHF = waveHeightSpot / 100;
        if (whfFill.fillAmount <= 0)
            direction = "";
        else if (whfFill.fillAmount >= maxWHF)
            direction = "down";

        if (direction == "down")
            whfFill.fillAmount -= 1.6f * Time.deltaTime * ((frequencySpot / 100) + 0.01f);
        else
            whfFill.fillAmount += 1.6f * Time.deltaTime * ((frequencySpot / 100) + 0.01f);
    }
    void ChangeWaveType()
    {
        currentWaveCurrentFrame = currentAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length * (currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) * currentAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;

        if (currentWaveCurrentFrame < 1)
        {
            if (waveHeight <= 33)
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                {
                    if (fireOut)
                    {
                        if (flag1)
                        {
                            flag1 = false;
                            campfireRiskFloor = 0;
                            campfireRisk = 0;
                            campfireRiskFill.fillAmount = 0;
                            campfireLit = false;
                            campfireLight.SetActive(false);
                            campfireButton.GetComponent<Button>().interactable = true;
                            crackling.Stop();
                            extinguish.Play();
                            sleepiness -= 20;
                        }
                        waveNightLow.SetActive(false);
                        waveNightLowFireless.SetActive(true);
                        waveNight.SetActive(false);
                        waveNightFireless.SetActive(false);
                        waveNightHigh.SetActive(false);
                        waveNightHighFireless.SetActive(false);
                        WaveSpeed(frequency);
                        currentAnimator = waveNightLowFirelessAnimator;
                    }
                    else
                    {
                        waveNightLow.SetActive(true);
                        waveNightLowFireless.SetActive(false);
                        waveNight.SetActive(false);
                        waveNightFireless.SetActive(false);
                        waveNightHigh.SetActive(false);
                        waveNightHighFireless.SetActive(false);
                        WaveSpeed(frequency);
                        currentAnimator = waveNightLowAnimator;
                    }
                }
                else if (PlayerPrefs.GetInt("_level") == 1)
                {
                    waveDawnLow.SetActive(true);
                    waveDawn.SetActive(false);
                    waveDawnHigh.SetActive(false);
                    WaveSpeed(frequency);
                    currentAnimator = waveDawnLowAnimator;
                }
                else if (PlayerPrefs.GetInt("_level") == 2)
                {
                    waveNoonLow.SetActive(true);
                    waveNoon.SetActive(false);
                    waveNoonHigh.SetActive(false);
                    WaveSpeed(frequency);
                    currentAnimator = waveNoonLowAnimator;
                }
            }
            else if (waveHeight > 33 && waveHeight <= 66)
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                {
                    if (fireOut)
                    {
                        if (flag1)
                        {
                            flag1 = false;
                            campfireRiskFloor = 0;
                            campfireRisk = 0;
                            campfireRiskFill.fillAmount = 0;
                            campfireLit = false;
                            campfireLight.SetActive(false);
                            campfireButton.GetComponent<Button>().interactable = true;
                            crackling.Stop();
                            extinguish.Play();
                            sleepiness -= 20;
                        }
                        waveNightLow.SetActive(false);
                        waveNightLowFireless.SetActive(false);
                        waveNight.SetActive(false);
                        waveNightFireless.SetActive(true);
                        waveNightHigh.SetActive(false);
                        waveNightHighFireless.SetActive(false);
                        WaveSpeed(frequency);
                        currentAnimator = waveNightFirelessAnimator;
                    }
                    else
                    {
                        waveNightLow.SetActive(false);
                        waveNightLowFireless.SetActive(false);
                        waveNight.SetActive(true);
                        waveNightFireless.SetActive(false);
                        waveNightHigh.SetActive(false);
                        waveNightHighFireless.SetActive(false);
                        WaveSpeed(frequency);
                        currentAnimator = waveNightAnimator;
                    }
                }
                else if (PlayerPrefs.GetInt("_level") == 1)
                {
                    waveDawnLow.SetActive(false);
                    waveDawn.SetActive(true);
                    waveDawnHigh.SetActive(false);
                    WaveSpeed(frequency);
                    currentAnimator = waveDawnAnimator;
                }
                else if (PlayerPrefs.GetInt("_level") == 2)
                {
                    waveNoonLow.SetActive(false);
                    waveNoon.SetActive(true);
                    waveNoonHigh.SetActive(false);
                    WaveSpeed(frequency);
                    currentAnimator = waveNoonAnimator;
                }
            }
            else if (waveHeight > 66)
            {
                if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                {
                    if (fireOut)
                    {
                        if (flag1)
                        {
                            flag1 = false;
                            campfireRiskFloor = 0;
                            campfireRisk = 0;
                            campfireRiskFill.fillAmount = 0;
                            campfireLit = false;
                            campfireLight.SetActive(false);
                            campfireButton.GetComponent<Button>().interactable = true;
                            crackling.Stop();
                            extinguish.Play();
                            sleepiness -= 20;
                        }
                        waveNightLow.SetActive(false);
                        waveNightLowFireless.SetActive(false);
                        waveNight.SetActive(false);
                        waveNightFireless.SetActive(false);
                        waveNightHigh.SetActive(false);
                        waveNightHighFireless.SetActive(true);
                        WaveSpeed(frequency);
                        currentAnimator = waveNightHighFirelessAnimator;
                    }
                    else
                    {
                        waveNightLow.SetActive(false);
                        waveNightLowFireless.SetActive(false);
                        waveNight.SetActive(false);
                        waveNightFireless.SetActive(false);
                        waveNightHigh.SetActive(true);
                        waveNightHighFireless.SetActive(false);
                        WaveSpeed(frequency);
                        currentAnimator = waveNightHighAnimator;
                    }
                }
                else if (PlayerPrefs.GetInt("_level") == 1)
                {
                    waveDawnLow.SetActive(false);
                    waveDawn.SetActive(false);
                    waveDawnHigh.SetActive(true);
                    WaveSpeed(frequency);
                    currentAnimator = waveDawnHighAnimator;
                }
                else if (PlayerPrefs.GetInt("_level") == 2)
                {
                    waveNoonLow.SetActive(false);
                    waveNoon.SetActive(false);
                    waveNoonHigh.SetActive(true);
                    WaveSpeed(frequency);
                    currentAnimator = waveNoonHighAnimator;
                }
            }
        }
    }
    void WaitTimeChecker()
    {
        switch (textSpeed)
        {
            case 16:
                waitTime = 0.64f;
                break;
            case 32:
                waitTime = 1.28f;
                break;
            case 64:
                waitTime = 2.56f;
                break;
            case 128:
                waitTime = 5.12f;
                break;
        }
    }
    void FactorCheck()
    {
        if (he.activeSelf && she.activeSelf)
        {
            if (waveHeightCorrect && frequencyCorrect && campfireLit)
                factorsCorrect = 3;
            else if ((waveHeightCorrect && frequencyCorrect) || (frequencyCorrect && campfireLit) || (waveHeightCorrect && campfireLit))
                factorsCorrect = 2;
            else if (waveHeightCorrect || frequencyCorrect || campfireLit)
                factorsCorrect = 1;
            else
                factorsCorrect = 0;
        }
        else if (he.activeSelf)
        {
            if (musicPlaying)
            {
                if (waveHeightCorrect && frequencyCorrect && campfireLit && raining)
                    factorsCorrect = 3;
                else if (
                (waveHeightCorrect && frequencyCorrect && campfireLit)
                || (waveHeightCorrect && frequencyCorrect && raining)
                || (frequencyCorrect && campfireLit && raining))
                    factorsCorrect = 2;
                else if (
                (waveHeightCorrect && frequencyCorrect)
                || (waveHeightCorrect && campfireLit)
                || (waveHeightCorrect && raining)
                || (frequencyCorrect && campfireLit)
                || (frequencyCorrect && raining)
                || (campfireLit && raining))
                    factorsCorrect = 1;
                else if (waveHeightCorrect || frequencyCorrect || campfireLit || raining)
                    factorsCorrect = 0;
                else
                    factorsCorrect = -1;
            }
            else
            {
                if (waveHeightCorrect && frequencyCorrect && campfireLit && raining)
                    factorsCorrect = 4;
                else if (
                (waveHeightCorrect && frequencyCorrect && campfireLit)
                || (waveHeightCorrect && frequencyCorrect && raining)
                || (frequencyCorrect && campfireLit && raining))
                    factorsCorrect = 3;
                else if (
                (waveHeightCorrect && frequencyCorrect)
                || (waveHeightCorrect && campfireLit)
                || (waveHeightCorrect && raining)
                || (frequencyCorrect && campfireLit)
                || (frequencyCorrect && raining)
                || (campfireLit && raining))
                    factorsCorrect = 2;
                else if (waveHeightCorrect || frequencyCorrect || campfireLit || raining)
                    factorsCorrect = 1;
                else
                    factorsCorrect = 0;
            }
        }
        else if (she.activeSelf)
        {
            if (raining)
            {
                if (waveHeightCorrect && frequencyCorrect && campfireLit && musicPlaying)
                    factorsCorrect = 3;
                else if (
                (waveHeightCorrect && frequencyCorrect && campfireLit)
                || (waveHeightCorrect && frequencyCorrect && musicPlaying)
                || (frequencyCorrect && campfireLit && musicPlaying))
                    factorsCorrect = 2;
                else if (
                (waveHeightCorrect && frequencyCorrect)
                || (waveHeightCorrect && campfireLit)
                || (waveHeightCorrect && musicPlaying)
                || (frequencyCorrect && campfireLit)
                || (frequencyCorrect && musicPlaying)
                || (campfireLit && musicPlaying))
                    factorsCorrect = 1;
                else if (waveHeightCorrect || frequencyCorrect || campfireLit || musicPlaying)
                    factorsCorrect = 0;
                else
                    factorsCorrect = -1;
            }
            else
            {
                if (waveHeightCorrect && frequencyCorrect && campfireLit && musicPlaying)
                    factorsCorrect = 4;
                else if (
                (waveHeightCorrect && frequencyCorrect && campfireLit)
                || (waveHeightCorrect && frequencyCorrect && musicPlaying)
                || (frequencyCorrect && campfireLit && musicPlaying))
                    factorsCorrect = 3;
                else if (
                (waveHeightCorrect && frequencyCorrect)
                || (waveHeightCorrect && campfireLit)
                || (waveHeightCorrect && musicPlaying)
                || (frequencyCorrect && campfireLit)
                || (frequencyCorrect && musicPlaying)
                || (campfireLit && musicPlaying))
                    factorsCorrect = 2;
                else if (waveHeightCorrect || frequencyCorrect || campfireLit || musicPlaying)
                    factorsCorrect = 1;
                else
                    factorsCorrect = 0;
            }
        }
    }
    void WaveSpeed(float value)
    {
        currentAnimator.speed = 0.6f + (value * 0.006f);
        waveAudio.pitch = 1 + (value / 100);
    }
    void FallingAsleep()
    {
        sleepinessBar.value = sleepiness;
        if (PlayerPrefs.GetInt("_level") == 3)
        {
            if (sleepiness <= 0 && !lost)
            {
                lost = true;
                sleepiness = 0;
                if (PlayerPrefs.GetInt("_relationship") == 1)
                    StartCoroutine(LoadEnding(1));
                else if (PlayerPrefs.GetInt("_relationship") == 2)
                    StartCoroutine(LoadEnding(2));
            }
            else if (sleepiness >= 100 && !won)
            {
                won = true;
                sleepiness = 100;
                if (PlayerPrefs.GetInt("_relationship") == 1)
                    StartCoroutine(LoadEnding(3));
                else if (PlayerPrefs.GetInt("_relationship") == 2)
                    StartCoroutine(LoadEnding(4));
            }
        }
        else
        {
            if (sleepiness <= 0 && !lost)
            {
                lost = true;
                sleepiness = 0;
                StartCoroutine(Lose());
            }
            else if (sleepiness >= 100 && !won)
            {
                won = true;
                sleepiness = 100;
                StartCoroutine(Win());
            }
        }

        if (!won && !lost)
        {
            if (he.activeSelf && she.activeSelf)
            {
                if (factorsCorrect == 3)
                    sleepiness += Time.deltaTime * difficulty * 2;
                else if (factorsCorrect == 2)
                    sleepiness += Time.deltaTime * difficulty * 1.3f;
                else if (factorsCorrect == 1)
                    sleepiness -= Time.deltaTime * difficulty * 1.3f;
                else if (factorsCorrect == 0)
                    sleepiness -= Time.deltaTime * difficulty * 2;
            }
            else if (he.activeSelf)
            {
                if (factorsCorrect == 4)
                    sleepiness += Time.deltaTime * difficulty * 2.3f;
                else if (factorsCorrect == 3)
                    sleepiness += Time.deltaTime * difficulty * 2;
                else if (factorsCorrect == 2)
                    sleepiness += Time.deltaTime * difficulty * 1.3f;
                else if (factorsCorrect == 1)
                    sleepiness -= Time.deltaTime * difficulty * 1.3f;
                else if (factorsCorrect == 0)
                    sleepiness -= Time.deltaTime * difficulty * 2;
                else if (factorsCorrect == -1)
                    sleepiness -= Time.deltaTime * difficulty * 2.3f;
            }
            else if (she.activeSelf)
            {
                if (factorsCorrect == 4)
                    sleepiness += Time.deltaTime * difficulty * 2.3f;
                else if (factorsCorrect == 3)
                    sleepiness += Time.deltaTime * difficulty * 2;
                else if (factorsCorrect == 2)
                    sleepiness += Time.deltaTime * difficulty * 1.3f;
                else if (factorsCorrect == 1)
                    sleepiness -= Time.deltaTime * difficulty * 1.3f;
                else if (factorsCorrect == 0)
                    sleepiness -= Time.deltaTime * difficulty * 2;
                else if (factorsCorrect == -1)
                    sleepiness -= Time.deltaTime * difficulty * 2.3f;
            }
        }
    }
    IEnumerator Dialogue(int index)
    {
        speaking = true;
        yield return _waitForSeconds1;
        switch (index)
        {
            case 0:
                StartCoroutine(Speak("This is nice.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("It would be nicer if these waves weren't so damn low.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wish someone could just slide some slider and change the height and frequency of these waves.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Maybe they could even try to match some indicator on their screen.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                yield return new WaitUntil(() => waveHeightCorrect && frequencyCorrect);
                permitted = true;
                StartCoroutine(Speak("Ah, that's more my speed. Thank you imaginary person.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("It's a shame I have to leave here in a few minutes.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("I really like this spot.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                break;
            case 1:
                StartCoroutine(Speak("This spot will have to do", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                permitted = true;
                StartCoroutine(Speak("I'm glad I got here before it got crowded.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("But looks like someone was here even earlier than me.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("I wonder who they were.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Shame they left, it gets lonely at this hour of the day.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                //! More dialogue
                break;
            case 2:
                permitted = true;
                StartCoroutine(Speak("???: You are in my spot.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("***: No way. I've been here since dawn.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("???: That's because I left here at dawn.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("***: That was you? I wondered who would leave", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("right when it's the best time to be here.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("???: Well I didn't have much choice. But it doesn't matter.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("You are still in my spot.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("***: Calm down, it is big enough for both of us.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Hayden: I guess. Hayden by the way.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: Finally, a name to put on the face. I'm Wendy.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Hayden: Yeah sorry for my manners. There was someone in my spot.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: You know the beach is like, miles long right?", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Hayden: Yeah, but this is the best spot on the whole beach.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: I noticed. Something about it just draws you here.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                yield return _waitForSeconds5;
                StartCoroutine(Speak("Hayden: You wanna spend the night here as well?", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: I haven't decided yet, will you?", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Hayden: One way to find out.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Do I want to spend the night here?", waitTime, textSpeed, false));
                choosing = true;
                yield return new WaitForSeconds(waitTime * 1.5f);
                StartCoroutine(PresentChoice(textSpeed));
                yield return new WaitUntil(() => !choosing);
                StartCoroutine(FadeOut(dialogueBox, textSpeed, 235));
                StartCoroutine(FadeOut(dialogueBox.GetComponentInChildren<TextMeshProUGUI>().gameObject, textSpeed, 255, false));
                yield return new WaitUntil(() => dialogueBox.GetComponent<Image>().color.a == 0);
                dialogueBox.SetActive(false);
                dialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
                yield return new WaitForSeconds(0.5f);
                if (choice)
                {
                    PlayerPrefs.SetInt("_relationship", 1);
                    StartCoroutine(Speak("Wendy: Sure, I wonder how it looks like at night.", waitTime, textSpeed));
                    yield return new WaitForSeconds(waitTime * 2.2f);
                    StartCoroutine(Speak("Hayden: Don't want to ruin the surprise.", waitTime, textSpeed));
                    yield return new WaitForSeconds(waitTime * 2.2f);
                }
                else
                {
                    PlayerPrefs.SetInt("_relationship", 2);
                    StartCoroutine(Speak("Wendy: Sorry, I need to be somewhere. I promised.", waitTime, textSpeed));
                    yield return new WaitForSeconds(waitTime * 2.2f);
                    StartCoroutine(Speak("Hayden: Your loss.", waitTime, textSpeed));
                    yield return new WaitForSeconds(waitTime * 2.2f);
                }
                break;
            case 3:
                permitted = true;
                switch (PlayerPrefs.GetInt("_relationship"))
                {
                    case 0:
                        StartCoroutine(Speak("You are not supposed to see this.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        break;
                    case 1:
                        StartCoroutine(Speak("Wendy: Woah.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Hayden: Told you, it's even better at night.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Wendy: Yeah I'm glad I came here now.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Hayden: I could watch this for hours.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Wendy: Same, but I don't know if I'll be able to.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("I am really sleepy.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        break;
                    case 2:
                        StartCoroutine(Speak("Hayden: I thought you needed to be somewhere?.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Wendy: Change of plans, so I decided to follow your advice.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Hayden: Was it good advice?", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Wendy: It's not as good as I expected I guess.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Hayden: I find it spectacular.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Wendy: I think I'll just sleep in this corner.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        StartCoroutine(Speak("Hayden: Suit yourself.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.2f);
                        break;
                }
                break;
            default:
                break;
        }
        speaking = false;
    }
    IEnumerator Speak(string sentence, float closeTime, int cd, bool autoClose = true)
    {
        TextMeshProUGUI dialogueText = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        dialogueText.text = sentence;
        dialogueBox.SetActive(true);
        if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
            StartCoroutine(FadeIn(dialogueBox, cd, 128));
        else
            StartCoroutine(FadeIn(dialogueBox, cd, 235));
        StartCoroutine(FadeIn(dialogueText.gameObject, cd, 255, false));
        yield return new WaitForSeconds(closeTime);
        if (autoClose)
        {
            if (PlayerPrefs.GetInt("_level") == 0 || PlayerPrefs.GetInt("_level") == 3)
                StartCoroutine(FadeOut(dialogueBox, cd, 128));
            else
                StartCoroutine(FadeOut(dialogueBox, cd, 235));
            StartCoroutine(FadeOut(dialogueText.gameObject, cd, 255, false));
            yield return new WaitUntil(() => dialogueBox.GetComponent<Image>().color.a == 0);
            dialogueBox.SetActive(false);
            dialogueText.text = "";
        }
    }
    IEnumerator PresentChoice(int cd)
    {
        StartCoroutine(FadeIn(choiceButton1.GetComponentInChildren<TextMeshProUGUI>().gameObject, cd, 255, false));
        StartCoroutine(FadeIn(choiceButton2.GetComponentInChildren<TextMeshProUGUI>().gameObject, cd, 255, false));
        yield return new WaitForSeconds(2);
        choiceButton1.GetComponent<Button>().interactable = true;
        choiceButton2.GetComponent<Button>().interactable = true;
        yield return new WaitUntil(() => !choosing);
        choiceButton1.GetComponent<Button>().interactable = false;
        choiceButton2.GetComponent<Button>().interactable = false;
        StartCoroutine(FadeOut(choiceButton1.GetComponentInChildren<TextMeshProUGUI>().gameObject, cd, 255, false));
        StartCoroutine(FadeOut(choiceButton2.GetComponentInChildren<TextMeshProUGUI>().gameObject, cd, 255, false));
    }
    public void Choose(bool result)
    {
        choice = false;
        if (result)
            choice = true;
        choosing = false;
    }
    IEnumerator Lose()
    {
        musicDown = true;
        yield return new WaitUntil(() => musicPlayer.pitch <= 0.01);
        musicDown = false;
        loseMenu.SetActive(true);
        choiceButton1.SetActive(false);
        choiceButton2.SetActive(false);
        StopAllCoroutines();
    }
    void MusicKiller()
    {
        if (musicDown)
        {
            if (musicPlayer.pitch > 0)
                musicPlayer.pitch -= Time.deltaTime;
        }
    }
    public void Restart()
    {
        fade.SetActive(true);
        StartCoroutine(FadeIn(fade, 51));
        Invoke(nameof(Necessary2), 1);
    }
    void Necessary2()
    {
        SceneManager.LoadScene("Game");
    }
    IEnumerator Win()
    {
        yield return new WaitUntil(() => !speaking);
        fade.SetActive(true);
        StartCoroutine(FadeIn(fade, 51));
        Invoke(nameof(Necessary3), 1);
    }
    void Necessary3()
    {
        PlayerPrefs.SetInt("_level", PlayerPrefs.GetInt("_level") + 1); 
        SceneManager.LoadScene("Game");
    }
    IEnumerator LoadEnding(int ending)
    {
        yield return new WaitUntil(() => !speaking);
        yield return _waitForSeconds3;
        switch (ending)
        {
            case 1:
                StartCoroutine(Speak("Hayden: Thanks for staying up with me.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: Thank you for inviting me here.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                break;
            case 2:
                StartCoroutine(Speak("Hayden: Seems you couldn't fall asleep.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: I wish I could have.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                break;
            case 3:
                StartCoroutine(Speak("Hayden: Oh no, I'm falling asleep.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Wendy: It's okay, at least we tried.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                break;
            case 4:
                StartCoroutine(Speak("Wendy: Good night.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                StartCoroutine(Speak("Hayden: Good night Wendy.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.2f);
                break;
            default:
                break;
        }
        fade.SetActive(true);
        StartCoroutine(FadeIn(fade, 51));
        PlayerPrefs.SetInt("_ending", ending);
        Invoke(nameof(Necessary4), 1);
    }
    void Necessary4()
    {
        SceneManager.LoadScene("Endings");
    }
    void WaveHeightCheck()
    {
        if (waveHeight >= waveHeightSpot - leeway && waveHeight <= waveHeightSpot + leeway)
            waveHeightCorrect = true;
        else
            waveHeightCorrect = false;
    }
    void FrequencyCheck()
    {
        if (frequency >= frequencySpot - leeway && frequency <= frequencySpot + leeway)
            frequencyCorrect = true;
        else
            frequencyCorrect = false;
    }
    IEnumerator CampfireCheck()
    {
        yield return new WaitUntil(() => permitted);
        yield return _waitForSeconds1;
        if (waveHeight >= 80 && campfireLit)
        {
            campfireRisk = Random.Range(campfireRiskFloor, 101);
            campfireRiskFloor += 4;
            campfireRiskFill.fillAmount = campfireRiskFloor / 100f;
        }

        if (campfireRisk >= 100)
            fireOut = true;
        yield return new WaitUntil(() => !fireOut);
        StartCoroutine(CampfireCheck());
    }
    public void LightCampfire()
    {
        StartCoroutine(LightingFire());
    }
    IEnumerator LightingFire()
    {
        yield return new WaitUntil(() => currentWaveCurrentFrame < 1);
        campfireLit = true;
        campfireLight.SetActive(true);
        flint.Play();
        crackling.Play();
        fireOut = false;
        flag1 = true;
    }
    IEnumerator ChangeSpots(float waveHeight, float frequency, bool again = false)
    {
        waveHeightSpot = waveHeight;
        frequencySpot = frequency;
        if (again)
        {
            yield return _waitForSeconds30;
            yield return new WaitUntil(() => permitted);
            StartCoroutine(ChangeSpots(Random.Range(10, 101), Random.Range(10, 101)));
        }
    }
    IEnumerator Wind()
    {
        yield return new WaitUntil(() => started);
        yield return _waitForSeconds10;
        wind = Random.Range(-windForce, windForce);

        if (wind < 0)
        {
            windFillLeft.gameObject.SetActive(true);
            windFillLeft.fillAmount = wind / 10;
            windFillRight.gameObject.SetActive(false);
        }
        else
        {
            windFillLeft.gameObject.SetActive(false);
            windFillRight.gameObject.SetActive(true);
            windFillRight.fillAmount = wind / 10;
        }

        waveHeight += wind;
        frequency += wind;
        StartCoroutine(Wind());
    }
    public IEnumerator FadeIn(GameObject objectToFade, int cooldown, float maxStep = 255, bool isImage = true)
    {
        float alpha = 0;
        float step = maxStep / cooldown;
        Image component;
        TextMeshProUGUI text;

        if (isImage)
        {
            component = objectToFade.GetComponent<Image>();
            Color color = component.color;

            for (int i = 0; i < cooldown; i++)
            {
                alpha += step;
                color.a = alpha / 255f;
                component.color = color;
                yield return _waitForSeconds0_01;
            }
            color.a = maxStep / 255f;
            component.color = color;
        }
        else
        {
            text = objectToFade.GetComponent<TextMeshProUGUI>();
            Color color = text.color;

            for (int i = 0; i < cooldown; i++)
            {
                alpha += step;
                if (text == dynamicText)
                    dynamicText.color = new Color(menuScript.currentPalette[3].r, menuScript.currentPalette[3].g, menuScript.currentPalette[3].b, alpha / 255f);
                else
                {
                    color.a = alpha / 255f;
                    text.color = color;
                }
                yield return _waitForSeconds0_01;
            }
            color.a = maxStep / 255f;
            if (text == dynamicText)
                dynamicText.color = menuScript.currentPalette[3];
            else
                text.color = color;
        }
    }
    public IEnumerator FadeOut(GameObject objectToFade, int cooldown, float startingAlpha = 255, bool isImage = true)
    {
        float alpha = startingAlpha;
        float step = startingAlpha / cooldown;
        Image component;
        TextMeshProUGUI text;

        if (isImage)
        {
            component = objectToFade.GetComponent<Image>();
            Color color = component.color;

            for (int i = 0; i < cooldown; i++)
            {
                alpha -= step;
                color.a = alpha / 255;
                component.color = color;
                yield return _waitForSeconds0_01;
            }
            color.a = 0;
            component.color = color;
        }
        else
        {
            text = objectToFade.GetComponent<TextMeshProUGUI>();
            Color color = text.color;

            for (int i = 0; i < cooldown; i++)
            {
                alpha -= step;
                if (text == dynamicText)
                    dynamicText.color = new Color(menuScript.currentPalette[3].r, menuScript.currentPalette[3].g, menuScript.currentPalette[3].b, alpha / 255f);
                else
                {
                    color.a = alpha / 255;
                    text.color = color;
                }
                yield return _waitForSeconds0_01;
            }
            color.a = 0;
            if (text == dynamicText)
                dynamicText.color = new Color(menuScript.currentPalette[3].r, menuScript.currentPalette[3].g, menuScript.currentPalette[3].b, 0);
            else
                text.color = color;
        }
    }
}
