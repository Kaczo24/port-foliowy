using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeathPanelScript : MonoBehaviour
{
    public Image th;
    public TextMeshProUGUI deathInfo;
    public TextMeshProUGUI deathMessage;
    public Button button;

    public IEnumerator Die(string text)
    {
        yield return new WaitForSeconds(0.2f);
        th.enabled = true;
        deathMessage.gameObject.SetActive(true);
        deathMessage.text = text;
        deathInfo.gameObject.SetActive(true);
        button.gameObject.SetActive(true);
        button.interactable = false;
        for (float n = 0; n < 1.2; n += 0.01f)
        {
            th.color = new Color(0.15f, 0.15f, 0.15f, n);
            deathMessage.color = new Color(0.9f, 0.9f, 0.9f, n - 0.2f);
            deathInfo.color = new Color(0.9f, 0.9f, 0.9f, n - 0.2f);
            ColorBlock c = button.colors;
            c.normalColor = new Color(0.9f, 0.9f, 0.9f, n - 0.2f);
            c.disabledColor = new Color(0.9f, 0.9f, 0.9f, n - 0.2f);
            button.colors = c;
            yield return new WaitForSeconds(0.015f);
        }
        button.interactable = true;
    }

}
