using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    readonly static WaitForSeconds _waitForSeconds10 = new(10);
    readonly static WaitForSeconds _waitForSeconds30 = new(30);
    Inputs input;

    float sleepiness;
    float waveHeight;
    float frequency;
    float wind;
    float waveHeightSpot;
    float frequencySpot;
    float risk;

    [SerializeField] GameObject pauseMenu;
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
        InitValues();
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

    void InitValues()
    {
        sleepiness = 20;
        waveHeight = 0;
        frequency = 0;
        wind = 0;
        waveHeightSpot = 0;
        frequencySpot = 0;
        risk = 0;
        paused = false;
        started = false;
        StartCoroutine(ChangeSpots());
        StartCoroutine(Wind());
    }
    void MenuCheck()
    {
        if (input.Player.Pause.WasPressedThisFrame())
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            paused = !paused;
        }
    }
    void FallingAsleep()
    {
        sleepinessBar.value = sleepiness;
        if (sleepiness <= 0)
            sleepiness = 0; //* Lose Level
        else if (sleepiness >= 100)
            sleepiness = 100; //* Win Level

        WaveHeightCheck();
        FrequencyCheck();
        CampfireCheck();
    }
    void WaveHeightCheck()
    {
        if (waveHeight >= waveHeightSpot - 10 && waveHeight <= waveHeightSpot + 10)
            sleepiness += Time.deltaTime * 2;
        else
            sleepiness -= Time.deltaTime * 2;
    }
    void FrequencyCheck()
    {
        if (frequency >= frequencySpot - 10 && frequency <= frequencySpot + 10)
            sleepiness += Time.deltaTime * 2;
        else
            sleepiness -= Time.deltaTime * 2;
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
}