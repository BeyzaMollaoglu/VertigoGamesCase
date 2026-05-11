using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class ZoneStripManager : MonoBehaviour
{
    [Header("Referanslar")]
    public RectTransform zoneStripContent;
    public RectTransform maskTransform;
    public GameObject zoneItemPrefab;

    [Header("Ayarlar")]
    public float itemWidth = 80f;
    public float spacing = 10f;
    public float animationDuration = 0.5f;

    public void InitStrip(int maxZones)
    {
        foreach (Transform child in zoneStripContent)
        {
            Destroy(child.gameObject);
        }

        float step = itemWidth + spacing;

        for (int i = 1; i <= maxZones; i++)
        {
            GameObject newZone = Instantiate(zoneItemPrefab, zoneStripContent);
            newZone.name = "Zone_" + i;

            RectTransform rt = newZone.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(itemWidth, 80f);

            float posX = ((i - 1) * step) + (itemWidth / 2f);
            rt.anchoredPosition3D = new Vector3(posX, 0, 0);
            rt.localScale = Vector3.one;

            // Yazı Ayarları ve Renklendirme Mantığı
            TextMeshProUGUI textComp = newZone.GetComponentInChildren<TextMeshProUGUI>(true);
            if (textComp != null)
            {
                textComp.gameObject.SetActive(true);
                textComp.enabled = true;
                textComp.text = i.ToString();

                // Varsayılan renk beyaz
                textComp.color = Color.white;

                // 30'un katları sarı (öncelikli)
                if (i % 30 == 0)
                {
                    textComp.color = Color.yellow;
                }
                // 5 veya 10'un katları yeşil
                else if (i % 5 == 0 || i % 10 == 0)
                {
                    textComp.color = Color.green;
                }

                textComp.rectTransform.localScale = Vector3.one;
                textComp.rectTransform.anchoredPosition3D = Vector3.zero;
                textComp.ForceMeshUpdate(true);
            }
        }

        zoneStripContent.sizeDelta = new Vector2(maxZones * step, 80f);
        UpdateStripPosition(1, true);
    }

    public void UpdateStripPosition(int currentZone, bool immediate = false)
    {
        float step = itemWidth + spacing;
        float maskWidth = maskTransform.rect.width;
        float centerOffset = maskWidth / 2f;

        float targetX = centerOffset - ((currentZone - 1) * step) - (itemWidth / 2f);

        zoneStripContent.DOKill();
        if (immediate)
        {
            zoneStripContent.anchoredPosition = new Vector2(targetX, 0);
        }
        else
        {
            zoneStripContent.DOAnchorPosX(targetX, animationDuration).SetEase(Ease.OutCubic);
        }
    }
}