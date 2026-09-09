using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI_Ctrl : MonoBehaviour
{
    [SerializeField] private GameObject gameObject_UpgradeUI;
    [SerializeField] private Button Button_UpgradeUIOnOff;
    [SerializeField] private Image arrowImage;
    [SerializeField] private Sprite rightArrowSprite;
    [SerializeField] private Sprite leftArrowSprite;


    private void OnEnable()
    {
        if (Button_UpgradeUIOnOff != null)
            Button_UpgradeUIOnOff.onClick.AddListener(OnOffUI);
    }

    public void OnOffUI()
    {
        if(gameObject_UpgradeUI != null)
            gameObject_UpgradeUI.SetActive(!gameObject_UpgradeUI.activeSelf);

        if (gameObject_UpgradeUI.activeSelf)
            arrowImage.sprite = rightArrowSprite;
        else
            arrowImage.sprite = leftArrowSprite;

    }
}
