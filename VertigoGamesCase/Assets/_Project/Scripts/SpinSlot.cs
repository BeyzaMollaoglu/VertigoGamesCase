using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpinSlot : MonoBehaviour
{
    public Image ui_image_reward_icon;
    public TextMeshProUGUI ui_text_reward_amount_value;

    public void SetSlot(RewardData data, int multiplier)
    {
        ui_image_reward_icon.sprite = data.rewardIcon;

        if (data.isDeath)
        {
            ui_text_reward_amount_value.text = "";
        }
        else
        {
            ui_text_reward_amount_value.text = "x" + (data.baseAmount * multiplier).ToString();
        }
    }
}