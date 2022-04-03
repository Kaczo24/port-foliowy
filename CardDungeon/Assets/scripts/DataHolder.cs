using UnityEngine;
using System.Collections.Generic;

public enum Location
{
    AshPlains,
    Hell,
    Taiga,
    IceBergs,
    Swamp,
    DarkForest,
    End
}

[System.Serializable]
public class DataHolder : MonoBehaviour
{
    public static DataHolder dataHolder;
    [Space]
    public Stats stats = new Stats();
    public List<Slot> slots = new List<Slot>();
    public List<Artifact> artifacts = new List<Artifact>();
    public int characterManaMax, characterManaStr, money;
    #region BattlePools
    [Space]
    public Pool AshPlains1Battle;
    public Pool 
        AshPlains2Battle,
        Hell1Battle,
        Hell2Battle,
        Taiga1Battle,
        Taiga2Battle,
        IceBergs1Battle,
        IceBergs2Battle,
        Swamp1Battle,
        Swamp2Battle,
        DarkForest1Battle,
        DarkForest2Battle,
        End1Battle, 
        End2Battle;
    #endregion
    [Space]
    public Enemy currentEnemy = null;
    public Room currentRoom, bossRoom;
    public Dialog currentDialog = null;
    public bool dialogFlag = false;
    public GameEvent currentEvent = null;
    [Space]
    public int floorIndex = 0;
    public float threatLevel = -0.5f;
    public Location[] locations = new Location[2];
    public Room[] rooms;
    void Start()
    {
        if (FindObjectsOfType<DataHolder>().Length > 1)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(this);
            dataHolder = this;
        }
    }

    public Pool stdBattle
    { get
        { 
            switch (locations[floorIndex / 2])
            {
                case Location.AshPlains:
                    if (floorIndex / 2 == 0)
                        return AshPlains1Battle;
                    else
                        return AshPlains2Battle;
                case Location.Hell:
                    if (floorIndex / 2 == 0)
                        return Hell1Battle;
                    else
                        return Hell2Battle;
                case Location.Taiga:
                    if (floorIndex / 2 == 0)
                        return Taiga1Battle;
                    else
                        return Taiga2Battle;
                case Location.IceBergs:
                    if (floorIndex / 2 == 0)
                        return IceBergs1Battle;
                    else
                        return IceBergs2Battle;
                case Location.Swamp:
                    if (floorIndex / 2 == 0)
                        return Swamp1Battle;
                    else
                        return Swamp2Battle;
                case Location.DarkForest:
                    if (floorIndex / 2 == 0)
                        return DarkForest1Battle;
                    else
                        return DarkForest2Battle;
                case Location.End:
                    if (floorIndex / 2 == 0)
                        return End1Battle;
                    else
                        return End2Battle;
                default:
                    return null;
            }
        }
    }

}

[System.Serializable]
public class Stats
{
    public int HPmax, hp, block, cardDraw;
    public List<Effect> effects = new List<Effect>();
    public List<Card> cards = new List<Card>();
}
