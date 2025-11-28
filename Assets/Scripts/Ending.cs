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
            case 1: //* She said yes, they stayed awake.
                break;
            case 2: //* She said no, they stayed awake.
                break;
            case 3: //* She said yes, they slept.
                break;
            case 4: //* She said no, they slept.
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