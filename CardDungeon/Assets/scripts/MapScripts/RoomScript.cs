using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoomScript : MonoBehaviour
{
    public Room room;
    public List<RoomScript> conected = new List<RoomScript>();
    List<LineRenderer> renderers = new List<LineRenderer>();
    void Start()
    {
        MapManager.mapManager.roomScripts.ForEach(y => { if (room.conected.Contains(y.room)) conected.Add(y); });
        if (room == DataHolder.dataHolder.currentRoom)
        {
            room.isDone = true;
            SerializedObject halo = new SerializedObject(GetComponent("Halo"));
            halo.FindProperty("m_Enabled").boolValue = true;
            halo.FindProperty("m_Color").colorValue = new Color(0.54f, 0.5f, 0.16f);
            halo.ApplyModifiedProperties();
            foreach (RoomScript r in conected)
            {
                halo = new SerializedObject(r.GetComponent("Halo"));
                halo.FindProperty("m_Enabled").boolValue = true;
                halo.FindProperty("m_Color").colorValue = new Color(0.7f, 0.7f, 0.7f);
                halo.ApplyModifiedProperties();
            }
        }
        else if (room.isDone)
            GetComponent<SpriteRenderer>().color = Color.white * 0.8f;
        if (room == DataHolder.dataHolder.bossRoom)
        {
            SerializedObject halo = new SerializedObject(GetComponent("Halo"));
            halo.FindProperty("m_Enabled").boolValue = true;
            halo.FindProperty("m_Color").colorValue = Color.red;
            halo.ApplyModifiedProperties();
        }
    }

    void OnMouseEnter()
    {
        foreach(RoomScript r in conected)
        {
            renderers.Add(r.gameObject.GetComponent<LineRenderer>());
            renderers[renderers.Count - 1].enabled = true;
            renderers[renderers.Count - 1].SetPositions(new Vector3[] { transform.position, r.room.pos });
        }
    }

    void OnMouseDown()
    {
        if (DataHolder.dataHolder.currentRoom.conected.Contains(room))
        {
            if (room.isDone)
            {
                DataHolder.dataHolder.currentRoom = room;
                SceneManager.LoadScene(1);
            }
            else
            {
                DataHolder.dataHolder.currentRoom = room;
                DataHolder.dataHolder.currentEvent = room.Event;
                SceneManager.LoadScene(2);
            }
        }
    }

    void OnMouseExit()
    {
        renderers.ForEach(x => x.enabled = false);
    }
}
