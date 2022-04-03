using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BattleManager : MonoBehaviour
{ 
    [HideInInspector]
    public static BattleManager bm;
    [Header("Prefabs")]
    public GameObject cardPrefab;
    public GameObject effectPrefab;
    public GameObject slotPrefab;
    public GameObject aritfactPrefab;
    [Space]
    [Header("Links")]
    public EnemyScript enemyScript;
    public GameObject cardPanel;
    public GameObject slotPanel;
    public GameObject enemieCardPanel;
    public CardShowPanelScript CardShowPanel;
    public HpBarScript HpBar;
    public Image ManaBar;
    public DeathPanelScript deathPanel;

    [HideInInspector]
    public DataHolder dataHolder;

    [HideInInspector]
    public int mana;

    List<SlotScript> slots = new List<SlotScript>();
    Stack<Card> drawPileCards = new Stack<Card>();
    [HideInInspector]
    public List<Card>
        enemyCards = new List<Card>(),
        handCards = new List<Card>(),
        discardPileCards = new List<Card>();
    public List<CardScript> inSlots;


    bool changingTurn = true;

    void Start()
    {
        bm = this;
        dataHolder = FindObjectOfType<DataHolder>();
        enemyScript.StartE(dataHolder.currentEnemy);

        HpBar.EffectPrefab = effectPrefab;

        HpBar.Turn(dataHolder.stats);
        mana = dataHolder.characterManaStr;
        ManaBar.fillAmount = (float)mana / dataHolder.characterManaMax;

        {
            List<Card> d = new List<Card>();
            dataHolder.stats.cards.ForEach(x => d.Add(x.Clone()));
            drawPileCards = new Stack<Card>(Helper.Randomize(d));
        }

        inSlots = new List<CardScript>();
        foreach(Slot s in dataHolder.slots)
        {
            slots.Add(Instantiate(slotPrefab, slotPanel.transform).GetComponent<SlotScript>());
            slots[slots.Count - 1].index = slots.Count - 1;
            slots[slots.Count - 1].slot = s;
            inSlots.Add(null);
        }


        EnemyAI();
    }

    void EnemyAI()
    {
        enemyCards.Add(enemyScript.PickCard());
        enemyCards.ForEach(x => Instantiate(cardPrefab, enemieCardPanel.transform).GetComponent<CardScript>().card = x);
        DrawCards();
    }

    void DrawCards()
    {
        for (int n = 0; n < dataHolder.stats.cardDraw; n++)
            if (drawPileCards.Count > 0)
                handCards.Add(drawPileCards.Pop());
            else
            {
                drawPileCards = new Stack<Card>(Helper.Randomize(discardPileCards));
                handCards.Add(drawPileCards.Pop());
            }
        foreach (Card c in handCards)
        {
            CardScript cardScript;
            (cardScript = Instantiate(cardPrefab, cardPanel.transform).GetComponent<CardScript>()).pickupable = true;
            cardScript.card = c;
            cardScript.handIndex = cardScript.transform.GetSiblingIndex();
        }
        changingTurn = false;
    }

    public void TextTurn()
    {
        if (changingTurn)
            return;
        changingTurn = true;
        if (mana < dataHolder.characterManaMax)
        {
            mana++;
            ManaBar.fillAmount = (float)mana / dataHolder.characterManaMax;
        }
        StartCoroutine(TurnEnding());
    }

    IEnumerator TurnEnding()
    {

        yield return new WaitForSeconds(0.4f);
        for (int n = 0; n < inSlots.Count; n++)
        {
            if (inSlots[n] == null)
                continue;
            yield return new WaitForSeconds(0.4f);
            CardActivationOutcome outcome = inSlots[n].card.Activate(dataHolder.stats, enemyScript.enemy.stats);
            if (!outcome.notAddToDiscard)
                discardPileCards.Add(inSlots[n].card);
            Destroy(inSlots[n].gameObject, 0.1f);
            inSlots[n] = null;
            if (UpdateBars() != 0)
                goto END;
        }

        yield return new WaitForSeconds(0.4f);
        enemyScript.enemy.stats.block = 0;
        if (UpdateBars() != 0)
            goto END;

        foreach (Card c in enemyCards)
        {
            yield return new WaitForSeconds(0.4f);
            c.Activate(enemyScript.enemy.stats, dataHolder.stats);
            Destroy(enemieCardPanel.transform.GetChild(0).gameObject, 0.1f);
            if (UpdateBars() != 0)
                goto END;
        }
        enemyCards = new List<Card>();

        yield return new WaitForSeconds(0.4f);
        for (int n = 0; n < handCards.Count; n++)
        {
            yield return new WaitForSeconds(0.1f);
            discardPileCards.Add(handCards[n]);
            Destroy(cardPanel.transform.GetChild(0).gameObject, 0.1f);
        }
        handCards = new List<Card>();

        dataHolder.stats.block = 0;
        if (UpdateBars() != 0)
            goto END;

        yield return new WaitForSeconds(0.2f);
        foreach (SlotScript s in slots)
            s.hasCard = false;
        
        EnemyAI();
    END:;
    }



    public void UseSkill()
    {
        if (changingTurn)
            return;
        if (mana >= 1)
        {
            mana--;
            ManaBar.fillAmount = (float)mana / dataHolder.characterManaMax;
            for (int n = 0; n < inSlots.Count; n++)
            {
                if (inSlots[n] == null)
                    continue;
                CardActivationOutcome outcome = inSlots[n].card.Activate(dataHolder.stats, enemyScript.enemy.stats);
                if (!outcome.notAddToDiscard)
                    discardPileCards.Add(inSlots[n].card);
                Destroy(inSlots[n].gameObject, 0.1f);
                inSlots[n] = null;
                UpdateBars();
                slots[n].hasCard = false;
                break;
            }
        }
    }

    public int UpdateBars()
    {
        if (dataHolder.stats.hp < 0)
            dataHolder.stats.hp = 0;
        if (enemyScript.enemy.stats.hp < 0)
            enemyScript.enemy.stats.hp = 0;
        enemyScript.hpBar.Turn(enemyScript.enemy.stats);
        HpBar.Turn(dataHolder.stats);
        if (dataHolder.stats.hp <= 0)
        {
            deathPanel.StartCoroutine(deathPanel.Die(""));
            return -1;
        }
        if (enemyScript.enemy.stats.hp <= 0)
        {
            StartCoroutine(Win());
            return 1;
        }
        return 0;
    }

    public IEnumerator noManaAnimation()
    {
        for (float n = 0; n < Mathf.PI; n += 0.01f)
        {
            ManaBar.color = new Color(0.8f + Mathf.Cos(Mathf.PI + n) * 0.2f, 1, 1);
            yield return new WaitForSeconds(0.015f);
        }
    }

    IEnumerator Win()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(2);
    }



    #region Buttons
    public void DrawPileBT()
    {
        if (changingTurn)
            return;
        if (!CardShowPanelScript.active)
            CardShowPanel.Activate(new List<Card>(drawPileCards));
    }
    public void DscardPileBT()
    {
        if (changingTurn)
            return;
        if (!CardShowPanelScript.active)
            CardShowPanel.Activate(discardPileCards);
    }
    #endregion
}
