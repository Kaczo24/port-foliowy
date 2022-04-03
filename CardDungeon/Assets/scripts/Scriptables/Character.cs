using UnityEngine;
using System.Collections.Generic;

public enum EChracter { main }

[CreateAssetMenu(menuName = "Character", order = 6)]
public class Character : ScriptableObject
{
    public int maxHp, cardDraw, manaMax;
    public List<Card> cards = new List<Card>();
    public List<Slot> slots = new List<Slot>();
    public List<Artifact> artifacts = new List<Artifact>();
    public EChracter eChracter;
}
