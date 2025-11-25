using TMPro;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI endingText;
    [SerializeField] GameObject fade;
    [SerializeField] GameManager gameManager;

    void Start()
    {
        fade.SetActive(true);
        StartCoroutine(gameManager.FadeOut(fade, 51));
        Invoke(nameof(Necessary), 0.51f);
        endingText.text = "Ending: " + PlayerPrefs.GetInt("_ending") + " of 4";
        switch (PlayerPrefs.GetInt("_ending"))
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 0:
                break;
        }
    }
    void Necessary()
    {
        fade.SetActive(false);
    }
}