using UnityEngine;

public enum SpinType { Bronze, Silver, Gold, All }

[CreateAssetMenu(fileName = "New Reward", menuName = "VertigoCase/Reward")]
public class RewardData : ScriptableObject
{
    public string rewardName;
    public Sprite rewardIcon;
    public int baseAmount;
    public bool isDeath;
    public SpinType rarity;
}