using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public Enemy enemy;
    public HpBarScript hpBar;
    Queue<Card> cards = new Queue<Card>();


    public void StartE(Enemy e)
    {
        enemy = e.Clone();
        enemy.stats.hp = e.stats.HPmax;
        hpBar.Turn(enemy.stats);
    }

    public Card PickCard()
    {
        while (cards.Count < 1)
            if (string.IsNullOrWhiteSpace(enemy.specialAI))
                cards.Enqueue(Helper.Pick(enemy.stats.cards));
            else
            {
                if (enemy.AIc.Count == 0 && enemy.chances.Count == 0)
                {
                    new List<string>(enemy.specialAI.Split(new char[] { '\n' , '\r'})).ForEach(x => enemy.AIc.Add(x.Split(' ')));
                    for (int n = enemy.AIc.Count - 1; n >= 0; n--)
                        if(enemy.AIc[n][0] == "%")
                        {
                            enemy.chances.Add(new Vector2Int(int.Parse(enemy.AIc[n][1]), int.Parse(enemy.AIc[n][2])));
                            enemy.AIc.RemoveAt(n);
                        }
                    

                }
                AddIndex(PickIndex(), true);
            }


        return cards.Dequeue();
    }

    void AddIndex(int i, bool prevHandle)
    {
        if (enemy.AIc.Count != 0)
        {
            
            string iS = i.ToString();
            List<string[]> rules = enemy.AIc.FindAll(x => x[0] == iS);
            if (rules.Count != 0)
            {
                if(prevHandle)
                    rules.FindAll(x => x[1] == "<").ForEach(x => cards.Enqueue(enemy.stats.cards[int.Parse(x[2])]));
                cards.Enqueue(enemy.stats.cards[i]);
                if (prevHandle)
                    rules.FindAll(x => x[1] == ">").ForEach(x => AddIndex(int.Parse(x[2]), false));

                List<int> forbidens = rules.FindAll(x => x[1] == "!").ConvertAll(x => int.Parse(x[2]));

                if (forbidens.Count > 0)
                {
                    int c = cards.Count;
                    Card[] cs = cards.ToArray();

                Forbids:
                    AddIndex(PickIndex(), true);
                    if (forbidens.Contains(enemy.stats.cards.FindIndex(x => x == cs[c])))
                    {
                        Stack<Card> css = new Stack<Card>(cs);
                        while (css.Count > c)
                            css.Pop();
                        cards = new Queue<Card>(css);
                        goto Forbids;
                    }
                }
            }
            else
                cards.Enqueue(enemy.stats.cards[i]);
        }
        else
            cards.Enqueue(enemy.stats.cards[i]);
    }

    int PickIndex()
    {
        int i = -1;
        if (enemy.chances.Count != 0)
        {
            int lower = 0;
            float rv = Random.value * 100;
            foreach (Vector2Int x in enemy.chances)
            {
                if (rv >= lower && rv < lower + x.x)
                {
                    i = x.y;
                    break;
                }
                lower += x.x;
            }
        }
        if (i == -1)
            do { i = Random.Range(0, enemy.stats.cards.Count); } while (enemy.chances.FindIndex(x => x.y == i) != -1);
        return i;
    }
}

/*
    KOLEJNOŚĆ WAŻNA
    0 < 1 - przed 0 musi 1
    0 > 1 - po 0 musi 1
    0 ! 1 - po 0 nie 1

    % 90 0 - 90% na 0
*/