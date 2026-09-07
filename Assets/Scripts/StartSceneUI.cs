using UnityEngine;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(GameStart);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void GameStart()
    {
        LoadingManager.Instance.LoadSceneWithLoading("GameScene");
    }
    
    private void ExitGame()
    {
        LoadingManager.Instance.QuitGame();
    }
}
