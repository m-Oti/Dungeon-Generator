using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NaughtyAttributes;

public class DungeonGenerator: MonoBehaviour
{
    System.Random rand = new System.Random();
    public int width = 100;
    public int height = 100;
    RectInt startRoom;
    public bool splitHorizontally;
    //public List<RectInt> rooms = new List<RectInt>();
    public List<RectInt> rooms;
    public int splitTimes = 0;
    public void Start()
    {
        
        startRoom = new RectInt(0, 0, width, height);
        // placing doors
    }
    public void Splitting()
    {
        for (int a = 0; a < splitTimes; a++)
        {
            //for (int j = rooms.Count - 1; j >= 0; j--)
            //{
            RectInt newRoom = rooms[0];
            rooms.Remove(newRoom);

                splitHorizontally = rand.Next(0, 2) == 1;
                Debug.Log(splitHorizontally);
                if (splitHorizontally)
                    SplitHorizontally(newRoom);
                else SplitVertically(newRoom);
            //}
        }
    }
    public void Update()
    {


        // drawing
        foreach (RectInt room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.magenta);
        }
    }

    void SplitVertically(RectInt room)
    {
        int w = rand.Next(3, room.width - 3);
        
        RectInt room1 = new RectInt(room.x, room.y, w, room.height);
        RectInt room2 = new RectInt(room.x + room1.width - 1, room.y, room.width - room1.width + 1, room.height);

        rooms.Add (room1);
        rooms.Add (room2);

        rooms.Remove(room);
        //AlgorithmsUtils.DebugRectInt(room1, Color.yellow);
        //AlgorithmsUtils.DebugRectInt(room2, Color.green);
    }

    void SplitHorizontally(RectInt room)
    {
        int h = rand.Next(3, room.height - 3);
        
        RectInt room1 = new RectInt(room.x, room.y, room.width, h);
        RectInt room2 = new RectInt(room.x, room.y + room1.height - 1, room.width, room.height - room1.height +1);
        
        rooms.Add(room1);
        rooms.Add(room2);

        rooms.Remove(room);
        //AlgorithmsUtils.DebugRectInt(room1, Color.yellow);
        //AlgorithmsUtils.DebugRectInt(room2, Color.green);
    }
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Generate()
    {
        rooms = new List<RectInt>();
        rooms.Add(startRoom);
        //AlgorithmsUtils.DebugRectInt(room, Color.magenta);

        Splitting();
    }
}
