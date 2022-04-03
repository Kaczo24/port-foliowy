using UnityEngine;
using System.Collections;

[System.Serializable]
public class Dialog
{
    [TextArea]
    public string[] texts;
    public Option[] options;


    [System.Serializable]
    public struct Option
    {
        public string text;
        [Space]
        public Outcome[] outcomes;
        [Space(20)]
        public int moneyR;
        public int hpR;
    }

    [System.Serializable]
    public struct Outcome
    {
        public Dialog dialog;
        [Space]
        [TextArea]
        public string deathMessage;
        [Space]
        public int Dhp;
        public int Dmoney;
        public Pool specialCardPool;
        public Enemy SpecialnemyFigth;
        public bool stdFightLoot;
    }
}
