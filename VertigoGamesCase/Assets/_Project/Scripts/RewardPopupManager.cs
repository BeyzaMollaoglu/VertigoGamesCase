using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class RewardPopupManager : MonoBehaviour
{
    [Header("UI Panelleri")]
    public GameObject popupPanel;
    public RectTransform mainContainer;
    public Image rewardIcon;
    public TextMeshProUGUI rewardAmountText;
    public Button claimButton;

    [Header("VFX Objeleri")]
    public RectTransform starFlash;
    public RectTransform starGlow;

    [Header("Ayarlar")]
    public float animationDuration = 0.5f;

    private void Start()
    {
        popupPanel.SetActive(false);
        claimButton.onClick.AddListener(OnClaimClick);
    }

    public void ShowReward(Sprite icon, string amount)
    {
        rewardIcon.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        rewardIcon.preserveAspect = true;

        rewardIcon.sprite = icon;
        rewardAmountText.text = amount;

        popupPanel.SetActive(true);
        mainContainer.localScale = Vector3.zero;

        mainContainer.DOKill();
        mainContainer.DOScale(1f, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);

        starFlash.DORotate(new Vector3(0, 0, 360), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void OnClaimClick()
    {
        mainContainer.DOScale(0f, animationDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            popupPanel.SetActive(false);
            starFlash.DOKill();
            starGlow.DOKill();
        });
    }
}