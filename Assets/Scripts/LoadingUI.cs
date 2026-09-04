using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Slider loadingBar;
    public TMP_Text loadingText;

    private void Start()
    {
        string target = LoadingManager.Instance.targetSceneName;

        StartCoroutine(LoadSceneProcess(target));
    }

    private IEnumerator LoadSceneProcess(string target)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingBar.value = progress;
            loadingText.text = $"{progress * 100:0}%";

            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}