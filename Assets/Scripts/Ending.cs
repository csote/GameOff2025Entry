using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI endingText;
    [SerializeField] TextMeshProUGUI flavourText;
    [SerializeField] GameObject endText;
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
                flavourText.text = "And they lived happily ever after.";
                break;
            case 2:
                flavourText.text = "What is her problem?";
                break;
            case 3:
                flavourText.text = "It's almost like you need to do the opposite of that.";
                break;
            case 4:
                flavourText.text = "Ouch.";
                break;
            case 0:
                break;
        }
        StartCoroutine(GoAway());
    }
    void Necessary()
    {
        fade.SetActive(false);
    }
    IEnumerator GoAway()
    {
        yield return new WaitForSeconds(5);
        fade.SetActive(true);
        StartCoroutine(gameManager.FadeIn(fade, 51));
        yield return new WaitUntil(() => fade.GetComponent<Image>().color.a >= 1);
        yield return new WaitForSeconds(2);
        StartCoroutine(gameManager.FadeIn(endText, 255, 255, false));
    }
}