using UnityEngine;

public class GameManager : MonoBehaviour
{
    Inputs input;

    [SerializeField] GameObject pauseMenu;

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
        
    }
    void Update()
    {
        MenuCheck();
    }
    void MenuCheck()
    {
        if (input.Player.Pause.WasPressedThisFrame())
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }
}