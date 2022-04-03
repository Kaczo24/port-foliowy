using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public Stats stats = new Stats();
    public Texture2D texture;
    [Space]
    [TextArea]
    public string specialAI;
    //[HideInInspector]
    public List<string[]> AIc = new List<string[]>();
    public List<Vector2Int> chances = new List<Vector2Int>();

    public Enemy Clone()
    {
        Enemy e = new Enemy();
        e.stats.cards = stats.cards;
        e.stats.cards.ForEach(x => x = x.Clone());
        Effect[] eff = new Effect[stats.effects.Count];
        stats.effects.CopyTo(eff);
        e.stats.effects = new List<Effect>(eff);
        e.stats.HPmax = stats.HPmax;
        e.texture = texture;
        e.specialAI = specialAI;
        return e;
    }
}

