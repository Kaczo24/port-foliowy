using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Room
{
    public Vector2 pos;
    public GameEvent Event;
    public Enemy enemy;
    public List<Room> conected = new List<Room>();
    public bool isDone = false;
    bool isConected = false;
    static float connectionRange = 1.8f;
    Room(float x, float y, GameEvent @event)
    {
        pos = new Vector2(x, y);
        Event = @event;
    }


    public static Room[] GenerateRooms(int number, GameEvent[] events)
    {
        List<Room> rooms = new List<Room>();
        for (int n = 0; n < number; n++)
            rooms.Add(new Room(Random.Range(-8f, 8f), Random.Range(-4f, 4f), Helper.Pick(events)));
        foreach (Room r in rooms)
            while (!rooms.TrueForAll(x => (x == r) || (x.pos - r.pos).sqrMagnitude > 0.17f))
            {
                r.pos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            }

        rooms.ForEach(x => rooms.ForEach(y => { if (x != y) if ((x.pos - y.pos).sqrMagnitude < connectionRange * connectionRange) x.conected.Add(y); }));
        rooms[0].isConected = true;
        rooms[0].ConectionTest();
        while (!rooms.TrueForAll(x => x.isConected && (Random.value < 0.3 ? x.conected.Count > 2 : true)))
        {
            Room closestN = rooms[0], closestP = rooms[0];
            float record = 999;
            foreach(Room r in rooms)
                if(!r.isConected)
                {
                    float rec = 999;
                    Room cloN = r, cloP = r;
                    foreach (Room rr in rooms)
                        if (rr.isConected && !r.conected.Contains(rr))
                        {
                            float mg = (rr.pos - r.pos).sqrMagnitude;
                            if (mg < rec)
                            {
                                rec = mg;
                                cloN = r;
                                cloP = rr;
                            }
                        }
                    if(rec < record)
                    {
                        record = rec;
                        closestN = cloN;
                        closestP = cloP;
                    }
                }
            closestN.pos = ((closestN.pos - closestP.pos).normalized * Random.Range(connectionRange - 0.2f, connectionRange + 0.1f)) + closestP.pos;
            rooms.ForEach(x => rooms.ForEach(y => 
            {
                if (x != y)
                    if ((x.pos - y.pos).sqrMagnitude < connectionRange * connectionRange)
                    {
                        if (!x.conected.Contains(y))
                            x.conected.Add(y);
                    }
                    else if(x.conected.Contains(y))
                        x.conected.Remove(y);
            }));
            rooms.ForEach(x => x.isConected = false);
            closestP.isConected = true;
            closestP.ConectionTest();
        }

        rooms.Sort((x, y) => (x.pos - rooms[0].pos).sqrMagnitude.CompareTo((y.pos - rooms[0].pos).sqrMagnitude));
        return rooms.ToArray();
    }
    
    void ConectionTest()
    {
        foreach (Room r in conected)
        {
            if (!r.isConected)
            {
                r.isConected = true;
                r.ConectionTest();
            }
        }
    }
}
