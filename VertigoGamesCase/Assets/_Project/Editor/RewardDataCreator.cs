using UnityEngine;
using UnityEditor;
using System.IO;

public class RewardDataCreator
{
    [MenuItem("Tools/Reward Data Olustur")]
    public static void CreateRewardData()
    {
        Object[] objects = Selection.objects;
        string targetFolder = "Assets/_project/Resources/ScriptableObjects";

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            AssetDatabase.Refresh();
        }

        foreach (Object obj in objects)
        {
            Sprite targetSprite = null;

            if (obj is Sprite)
            {
                targetSprite = (Sprite)obj;
            }
            else if (obj is Texture2D)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                targetSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            if (targetSprite != null)
            {
                RewardData newData = ScriptableObject.CreateInstance<RewardData>();
                newData.rewardName = targetSprite.name;
                newData.rewardIcon = targetSprite;
                newData.baseAmount = 10;
                newData.isDeath = false;
                newData.rarity = SpinType.Bronze;

                string assetPath = Path.Combine(targetFolder, targetSprite.name + ".asset");
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                AssetDatabase.CreateAsset(newData, assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}