using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    readonly static WaitForSeconds _waitForSeconds10 = new(10);
    readonly static WaitForSeconds _waitForSeconds30 = new(30);
    readonly static WaitForSeconds _waitForSeconds0_01 = new(0.01f);
    Inputs input;

    float sleepiness;
    float waveHeight;
    float frequency;
    float wind;
    float waveHeightSpot;
    float frequencySpot;
    float risk;
    float difficulty;
    bool won;
    bool lost;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject fade;
    [SerializeField] Slider sleepinessBar;

    [HideInInspector] public bool paused;
    bool started;

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
        fade.SetActive(true);
        StartCoroutine(FadeOut(fade, 51));
        Invoke(nameof(Necessary), 0.51f);
        #if UNITY_EDITOR
        PlayerPrefs.SetInt("_level", 0);
        #endif
        InitValues(PlayerPrefs.GetInt("_level"));
    }
    void Necessary()
    {
        fade.SetActive(false);
    }
    void Update()
    {
        MenuCheck();
        if (!paused && started)
        {
            FallingAsleep();
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
        started = true;
    }

    void InitValues(int index)
    {
        switch (index)
        {
            case 0:
                sleepiness = 50;
                difficulty = 2;
                break;
            case 1:
                sleepiness = 25;
                difficulty = 1;
                break;
            case 2:
                sleepiness = 20;
                difficulty = 0.5f;
                break;
            default:
                break;
        }
        sleepinessBar.value = sleepiness;

        waveHeight = 0;
        frequency = 0;
        wind = 0;
        waveHeightSpot = 0;
        frequencySpot = 0;
        risk = 0;
        won = false;
        lost = false;
        paused = false;
        started = false;
        StartCoroutine(ChangeSpots());
        StartCoroutine(Wind());
    }
    void MenuCheck()
    {
        if (input.Player.Pause.WasPressedThisFrame() && !loseMenu.activeSelf)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            paused = !paused;
        }
    }
    void FallingAsleep()
    {
        sleepinessBar.value = sleepiness;
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

        if (!won && !lost)
        {
            WaveHeightCheck();
            FrequencyCheck();
            CampfireCheck();
        }
    }
    IEnumerator Lose()
    {
        //* Waking up animation
        yield return null;
        loseMenu.SetActive(true);
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
        PlayerPrefs.SetInt("_level", PlayerPrefs.GetInt("_level") + 1);
        yield return null;
        fade.SetActive(true);
        StartCoroutine(FadeIn(fade, 51));
        Invoke(nameof(Necessary2), 1);
    }
    void WaveHeightCheck()
    {
        if (waveHeight >= waveHeightSpot - 10 && waveHeight <= waveHeightSpot + 10)
            sleepiness += Time.deltaTime * difficulty;
        else
            sleepiness -= Time.deltaTime * difficulty * 1.2f;
    }
    void FrequencyCheck()
    {
        if (frequency >= frequencySpot - 10 && frequency <= frequencySpot + 10)
            sleepiness += Time.deltaTime * difficulty;
        else
            sleepiness -= Time.deltaTime * difficulty * 1.2f;
    }
    void CampfireCheck()
    {
        if (waveHeight >= 90)
            risk += Time.deltaTime;
        else
            risk -= Time.deltaTime;

        if (risk <= 0)
            risk = 0;
        else if (risk >= 10)
        {
            risk = 0;
            sleepiness -= 20;
        }
    }
    IEnumerator ChangeSpots()
    {
        waveHeightSpot = Random.Range(0, 100f);
        frequencySpot = Random.Range(0, 100f);
        yield return _waitForSeconds30;
        StartCoroutine(ChangeSpots());
    }
    IEnumerator Wind()
    {
        yield return _waitForSeconds10;
        wind = Random.Range(-10f, 10f);
        waveHeight += wind;
        frequency += wind;
        StartCoroutine(Wind());
    }
    public static IEnumerator FadeIn(GameObject objectToFade, int cooldown)
    {
        float a = 0;
        float n = 255 / cooldown;
        Image component = objectToFade.GetComponent<Image>();
        Color color = component.color;

        for (int i = 0; i < cooldown; i++)
        {
            a += n;
            color.a = a / 255;
            component.color = color;
            yield return _waitForSeconds0_01;
        }
        color.a = 1;
        component.color = color;
    }
    public static IEnumerator FadeOut(GameObject objectToFade, int cooldown)
    {
        float a = 255;
        float n = 255 / cooldown;
        Image component = objectToFade.GetComponent<Image>();
        Color color = component.color;

        for (int i = 0; i < cooldown; i++)
        {
            a -= n;
            color.a = a / 255;
            component.color = color;
            yield return _waitForSeconds0_01;
        }
        color.a = 0;
        component.color = color;
    }
}