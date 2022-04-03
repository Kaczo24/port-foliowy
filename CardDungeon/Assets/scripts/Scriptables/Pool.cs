using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Pool")]
public class Pool : ScriptableObject
{
    public List<Enemy> enemies = new List<Enemy>();
    public List<Card> cards = new List<Card>();
    public List<GameEvent> events = new List<GameEvent>();
}
