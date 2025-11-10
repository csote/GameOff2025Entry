using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject focusMe;
    [SerializeField] GameObject fade;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
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
    [SerializeField] Image[] controlImages;
    [SerializeField] Sprite[] sprites1;
    [SerializeField] Sprite[] sprites2;
    [SerializeField] Sprite[] sprites3;
    [SerializeField] Image pauseMenuBG;

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
                dialogueBox.sprite = sprites1[5];
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
                dialogueBox.sprite = sprites2[5];
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
                dialogueBox.sprite = sprites3[5];
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