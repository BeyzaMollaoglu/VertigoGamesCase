using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpinSlot : MonoBehaviour
{
    public Image ui_image_reward_icon;
    public TextMeshProUGUI ui_text_reward_amount_value;
    private RewardData _currentData;

    public void SetSlot(RewardData data, int multiplier)
    {
        _currentData = data;
        ui_image_reward_icon.sprite = data.rewardIcon;
        ui_image_reward_icon.preserveAspect = true;

        if (data.isDeath) ui_text_reward_amount_value.text = "";
        else ui_text_reward_amount_value.text = "x" + (data.baseAmount * multiplier).ToString();
    }

    public RewardData GetCurrentData() => _currentData;
}