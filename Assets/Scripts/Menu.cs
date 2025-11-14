using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject focusMe;
    [SerializeField] GameObject fade;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider textSpeedSlider;
    [SerializeField] TextMeshProUGUI textSpeedText;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    [SerializeField] Color[] colourPalette1;
    [SerializeField] Color[] colourPalette2;
    [SerializeField] Color[] colourPalette3;
    [HideInInspector] public Color[] currentPalette;
    [SerializeField] Button colourPalette1Object;
    [SerializeField] Button colourPalette2Object;
    [SerializeField] Button colourPalette3Object;
    [SerializeField] Button[] buttons;
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] Image[] imagesForeground;
    [SerializeField] Image[] imagesBackground;
    [SerializeField] Image[] imagesBackgroundDark;
    [SerializeField] Image[] images1;
    [SerializeField] Image[] images2;
    [SerializeField] Image[] images3;
    [SerializeField] Image[] images4;
    [SerializeField] Image dialogueBox;
    [SerializeField] Image WHF;
    [SerializeField] Image WHFFill;
    [SerializeField] Image wind;
    [SerializeField] Image campfireRisk;
    [SerializeField] Image campfireRiskFill;
    [SerializeField] Image boomboxRisk;
    [SerializeField] Image boomboxRiskFill;
    [SerializeField] Image[] controlImages;
    [SerializeField] Sprite[] sprites1;
    [SerializeField] Sprite[] sprites2;
    [SerializeField] Sprite[] sprites3;
    [SerializeField] Image pauseMenuBG;
    #endregion

    void Start()
    {
        InitValues();
        ListResolutions();
    }

    void InitValues()
    {
        audioMixer.SetFloat("_masterVolume", PlayerPrefs.GetFloat("_masterVolume") - 40);
        audioMixer.SetFloat("_musicVolume", PlayerPrefs.GetFloat("_musicVolume") - 40);
        audioMixer.SetFloat("_SFXVolume", PlayerPrefs.GetFloat("_SFXVolume") - 40);
        masterSlider.value = PlayerPrefs.GetFloat("_masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("_musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("_SFXVolume");

        qualityDropdown.value = PlayerPrefs.GetInt("_qualityLevel");
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("_qualityLevel"));

        if (PlayerPrefs.GetInt("_textSpeedLevel") == 0)
            PlayerPrefs.SetInt("_textSpeedLevel", 2);
        textSpeedSlider.value = PlayerPrefs.GetInt("_textSpeedLevel");
        SetTextSpeed(PlayerPrefs.GetInt("_textSpeedLevel"));

        if (PlayerPrefs.GetInt("_palette") == 0)
            PlayerPrefs.SetInt("_palette", 1);
        SetColourPalette(PlayerPrefs.GetInt("_palette"));

        switch (PlayerPrefs.GetInt("_palette"))
        {
            case 1:
                colourPalette1Object.interactable = false;
                break;
            case 2:
                colourPalette2Object.interactable = false;
                break;
            case 3:
                colourPalette3Object.interactable = false;
                break;
        }
    }
    public void Play()
    {
        fade.SetActive(true);
        StartCoroutine(GameManager.FadeIn(fade, 51));
        Invoke(nameof(LoadGame), 1);
    }
    void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void FocusFocusMe()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(focusMe); //! Sometimes throws object reference null
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("_masterVolume", volume - 40);
        PlayerPrefs.SetFloat("_masterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("_musicVolume", volume - 40);
        PlayerPrefs.SetFloat("_musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("_SFXVolume", volume - 40);
        PlayerPrefs.SetFloat("_SFXVolume", volume);
    }
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("_qualityLevel", index);
    }
    void ListResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new();

        int currentIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
    public void ResetLevels()
    {
        PlayerPrefs.SetInt("_level", 0);
        PlayerPrefs.SetInt("_relationship", 0);
        PlayerPrefs.SetInt("_ending", 0);
    }
    public void SetTextSpeed(float value)
    {
        switch (value)
        {
            case 1:
                GameManager.textSpeed = 255;
                textSpeedText.text = "1 - We have places to be.";
                PlayerPrefs.SetInt("_textSpeedLevel", 1);
                break;
            case 2:
                GameManager.textSpeed = 85;
                textSpeedText.text = "2 - Great taste for relaxation.";
                PlayerPrefs.SetInt("_textSpeedLevel", 2);
                break;
            case 3:
                GameManager.textSpeed = 51;
                textSpeedText.text = "3 - What I use personally.";
                PlayerPrefs.SetInt("_textSpeedLevel", 3);
                break;
            case 4:
                GameManager.textSpeed = 17;
                textSpeedText.text = "4 - Fast.";
                PlayerPrefs.SetInt("_textSpeedLevel", 4);
                break;
            case 5:
                GameManager.textSpeed = 15;
                textSpeedText.text = "5 - Slightly faster.";
                PlayerPrefs.SetInt("_textSpeedLevel", 5);
                break;
            case 6:
                GameManager.textSpeed = 5;
                textSpeedText.text = "6 - Even faster.";
                PlayerPrefs.SetInt("_textSpeedLevel", 6);
                break;
            case 7:
                GameManager.textSpeed = 3;
                textSpeedText.text = "7 - Why is this an option?";
                PlayerPrefs.SetInt("_textSpeedLevel", 7);
                break;
            case 8:
                GameManager.textSpeed = 1;
                textSpeedText.text = "8 - Can you even read that fast?";
                PlayerPrefs.SetInt("_textSpeedLevel", 8);
                break;
        }
    }
    public void SetColourPalette(int index)
    {
        switch (index)
        {
            case 1:
                currentPalette = colourPalette1;
                ColourSwitcher(colourPalette1);
                break;
            case 2:
                currentPalette = colourPalette2;
                ColourSwitcher(colourPalette2);
                break;
            case 3:
                currentPalette = colourPalette3;
                ColourSwitcher(colourPalette3);
                break;
            default:
                Debug.Log("Wtf?");
                break;
        }
        PlayerPrefs.SetInt("_palette", index);
    }
    void ColourSwitcher(Color[] palette)
    {
        ButtonColourSwitcher(palette);
        TextColourSwitcher(palette);
        ImageColourSwitcher(palette);
        ImageSwitcher(palette);

        if (SceneManager.GetActiveScene().name == "Game")
            ControlImageSwitcher(palette);
    }
    public void ControlImageSwitcher(Color[] palette)
    {
        foreach (Image image in controlImages)
        {
            if (palette == colourPalette1)
                image.sprite = sprites1[0];
            if (palette == colourPalette2)
                image.sprite = sprites2[0];
            if (palette == colourPalette3)
                image.sprite = sprites3[0];
        }
    }
    void ImageSwitcher(Color[] palette)
    {
        if (palette == colourPalette1)
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                dialogueBox.sprite = sprites1[5];
                WHF.sprite = sprites1[6];
                WHFFill.sprite = sprites1[7];
                campfireRisk.sprite = sprites1[8];
                campfireRiskFill.sprite = sprites1[9];
                wind.sprite = sprites1[10];
                boomboxRisk.sprite = sprites1[11];
                boomboxRiskFill.sprite = sprites1[12];
            }
            foreach (Image image in images1)
                image.sprite = sprites1[1];
            foreach (Image image in images2)
                image.sprite = sprites1[2];
            foreach (Image image in images3)
                image.sprite = sprites1[3];
            foreach (Image image in images4)
                image.sprite = sprites1[4];
        }
        if (palette == colourPalette2)
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                dialogueBox.sprite = sprites2[5];
                WHF.sprite = sprites2[6];
                WHFFill.sprite = sprites2[7];
                campfireRisk.sprite = sprites2[8];
                campfireRiskFill.sprite = sprites2[9];
                wind.sprite = sprites2[10];
                boomboxRisk.sprite = sprites2[11];
                boomboxRiskFill.sprite = sprites2[12];
            }
            foreach (Image image in images1)
                image.sprite = sprites2[1];
            foreach (Image image in images2)
                image.sprite = sprites2[2];
            foreach (Image image in images3)
                image.sprite = sprites2[3];
            foreach (Image image in images4)
                image.sprite = sprites2[4];
        }
        if (palette == colourPalette3)
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                dialogueBox.sprite = sprites3[5];
                WHF.sprite = sprites3[6];
                WHFFill.sprite = sprites3[7];
                campfireRisk.sprite = sprites3[8];
                campfireRiskFill.sprite = sprites3[9];
                wind.sprite = sprites3[10];
                boomboxRisk.sprite = sprites3[11];
                boomboxRiskFill.sprite = sprites3[12];
            }
            foreach (Image image in images1)
                image.sprite = sprites3[1];
            foreach (Image image in images2)
                image.sprite = sprites3[2];
            foreach (Image image in images3)
                image.sprite = sprites3[3];
            foreach (Image image in images4)
                image.sprite = sprites3[4];
        }
    }
    void ButtonColourSwitcher(Color[] palette)
    {
        Camera.main.backgroundColor = palette[0];
        pauseMenuBG.color = palette[0];
        foreach (Button button in buttons)
        {
            var colors = button.colors;
            colors.normalColor = palette[3];
            colors.highlightedColor = palette[2];
            colors.pressedColor = palette[1];
            colors.selectedColor = palette[0];
            colors.disabledColor = palette[4];
            button.colors = colors;
        }
    }
    void TextColourSwitcher(Color[] palette)
    {
        foreach (TextMeshProUGUI text in texts)
            text.color = palette[3];
    }
    void ImageColourSwitcher(Color[] palette)
    {
        foreach(Image image in imagesForeground)
            image.color = palette[2];

        foreach(Image image in imagesBackground)
            image.color = palette[1];

        foreach (Image image in imagesBackgroundDark)
            image.color = palette[5];
    }
}