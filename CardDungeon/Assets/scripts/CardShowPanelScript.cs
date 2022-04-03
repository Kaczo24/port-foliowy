using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardShowPanelScript : MonoBehaviour
{
    public RectTransform panel;
    public GameObject cardPrefab;
    [HideInInspector]
    public static bool active = false;
    List<Card> cards = new List<Card>();
    public void Activate(List<Card> toShow)
    {
        active = true;
        GetComponent<Image>().enabled = true;
        cards = toShow;
        cards.ForEach(x => Instantiate(cardPrefab, panel).GetComponent<CardScript>().card = x);
    }

    public void Deactivate()
    {
        active = false;
        GetComponent<Image>().enabled = false;
        for (int n = 0; n < panel.childCount; n++)
            Destroy(panel.GetChild(n).gameObject);
    }

    void Update()
    {
        if(active)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                Deactivate();
            if(cards.Count > 40)
            {
                if (Input.mouseScrollDelta.y > 0 && panel.anchoredPosition.y > 0)
                    panel.position -= new Vector3(0, 15);

                if (Input.mouseScrollDelta.y < 0 && panel.anchoredPosition.y < (Mathf.Ceil((cards.Count - 40) / 10f) * 245) + 20)
                    panel.position += new Vector3(0, 15);
            }
        }
    }
}
