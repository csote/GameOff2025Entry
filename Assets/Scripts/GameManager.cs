using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    readonly static WaitForSeconds _waitForSeconds10 = new(10);
    readonly static WaitForSeconds _waitForSeconds30 = new(30);
    readonly static WaitForSeconds _waitForSeconds0_01 = new(0.01f);

    Inputs input;
    Menu menuScript;

    float sleepiness;
    float waveHeight;
    float frequency;
    float wind;
    float waveHeightSpot;
    float frequencySpot;
    float leeway;
    int risk;
    int riskN;
    float difficulty;
    int factorsCorrect;
    bool waveHeightCorrect;
    bool frequencyCorrect;
    bool campfireLit;
    bool musicPlaying;
    bool raining;
    bool won;
    bool lost;

    [SerializeField] GameObject he;
    [SerializeField] GameObject she;

    [SerializeField] TextMeshProUGUI wh;
    [SerializeField] TextMeshProUGUI f;
    [SerializeField] TextMeshProUGUI windText;
    [SerializeField] TextMeshProUGUI n;
    [SerializeField] GameObject campfire;
    [SerializeField] Sprite fireless;

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
        menuScript = GetComponent<Menu>();
        menuScript.ControlImageSwitcher(menuScript.currentPalette);
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
            FactorCheck();
            wh.text = "WH: " + waveHeightSpot;
            f.text = "F: " + frequencySpot;
            windText.text = "W: " + wind;
            n.text = "N: " + riskN;
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
                he.SetActive(true);
                she.SetActive(false);
                difficulty = 2.5f;
                campfireLit = true;
                musicPlaying = false;
                raining = false;
                break;
            case 1:
                he.SetActive(false);
                she.SetActive(true);
                difficulty = 2;
                campfireLit = false;
                musicPlaying = true;
                raining = true;
                break;
            case 2:
                he.SetActive(true);
                she.SetActive(true);
                difficulty = 1.5f;
                campfireLit = false;
                musicPlaying = true;
                raining = false;
                break;
            case 3:
                he.SetActive(true);
                she.SetActive(true);
                difficulty = 1;
                campfireLit = true;
                musicPlaying = true;
                raining = true;
                break;
            default:
                break;
        }

        sleepiness = 50;
        sleepinessBar.value = sleepiness;
        waveHeight = 0;
        frequency = 0;
        wind = 0;
        waveHeightSpot = 0;
        frequencySpot = 0;
        leeway = 10;
        risk = 0;
        riskN = 0;
        factorsCorrect = 0;
        waveHeightCorrect = false;
        frequencyCorrect = false;
        won = false;
        lost = false;
        paused = false;
        started = false;
        StartCoroutine(ChangeSpots());
        if (index != 0)
            StartCoroutine(Wind());
        StartCoroutine(CampfireCheck());
    }
    void MenuCheck()
    {
        if (input.Player.Pause.WasPressedThisFrame() && !loseMenu.activeSelf)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            paused = !paused;
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
        yield return new WaitForSeconds(1);
        if (waveHeight >= 90 && campfireLit)
        {
            risk = Random.Range(riskN, 101);
            riskN++;
        }

        if (risk >= 100)
        {
            riskN = 0;
            risk = 0;
            campfireLit = false;
            campfire.GetComponent<Image>().sprite = fireless;
            campfire.GetComponent<Button>().interactable = true;
            sleepiness -= 20;
        }
        StartCoroutine(CampfireCheck());
    }
    public void LightCampfire()
    {
        campfireLit = true;
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
    public void ResetLevels()
    {
        PlayerPrefs.SetInt("_level", 0);
    }
}