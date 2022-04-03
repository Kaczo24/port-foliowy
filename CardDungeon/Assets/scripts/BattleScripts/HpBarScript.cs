using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HpBarScript : MonoBehaviour
{
    [HideInInspector]
    public GameObject EffectPrefab;
    public Image image;
    public TextMeshProUGUI text;
    public Transform effectPanel;
    public GameObject Shield;
    public TextMeshProUGUI shieldText;

    Color red = new Color(0.754717f, 0, 0);
    Color blue = new Color(0.2666667f, 0.3333333f, 0.4666667f);

    public void Turn(Stats stats)
    {
        image.fillAmount = (float)stats.hp / stats.HPmax;
        text.text = stats.hp + " / " + stats.HPmax;
        if (stats.block > 0)
        {
            Shield.SetActive(true);
            image.color = blue;
            shieldText.text = stats.block.ToString();
        }
        else
        {
            Shield.SetActive(false);
            image.color = red;
        }
    }
}
