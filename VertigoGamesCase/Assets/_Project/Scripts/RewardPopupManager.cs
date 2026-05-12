using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

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
    private Dictionary<string, CollectedItem> _activeRewards = new Dictionary<string, CollectedItem>();

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

        popupPanel.SetActive(false);

        if (_activeRewards.ContainsKey(_lastEarnedSprite.name))
        {
            UpdateExistingReward(_activeRewards[_lastEarnedSprite.name]);
        }
        else
        {
            CreateNewReward();
        }
    }

    private void CreateNewReward()
    {
        GameObject spawnedItem = Instantiate(rewardItemPrefab, transform.parent);
        CollectedItem itemScript = spawnedItem.GetComponent<CollectedItem>();

        itemScript.iconImage.sprite = _lastEarnedSprite;
        itemScript.iconImage.preserveAspect = true;
        itemScript.amountText.text = _lastAmount;

        _activeRewards.Add(_lastEarnedSprite.name, itemScript);

        AnimateItem(itemScript.GetComponent<RectTransform>(), true);
    }

    private void UpdateExistingReward(CollectedItem existingItem)
    {
        int currentAmount = ParseAmount(existingItem.amountText.text);
        int addedAmount = ParseAmount(_lastAmount);
        string newTotal = "x" + (currentAmount + addedAmount);

        GameObject dummy = Instantiate(rewardItemPrefab, transform.parent);
        dummy.GetComponent<CollectedItem>().iconImage.sprite = _lastEarnedSprite;
        dummy.GetComponent<CollectedItem>().amountText.text = "";

        existingItem.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 1);

        AnimateItem(dummy.GetComponent<RectTransform>(), false, existingItem.transform, newTotal, existingItem);
    }

    private void AnimateItem(RectTransform itemRect, bool isNew, Transform targetPos = null, string amountToSet = "", CollectedItem itemToUpdate = null)
    {
        itemRect.position = rewardIcon.transform.position;
        Vector3 targetWorldPos = isNew ? leftPanelContent.position : targetPos.position;

        Sequence s = DOTween.Sequence();
        s.Append(itemRect.DOMove(targetWorldPos, 0.8f).SetEase(Ease.InQuart));
        s.Join(itemRect.DOScale(isNew ? Vector3.one : Vector3.zero, 0.8f).SetEase(Ease.InQuart));

        s.OnComplete(() =>
        {
            if (isNew)
            {
                itemRect.SetParent(leftPanelContent, false);
                itemRect.localScale = Vector3.one;
                itemRect.localRotation = Quaternion.identity;
                LayoutRebuilder.ForceRebuildLayoutImmediate(leftPanelContent.GetComponent<RectTransform>());
            }
            else
            {
                if (itemToUpdate != null) itemToUpdate.amountText.text = amountToSet;
                Destroy(itemRect.gameObject);
            }
        });
    }

    private int ParseAmount(string text)
    {
        string clean = text.Replace("x", "").Trim();
        int.TryParse(clean, out int result);
        return result;
    }
}