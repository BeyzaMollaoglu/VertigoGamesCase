using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;

public class SpinManager : MonoBehaviour
{
    [Header("Zone Settings")]
    public int currentZone = 1;
    public List<RewardData> rewardPool;
    public RewardData deathData;

    [Header("Spin Graphics")]
    public Image ui_image_spin_main;
    public Sprite bronzeSpinSprite;
    public Sprite silverSpinSprite;
    public Sprite goldSpinSprite;

    [Header("UI References")]
    public List<SpinSlot> ui_spin_slots;
    public Button ui_button_spin_execute;
    public TextMeshProUGUI ui_text_zone_number_value;

    private bool isSpinning = false;

    private void Start()
    {
        ui_button_spin_execute.onClick.AddListener(ExecuteSpin);
        SetupSpinWheel();
    }

    public void SetupSpinWheel()
    {
        SpinType currentRarity;
        int multiplier = 1;

        if (currentZone % 30 == 0) { currentRarity = SpinType.Gold; multiplier = 10; ui_image_spin_main.sprite = goldSpinSprite; }
        else if (currentZone % 5 == 0) { currentRarity = SpinType.Silver; multiplier = 5; ui_image_spin_main.sprite = silverSpinSprite; }
        else { currentRarity = SpinType.Bronze; multiplier = 1; ui_image_spin_main.sprite = bronzeSpinSprite; }

        ui_text_zone_number_value.text = currentZone.ToString();

        List<RewardData> availablePool = rewardPool.Where(x => (x.rarity == currentRarity || x.rarity == SpinType.All) && !x.isDeath).ToList();
        List<RewardData> selectedRewards = availablePool.OrderBy(x => Random.value).Take(8).ToList();

        if (currentRarity == SpinType.Bronze)
        {
            int deathIndex = Random.Range(0, 8);
            selectedRewards[deathIndex] = deathData;
        }

        for (int i = 0; i < ui_spin_slots.Count; i++)
        {
            ui_spin_slots[i].SetSlot(selectedRewards[i], multiplier);
        }
    }

    public void ExecuteSpin()
    {
        if (isSpinning) return;
        isSpinning = true;

        int randomSlotIndex = Random.Range(0, 8);
        float targetAngle = 360f * 5f + (randomSlotIndex * 45f);

        ui_image_spin_main.transform.DORotate(new Vector3(0, 0, -targetAngle), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                isSpinning = false;
                Debug.Log("Kazandığın Slot: " + randomSlotIndex);
            });
    }
}