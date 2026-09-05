using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingUI : MonoBehaviour
{
    public Slider loadingBar;
    public TMP_Text loadingText;
    public float minLoadTime = 2f;
    public float fillSpeed = 1f;
    public Image diceImage;
    public float jumpPower = 100f;

    private void Start()
    {
        string target = LoadingManager.Instance.targetSceneName;

        if (!string.IsNullOrEmpty(target))
        {
            StartCoroutine(LoadSceneProcess(target));
        }
    }

    private IEnumerator LoadSceneProcess(string target)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        float timer = 0f;
        loadingBar.value = 0f;
        loadingText.text = "0%";

        Vector3 startPosition = diceImage.transform.localPosition;

        diceImage.transform.DOLocalJump(startPosition, jumpPower, 1, 1f)
            .SetLoops(-1, LoopType.Restart).SetLink(diceImage.gameObject);
        
        diceImage.transform.DORotate(new Vector3(0f, 0f, 720f),1f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart).SetLink(diceImage.gameObject);
        
        
        while (!op.isDone)
        {
            yield return null;

            
            timer += Time.deltaTime;

            float fakeProgrress = timer / minLoadTime;
            float realProgress = op.progress / 0.9f;
            
            float targetProgress = Mathf.Min(fakeProgrress, realProgress);
            
            loadingBar.value = Mathf.Lerp(loadingBar.value, targetProgress, fillSpeed * Time.deltaTime);
            loadingText.text = $"{loadingBar.value * 100:0}%";

            if (op.progress >= 0.9f && loadingBar.value >= 0.99f)
            {
                loadingBar.value = 1f;
                loadingText.text = "100%";

                yield return new WaitForSeconds(1f);
                
                op.allowSceneActivation = true;
            }
        }
    }
}