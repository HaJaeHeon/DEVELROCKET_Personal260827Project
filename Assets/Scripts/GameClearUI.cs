using UnityEngine;
using UnityEngine.UI;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        mainMenuButton.onClick.AddListener(GameClear);
        exitButton.onClick.AddListener(ExitMenu);
    }
    private void OnDisable()
    {
        mainMenuButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void GameClear()
    {
        LoadingManager.Instance.LoadSceneWithLoading("StartScene");
    }

    private void ExitMenu()
    {
        LoadingManager.Instance.QuitGame();
    }
}
