using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Element { neutral, fire, ice, magic}
public enum Rarity { Starting, Common, Uncomon, Rare, Legendary}
public enum Slot { Normal, Frozen, Temproary, Single}

public struct CardActivationOutcome
{
    public bool notAddToDiscard;
}

[CreateAssetMenu(menuName = "Card", fileName = "NewCard")]
public class Card : ScriptableObject
{
    //public string Name { get { return this.name; } set { name = value; } }
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionU;
    public Texture2D image;
    public Element element;
    public Rarity rarity;
    public bool upgrade = false;
    [Space]
    public bool isSkill = false;
    public int manaCost;
    [Space]
    public BasicValues normal;
    public BasicValues upgraded;

    [Space]
    public int SpecialActionIndex = -1;


    public CardActivationOutcome Activate(Stats user, Stats target)
    {
        CardActivationOutcome outcome = new CardActivationOutcome();
        BasicValues bv;
        if (upgrade)
            bv = upgraded;
        else
            bv = normal;
        user.block += bv.block;


        target.block -= bv.damage;
        if (target.block < 0)
        {
            target.hp += target.block;
            target.block = 0;
        }

        if(bv.burnThis)
            outcome.notAddToDiscard = true;

        switch (SpecialActionIndex)
        {
            case 0:
                BattleManager.bm.dataHolder.stats.hp += upgrade ? 3 : 5;
                break;
            default:
                break;
        }
        BattleManager.bm.UpdateBars();
        return outcome;
    }


    public Card Clone()
    {
        Card o = new Card();
        o.name = name;
        o.description = description;
        o.descriptionU = descriptionU;
        o.image = image;
        o.element = element;
        o.rarity = rarity;
        o.upgrade = upgrade;
        o.normal = normal;
        o.upgraded = upgraded;
        o.isSkill = isSkill;
        o.manaCost = manaCost;
        o.SpecialActionIndex = SpecialActionIndex;
        return o;
    }


    public static Dictionary<Element, Color> ElemColDic = new Dictionary<Element, Color>()
    {
        { Element.neutral, Color.gray },
        { Element.fire, new Color(0.9f, 0.4f, 0) },
        { Element.ice, new Color(0.4f, 0.6f, 1) },
        { Element.magic, new Color(0.393f, 0.123f, 0.503f) }
    };
    public static Dictionary<Rarity, Color> RareColorDic = new Dictionary<Rarity, Color>()
    {
        { Rarity.Starting, new Color(0.4f, 0.4f, 0.4f) },
        { Rarity.Common, new Color(0.4f, 0.4f, 0.4f) },
        { Rarity.Uncomon, new Color(0.6f, 0.85f, 0.95f) },
        { Rarity.Rare, new Color(1, 0, 0) },
        { Rarity.Legendary, new Color(1, 0.802f, 0) }
    };
    [System.Serializable]
    public struct BasicValues
    {
        public int damage, block;
        public bool burnThis;
    }
}
