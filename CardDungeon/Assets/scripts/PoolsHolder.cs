using UnityEngine;
using System.Collections.Generic;

public class PoolsHolder : MonoBehaviour
{
    public Pool ashPlainsEvents,
        hellEvents,
        taigaEvents,
        iceBergsEvents,
        swampEvents,
        darkForestEvents,
        endEvents;

    [HideInInspector]
    public Dictionary<Location, Pool> getPool;

    void Start()
    {
        getPool = new Dictionary<Location, Pool>()
        {
            { Location.AshPlains, ashPlainsEvents },
            { Location.Hell, hellEvents },
            { Location.Taiga, taigaEvents },
            { Location.IceBergs, iceBergsEvents },
            { Location.Swamp, swampEvents },
            { Location.DarkForest, darkForestEvents },
            { Location.End, endEvents }
        };
    }

}
