using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager mapManager;
    [Header("Prefabs")]
    public GameObject locationPrefab;

    [Header("References")]
    [HideInInspector]
    public List<RoomScript> roomScripts = new List<RoomScript>();
    void Start()
    {
        mapManager = this;
        foreach (Room r in DataHolder.dataHolder.rooms)
        {
            roomScripts.Add(Instantiate(locationPrefab, r.pos, Quaternion.identity, transform).GetComponent<RoomScript>());
            roomScripts[roomScripts.Count - 1].room = r;
        }
    }

    void Update()
    {

    }
}
