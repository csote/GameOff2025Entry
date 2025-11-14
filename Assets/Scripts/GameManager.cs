using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    #region Regions
    #region WaitForSeconds
    readonly static WaitForSeconds _waitForSeconds0_01 = new(0.01f);
    readonly static WaitForSeconds _waitForSeconds1 = new(1);
    readonly static WaitForSeconds _waitForSeconds10 = new(10);
    readonly static WaitForSeconds _waitForSeconds30 = new(30);
    #endregion

    #pragma warning disable UDR0001
    public static int textSpeed; //* Bro is contained :skullemoji:
    #pragma warning restore UDR0001

    #region General
    Inputs input;
    Menu menuScript;
    #endregion

    #region Variables
    float sleepiness, waveHeight, frequency, wind, waveHeightSpot, frequencySpot, leeway, difficulty, waitTime, maxWHF, windForce;
    int campfireRisk, boomboxRisk, campfireRiskFloor, boomboxRiskFloor, factorsCorrect;
    bool waveHeightCorrect, frequencyCorrect, campfireLit, musicPlaying, raining, won, lost, started, speaking, choosing, choice;
    string direction;
    #endregion

    #region SerializeField
    [SerializeField] GameObject he;
    [SerializeField] GameObject she;

    [SerializeField] Light2D globalLight;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] GameObject waveDawn;
    [SerializeField] GameObject waveNoon;
    [SerializeField] GameObject waveNight;
    [SerializeField] Animator waveDawnAnimator;
    [SerializeField] Animator waveNoonAnimator;
    [SerializeField] Animator waveNightAnimator;
    [SerializeField] GameObject campfire;
    [SerializeField] GameObject campfireLight;
    [SerializeField] GameObject boombox;
    [SerializeField] Sprite fireless;
    [SerializeField] Sprite broken;
    [SerializeField] Image whfFill;
    [SerializeField] Image campfireRiskFill;
    [SerializeField] Image boomboxRiskFill;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject choiceButton1;
    [SerializeField] GameObject choiceButton2;
    [SerializeField] GameObject fade;
    [SerializeField] Slider sleepinessBar;

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
        WaitTimeChecker();
        if (!paused && started)
        {
            FactorCheck();
            UpdateUI();
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
                waveNight.SetActive(true);
                waveNightAnimator.speed = 0.6f;
                difficulty = 2.5f;
                globalLight.intensity = 0.3f;
                campfireLit = true;
                campfireLight.SetActive(true);
                musicPlaying = false;
                raining = false;
                windForce = 10;
                break;
            case 1:
                he.SetActive(false);
                she.SetActive(true);
                waveDawn.SetActive(true);
                waveDawnAnimator.speed = 0.6f;
                difficulty = 2;
                globalLight.intensity = 1;
                globalLight.color = new Color(255/255f, 213/255f, 213/255f, 1);
                campfireLit = false;
                musicPlaying = true;
                raining = true;
                windForce = 6;
                break;
            case 2:
                he.SetActive(true);
                she.SetActive(true);
                waveNoon.SetActive(true);
                waveNoonAnimator.speed = 0.6f;
                difficulty = 1.5f;
                globalLight.intensity = 1;
                campfireLit = false;
                musicPlaying = true;
                raining = false;
                windForce = 3;
                break;
            case 3:
                he.SetActive(true);
                she.SetActive(true);
                waveNight.SetActive(true);
                waveNightAnimator.speed = 0.6f;
                difficulty = 1;
                globalLight.intensity = 0.3f;
                campfireLit = true;
                campfireLight.SetActive(true);
                musicPlaying = true;
                raining = true;
                windForce = 12;
                break;
            default:
                break;
        }

        waveNoonAnimator.speed = 0.6f;
        sleepiness = 50;
        sleepinessBar.value = sleepiness;
        waveHeight = 10;
        frequency = 10;
        wind = 0;
        waveHeightSpot = 10;
        frequencySpot = 10;
        leeway = 10;
        campfireRisk = 0;
        campfireRiskFloor = 0;
        boomboxRisk = 0;
        boomboxRiskFloor = 0;
        factorsCorrect = 0;
        waveHeightCorrect = false;
        frequencyCorrect = false;
        won = false;
        lost = false;
        paused = false;
        Time.timeScale = 1;
        started = false;
        speaking = false;
        choosing = false;
        choice = false;
        StartCoroutine(ChangeSpots());
        if (index != 0)
            StartCoroutine(Wind());
        StartCoroutine(CampfireCheck());
        StartCoroutine(BoomboxCheck());
        StartCoroutine(Dialogue(index));
    }
    void MenuCheck()
    {
        if (input.Player.Pause.WasPressedThisFrame() && !loseMenu.activeSelf)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
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
    void WaitTimeChecker()
    {
        switch (textSpeed)
        {
            case 1:
                waitTime = 0.02f;
                break;
            case 3:
                waitTime = 0.06f;
                break;
            case 5:
                waitTime = 0.1f;
                break;
            case 15:
                waitTime = 0.3f;
                break;
            case 17:
                waitTime = 0.34f;
                break;
            case 51:
                waitTime = 1.02f;
                break;
            case 85:
                waitTime = 1.7f;
                break;
            case 255:
                waitTime = 5.1f;
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
        if (waveDawn.activeSelf)
            waveDawnAnimator.speed = 0.6f + (value * 0.006f);
        else if (waveNoon.activeSelf)
            waveNoonAnimator.speed = 0.6f + (value * 0.006f);
        else if (waveNight.activeSelf)
            waveNightAnimator.speed = 0.6f + (value * 0.006f);
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
                    StartCoroutine(WinSpecial());
                else if (PlayerPrefs.GetInt("_relationship") == 2)
                    StartCoroutine(LoseSpecial());
            }
            else if (sleepiness >= 100 && !won)
            {
                won = true;
                sleepiness = 100;
                if (PlayerPrefs.GetInt("_relationship") == 1)
                    StartCoroutine(LoseSpecial());
                else if (PlayerPrefs.GetInt("_relationship") == 2)
                    StartCoroutine(WinSpecial());
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
    IEnumerator Dialogue(int index)
    {
        yield return new WaitUntil(() => started);
        speaking = true;
        yield return _waitForSeconds1;
        switch (index)
        {
            case 0:
                StartCoroutine(Speak("Lorem ipsum", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("dolor sit", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("amet consectetur", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("adipiscing elit", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                break;
            case 1:
                StartCoroutine(Speak("Lorem ipsum", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("dolor sit", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("amet consectetur", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("adipiscing elit", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                break;
            case 2:
                StartCoroutine(Speak("He: Hi.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("She: Hey.", waitTime, textSpeed));
                yield return new WaitForSeconds(waitTime * 2.5f);
                StartCoroutine(Speak("He: You like me?", waitTime, textSpeed, false));
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
                    StartCoroutine(Speak("She: Yep.", waitTime, textSpeed));
                    yield return new WaitForSeconds(waitTime * 2.5f);
                }
                else
                {
                    PlayerPrefs.SetInt("_relationship", 2);
                    StartCoroutine(Speak("She: Nope.", waitTime, textSpeed));
                    yield return new WaitForSeconds(waitTime * 2.5f);
                }
                break;
            case 3:
                switch (PlayerPrefs.GetInt("_relationship"))
                {
                    case 0:
                        StartCoroutine(Speak("You are not supposed to see this.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.5f);
                        break;
                    case 1:
                        StartCoroutine(Speak("He: Yay.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.5f);
                        break;
                    case 2:
                        StartCoroutine(Speak("He: :(.", waitTime, textSpeed));
                        yield return new WaitForSeconds(waitTime * 2.5f);
                        break;
                }
                break;
            default:
                Debug.Log("Not a level.");
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
        yield return null;
        loseMenu.SetActive(true);
        choiceButton1.SetActive(false);
        choiceButton2.SetActive(false);
        StopAllCoroutines();
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
        yield return new WaitUntil(() => !speaking);
        fade.SetActive(true);
        StartCoroutine(FadeIn(fade, 51));
        Invoke(nameof(Necessary2), 1);
    }
    IEnumerator LoseSpecial()
    {
        PlayerPrefs.SetInt("_ending", 1);
        SceneManager.LoadScene("Endings");
        yield return null;
    }
    IEnumerator WinSpecial()
    {
        PlayerPrefs.SetInt("_ending", 2);
        SceneManager.LoadScene("Endings");
        yield return null;
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
        yield return _waitForSeconds1;
        if (waveHeight >= 80 && campfireLit)
        {
            campfireRisk = Random.Range(campfireRiskFloor, 101);
            campfireRiskFloor++;
            campfireRiskFill.fillAmount = campfireRiskFloor / 100f;
        }

        if (campfireRisk >= 100)
        {
            campfireRiskFloor = 0;
            campfireRisk = 0;
            campfireRiskFill.fillAmount = 0;
            campfireLit = false;
            campfireLight.SetActive(false);
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
    IEnumerator BoomboxCheck()
    {
        yield return _waitForSeconds1;
        if (frequency >= 80 && musicPlaying)
        {
            boomboxRisk = Random.Range(boomboxRiskFloor, 101);
            boomboxRiskFloor++;
            boomboxRiskFill.fillAmount = boomboxRiskFloor / 100f;
        }

        if (boomboxRisk >= 100)
        {
            boomboxRiskFloor = 0;
            boomboxRisk = 0;
            boomboxRiskFill.fillAmount = 0;
            musicPlaying = false;
            boombox.GetComponent<Image>().sprite = broken;
            boombox.GetComponent<Button>().interactable = true;
            sleepiness -= 20;
        }
        StartCoroutine(BoomboxCheck());
    }
    public void RepairBoombox()
    {
        musicPlaying = true;
    }
    IEnumerator ChangeSpots()
    {
        yield return new WaitUntil(() => started);
        waveHeightSpot = Random.Range(10, 100f);
        frequencySpot = Random.Range(10, 100f);
        yield return _waitForSeconds30;
        StartCoroutine(ChangeSpots());
    }
    IEnumerator Wind()
    {
        yield return new WaitUntil(() => started);
        yield return _waitForSeconds10;
        wind = Random.Range(-windForce, windForce);
        waveHeight += wind;
        frequency += wind;
        StartCoroutine(Wind());
    }
    public static IEnumerator FadeIn(GameObject objectToFade, int cooldown, float maxStep = 255, bool isImage = true) //*1-3-5-15-17-51-85-255
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
                color.a = alpha / 255f;
                text.color = color;
                yield return _waitForSeconds0_01;
            }
            color.a = maxStep / 255f;
            text.color = color;
        }
    }
    public static IEnumerator FadeOut(GameObject objectToFade, int cooldown, float startingAlpha = 255, bool isImage = true)
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
                color.a = alpha / 255;
                text.color = color;
                yield return _waitForSeconds0_01;
            }
            color.a = 0;
            text.color = color;
        }
    }
}