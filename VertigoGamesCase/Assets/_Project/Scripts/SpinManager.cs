using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class SpinManager : MonoBehaviour
{
    [Header("Managers")]
    public ZoneStripManager zoneStripManager;

    [Header("Zone Settings")]
    public int currentZone = 1;
    public int maxZoneCount = 120;
    public List<RewardData> rewardPool;
    public RewardData deathData;

    [Header("Indicator Graphics")]
    public Image ui_image_indicator;
    public Sprite bronzeIndicatorSprite, silverIndicatorSprite, goldIndicatorSprite;

    [Header("Spin Graphics")]
    public Image ui_image_spin_main;
    public Sprite bronzeSpinSprite, silverSpinSprite, goldSpinSprite;

    [Header("UI References")]
    public List<SpinSlot> ui_spin_slots;
    public Button ui_button_spin_execute;

    private bool isSpinning = false;

    private void Start()
    {
        if (zoneStripManager != null)
            zoneStripManager.InitStrip(maxZoneCount);

        ui_button_spin_execute.onClick.AddListener(ExecuteSpin);
        SetupSpinWheel();
    }

    public void ExecuteSpin()
    {
        if (isSpinning) return;
        isSpinning = true;

        int randomSlotIndex = Random.Range(0, 8);
        float targetAngle = (360f * 5f) + (randomSlotIndex * 45f);

        ui_image_spin_main.transform.DORotate(new Vector3(0, 0, -targetAngle), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                isSpinning = false;
                currentZone++;

                if (zoneStripManager != null)
                    zoneStripManager.UpdateStripPosition(currentZone);

                SetupSpinWheel();
            });
    }

    public void SetupSpinWheel()
    {
        SpinType currentRarity;
        int multiplier = 1;

        if (currentZone % 30 == 0) { currentRarity = SpinType.Gold; multiplier = 10; ui_image_spin_main.sprite = goldSpinSprite; ui_image_indicator.sprite = goldIndicatorSprite; }
        else if (currentZone % 5 == 0) { currentRarity = SpinType.Silver; multiplier = 5; ui_image_spin_main.sprite = silverSpinSprite; ui_image_indicator.sprite = silverIndicatorSprite; }
        else { currentRarity = SpinType.Bronze; multiplier = 1; ui_image_spin_main.sprite = bronzeSpinSprite; ui_image_indicator.sprite = bronzeIndicatorSprite; }

        List<RewardData> availablePool = rewardPool.Where(x => (x.rarity == currentRarity || x.rarity == SpinType.All) && !x.isDeath).ToList();
        int countToTake = Mathf.Min(availablePool.Count, 8);
        List<RewardData> selectedRewards = availablePool.OrderBy(x => Random.value).Take(countToTake).ToList();

        if (currentRarity == SpinType.Bronze)
        {
            int deathIndex = Random.Range(0, Mathf.Min(selectedRewards.Count, 8));
            selectedRewards[deathIndex] = deathData;
        }

        for (int i = 0; i < ui_spin_slots.Count; i++)
        {
            if (i < selectedRewards.Count) ui_spin_slots[i].SetSlot(selectedRewards[i], multiplier);
        }
    }
}