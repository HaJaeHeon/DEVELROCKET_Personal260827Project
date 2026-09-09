using UnityEngine;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(GameStart);
        loadButton.onClick.AddListener(LoadGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void GameStart()
    {
        DataManager.Instance.ClickStartInitData();
        GameManager manager;
        if (manager = GameObject.FindAnyObjectByType<GameManager>())
        {
            GameManager.Instance.gameDatas = DataManager.Instance.currentDatas;
        }
        LoadingManager.Instance.LoadSceneWithLoading("GameScene");
    }

    private void LoadGame()
    {
        DataManager.Instance.ClickLoadData();
        GameManager manager;
        if (manager = GameObject.FindAnyObjectByType<GameManager>())
        {
            GameManager.Instance.gameDatas = DataManager.Instance.currentDatas;
        }
        LoadingManager.Instance.LoadSceneWithLoading("GameScene");
    }
    
    private void ExitGame()
    {
        LoadingManager.Instance.QuitGame();
    }
}
