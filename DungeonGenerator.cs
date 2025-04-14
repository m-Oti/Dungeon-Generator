using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NaughtyAttributes;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.UI;


public class DungeonGenerator : MonoBehaviour
{
    System.Random rand = new System.Random();
    public int width = 100;
    public int height = 100;
    RectInt startRoom;
    public bool splitHorizontally;
    //public List<RectInt> rooms = new List<RectInt>();
    public List<RectInt> rooms;
    public List<RectInt> centres;
    public List<RectInt> doors;
    public int splitTimes = 0;
    public int minSize = 10;
    List<RectInt> roomsToSplit = new List<RectInt>();
    Graph<RectInt> graph = new Graph<RectInt>();
    public void Start()
    {

        startRoom = new RectInt(0, 0, width, height);
        // placing doors
    }
    IEnumerator Splitting()
    {
        roomsToSplit = new List<RectInt>();
        roomsToSplit.Add(startRoom);

        //while there are still rooms to split
        //get the first room to split, remove it from the rooms to split list
        // if it can be split, split it, put the result back on the todolist
        // if it cannot be split, move it to the rooms list
        while (roomsToSplit.Count > 0)
        {
            // Process all rooms present at the start of this split iteration
            RectInt room = roomsToSplit[0];
            roomsToSplit.RemoveAt(0);
            AlgorithmsUtils.DebugRectInt(room, Color.magenta, 100f);

            //foreach (RectInt room in roomsToSplit)
            {
                if (room.height >= 2 * minSize + 1 && room.width >= 2 * minSize + 1)
                {
                    bool splitHorizontally = rand.Next(0, 2) == 1;
                    if (splitHorizontally)
                        SplitHorizontally(room);
                    else
                        SplitVertically(room);
                }
                else if (room.height >= 2 * minSize + 1)
                    SplitHorizontally(room);
                else if (room.width >= 2 * minSize + 1)
                    SplitVertically(room);
                else
                {
                    rooms.Add(room);
                    AlgorithmsUtils.DebugRectInt(room, Color.yellow, 100f);
                    AlgorithmsUtils.DebugRectInt(GetCentre(room), Color.red, 100f);
                    graph.AddNode(GetCentre(room));
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

    }
    public void Update()
    {


        // drawing

    }

    RectInt GetCentre(RectInt room)
    {
        RectInt centre = new RectInt(room.x + room.width / 2 - 1, room.y + room.height / 2 - 1, 1, 1);
        graph.AddNode(centre);
        return centre;
    }

    // Alternative: return a bool. True if split was successful!
    void SplitVertically(RectInt room)
    {

        if (room.width <= 2 * minSize)
        {
            Debug.Log("Room disappears (vert)");
            return; // Prevent invalid splits
        }
        int w = rand.Next(minSize, room.width - minSize);
        RectInt room1 = new RectInt(room.x, room.y, w, room.height);
        RectInt room2 = new RectInt(room.x + w - 1, room.y, room.width - w + 1, room.height);
        roomsToSplit.Add(room1);
        roomsToSplit.Add(room2);
        // roomsToSplit.Remove(room);

        Debug.Log("Result of v split: " + room1 + " " + room2);

        if (rooms.Contains(room))
        {
            Debug.Log("Something is wrong? " + room);
        }
        //rooms.Remove(room); // ?
    }

    void SplitHorizontally(RectInt room)
    {

        if (room.height <= 2 * minSize)
        {
            Debug.Log("Room disappears (hor)");
            return;
        }
        int h = rand.Next(minSize, room.height - minSize);
        RectInt room1 = new RectInt(room.x, room.y, room.width, h);
        RectInt room2 = new RectInt(room.x, room.y + h - 1, room.width, room.height - h + 1);
        roomsToSplit.Add(room1);
        roomsToSplit.Add(room2);
        // roomsToSplit.Remove(room);
        Debug.Log("Result of h split: " + room1 + " " + room2);
        if (rooms.Contains(room))
        {
            Debug.Log("Something is wrong? " + room);
        }
        //rooms.Remove(room); // ?
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Generate()
    {
        //if (rooms==null || rooms.Count==0)
        //rooms = new List<RectInt> { startRoom };
        //doors = new List<RectInt>();
        //Splitting();

        StartCoroutine(GenerateDungeon());
    }

    IEnumerator GenerateDungeon()
    {
        yield return Splitting();
        StartCoroutine(GenerateDoors());
    }


    IEnumerator GenerateDoors()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++) // Start from i+1 to avoid self-check
            {
                RectInt area = AlgorithmsUtils.Intersect(rooms[i], rooms[j]);
                if (area.width > 0 && area.height > 0) // Ensure there's an actual intersection
                {
                    if (area.width > area.height && area.width > 8)
                    {
                        // Horizontal door (placed at the top/bottom of the intersection)
                        int doorX = area.x + rand.Next(1, area.width - 2);//(area.width / 2 - 2);
                        int doorY = area.y;
                        RectInt door = new RectInt(doorX, doorY, 2, 1);
                        doors.Add(door);
                        graph.AddNode(door);
                        graph.AddEdge(door, GetCentre(rooms[i]));

                        AlgorithmsUtils.DebugRectInt(door, Color.green, 100f);

                    }
                    else if (area.height > 8)
                    {
                        // Vertical door (placed at the left/right of the intersection)
                        int doorY = area.y + rand.Next(1, area.height - 2);//(area.height / 2 - 2);
                        RectInt door = new RectInt(area.x, doorY, 1, 2);
                        doors.Add(door);
                        graph.AddNode(door);
                        graph.AddEdge(door, GetCentre(rooms[i]));

                        AlgorithmsUtils.DebugRectInt(door, Color.green, 100f);


                    }
                }


            }

            yield return new WaitForSeconds(0.1f);

        }
        StartCoroutine(DrawEdges());
    }

    IEnumerator DrawEdges()
    {
        foreach (RectInt room in rooms)
        {
            foreach (RectInt door in doors)
            {
                if (AlgorithmsUtils.Intersects(door, room))
                {
                    Vector3 pos1 = new Vector3(GetCentre(room).x, 0, GetCentre(room).y);
                    Vector3 pos2 = new Vector3(door.x, 0, door.y);

                    Debug.DrawLine(pos1, pos2, Color.red, 100f);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

}
