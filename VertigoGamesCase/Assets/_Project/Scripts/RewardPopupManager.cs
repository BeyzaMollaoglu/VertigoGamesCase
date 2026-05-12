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

    [Header("Left Panel Settings")]
    public Transform leftPanelContent;
    public GameObject rewardItemPrefab;
    private Sprite _lastEarnedSprite;
    private string _lastAmount;

    public CollectedItem collectedItem;

    private void Start()
    {
        popupPanel.SetActive(false);
        claimButton.onClick.AddListener(OnClaimButtonClicked);
    }

    public void ShowReward(Sprite icon, string amount)
    {
        _lastEarnedSprite = icon;
        _lastAmount = amount;

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

    public void OnClaimButtonClicked()
    {
        if (_lastEarnedSprite == null) return;

        // 1. Önce objeyi oluştur ve ayarlarını yap
        GameObject spawnedItem = Instantiate(rewardItemPrefab, transform.parent);
        CollectedItem itemScript = spawnedItem.GetComponent<CollectedItem>();

        if (itemScript != null)
        {
            itemScript.iconImage.sprite = _lastEarnedSprite;
            itemScript.iconImage.preserveAspect = true;
            itemScript.amountText.text = rewardAmountText.text;
        }

        RectTransform itemRect = spawnedItem.GetComponent<RectTransform>();
        itemRect.position = rewardIcon.transform.position;

        // Paneli hemen kapat
        popupPanel.SetActive(false);

        // 2. Animasyon dizisi (Sequence) oluştur
        Sequence collectSequence = DOTween.Sequence();

        collectSequence.Append(itemRect.DOMove(leftPanelContent.position, 0.8f).SetEase(Ease.InQuart));
        collectSequence.Join(itemRect.DOScale(Vector3.zero, 0.8f).SetEase(Ease.InQuart)); // Giderken küçülmesini sağlar

        collectSequence.OnComplete(() =>
        {
            // false parametresi objenin panelin pivot/scale ayarlarına hemen uymasını sağlar
            spawnedItem.transform.SetParent(leftPanelContent, false);

            // Scale ve Rotation sıfırlama
            spawnedItem.transform.localScale = Vector3.one;
            spawnedItem.transform.localRotation = Quaternion.identity;

            // Layout'u anında hesaplaması için zorla
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(leftPanelContent.GetComponent<RectTransform>());
        });
    }
}