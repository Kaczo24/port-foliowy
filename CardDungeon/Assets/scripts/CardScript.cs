using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class CardScript : MonoBehaviour
{
    public Card card;
    public int handIndex;
    public bool pickupable = false;
    Transform panel;
    Vector3 mousePlace;
    bool isFollowing = false, toDeattach = false, toActivate = false;
    SlotScript currentSlot = null;
    static int selectionTime = 0, cardNumber = 0;
    void Start()
    {
        transform.Find("BG").GetComponent<RawImage>().color = Card.ElemColDic[card.element];
        transform.Find("NameBG").GetComponent<RawImage>().color = Card.RareColorDic[card.rarity];

        cardNumber++;

        TextMeshProUGUI tmpname = transform.Find("NameBG").Find("Name (TMP)").GetComponent<TextMeshProUGUI>();
        if (card.upgrade)
        {
            tmpname.fontStyle = FontStyles.Bold;
            tmpname.text = card.name + "+";

            transform.Find("TextBg").Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = card.descriptionU;
        }
        else
        {
            tmpname.text = card.name;

            transform.Find("TextBg").Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = card.description;
        }


        if (card.image != null)
            transform.Find("Image").GetComponent<RawImage>().texture = card.image;

        if (pickupable)
            panel = transform.parent;
    }

    void OnDestroy()
    {
        cardNumber--;
    }
    void Update()
    {
        if (selectionTime > 0)
            selectionTime--;
        if (toActivate && Input.GetMouseButtonUp(0))
        {
            if (BattleManager.bm.mana >= card.manaCost)
            {
                CardActivationOutcome outcome = card.Activate(DataHolder.dataHolder.stats, BattleManager.bm.enemyScript.enemy.stats);
                BattleManager.bm.handCards.Remove(card);
                if (!outcome.notAddToDiscard)
                    BattleManager.bm.discardPileCards.Add(card);

                BattleManager.bm.mana -= card.manaCost;
                BattleManager.bm.ManaBar.fillAmount = (float)BattleManager.bm.mana / DataHolder.dataHolder.characterManaMax;
                Destroy(gameObject, 0.1f);
            }
            else
            {
                toActivate = false;
                StopFollowing();
                BattleManager.bm.StartCoroutine(BattleManager.bm.noManaAnimation());
            }
        }

        if (pickupable)
        {
            if (toDeattach)
            {
                Deattach();
                //toDeattach = false;
            }

            if (isFollowing)
                ((RectTransform)transform).position = Input.mousePosition + mousePlace;

            if (!isFollowing && selectionTime == 0 && pickupable && transform.parent != panel)
            {

                Attach();
            }


            if (transform.localPosition.x < -((RectTransform)transform.parent).rect.width / 2)
                transform.localPosition = new Vector3(-((RectTransform)transform.parent).rect.width / 2, transform.localPosition.y);
            if (transform.localPosition.x > ((RectTransform)transform.parent).rect.width / 2)
                transform.localPosition = new Vector3(((RectTransform)transform.parent).rect.width / 2, transform.localPosition.y);

            if (transform.localPosition.y < -((RectTransform)transform.parent).rect.height / 2)
                transform.localPosition = new Vector3(transform.localPosition.x, -((RectTransform)transform.parent).rect.height / 2);
            if (transform.localPosition.y > ((RectTransform)transform.parent).rect.height / 2)
                transform.localPosition = new Vector3(transform.localPosition.x, ((RectTransform)transform.parent).rect.height / 2);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (card.isSkill)
        {
            if (other.tag == "Player")
                toActivate = true;
        }
        else if (other.TryGetComponent(out currentSlot))
            if (currentSlot.hasCard)
                currentSlot = null;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        currentSlot = null;
        toActivate = false;
    }
    public void Deattach()
    {
        toDeattach = true;
        if (pickupable && !CardShowPanelScript.active && selectionTime == 0)
        {
            transform.localScale = new Vector3(1.2f, 1.2f);
            selectionTime = -1;
            toDeattach = false;
        }
    }
    private void Attach()
    {
        toDeattach = false;
        if (pickupable && !CardShowPanelScript.active && !isFollowing)
        {
            transform.localScale = new Vector3(1f, 1f);
            selectionTime = cardNumber;
        }
    }
    public void Follow()
    {
        if (pickupable && !CardShowPanelScript.active && !isFollowing)
        {
            transform.SetParent(transform.parent.parent);
            if (selectionTime != -1)
                Deattach();
            //transform.localScale = new Vector3(1f, 1f);
            mousePlace = ((RectTransform)transform).position - Input.mousePosition;
            isFollowing = true;
        }

    }
    public void StopFollowing()
    {
        if (Input.GetMouseButton(0))
            return;
        if (toActivate)
            return;
        toDeattach = false;
        //transform.localScale = new Vector3(1f, 1f);
        if (pickupable && !CardShowPanelScript.active)
        {
            isFollowing = false;
            if (currentSlot == null)
            {
                transform.SetParent(panel);
                transform.SetSiblingIndex(handIndex);
                Attach();
            }
            else
            {
                transform.SetParent(currentSlot.transform);
                transform.localPosition = new Vector3(0, 0);

                BattleManager.bm.inSlots[currentSlot.index] = this;
                BattleManager.bm.handCards.Remove(card);
                currentSlot.hasCard = true;

                pickupable = false;
                selectionTime = 0;
            }
        }
    }
}
