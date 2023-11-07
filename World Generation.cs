using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    //Contains all of the prefabs for this level
    public GameObject[] Prefabs = new GameObject[5];
    public GameObject WallZ;
    public GameObject WallX;
    //Seed entered by player or randomly generated
    public int Seed;
    //Limits the amount of rooms that can be placed (Debug)
    public int RoomLimit = 50;
    //Sets retry limit for trying to spawn a room
    public int RetryLimit = 10;
    //Holds all map info to generate new rooms
    List<GameObject> Nodes = new();
    List<GameObject> MapParts = new();
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(Seed);
        MapParts.Add(Instantiate(Prefabs[0],new Vector3(0,0,0), new Quaternion(0,0,0,0)));
        CheckForNodes(MapParts[0]);
        GenerateRooms();
    }

    void GenerateRooms()
    {
        int Count = 0;
        int RoomsGenerated = 0;
        bool NodesTouch = false;
        for(int i = 0; i < RoomLimit; i++)
        {
            NodesTouch = false;
            //Retry loop
            for (int n = 0; n < RetryLimit; n++)
            {
                NodesTouch = false;
                Vector3 LocalNodePos = new();
                Vector3 GlobalRoomPos = new();
                Vector3 NewPiecePos = new();
                bool RoomFailed = false;
                int PieceSelected = Random.Range(0, Prefabs.Length);
                LocalNodePos = Nodes[0].transform.localPosition;
                GlobalRoomPos = Nodes[0].transform.parent.position;
                NewPiecePos = GlobalRoomPos;
                if (LocalNodePos.x == -5)
                {
                    NewPiecePos = new(NewPiecePos.x - 10, NewPiecePos.y, NewPiecePos.z);
                }
                if (LocalNodePos.x == 5)
                {
                    NewPiecePos = new(NewPiecePos.x + 10, NewPiecePos.y, NewPiecePos.z);
                }
                if (LocalNodePos.z == -5)
                {
                    NewPiecePos = new(NewPiecePos.x, NewPiecePos.y, NewPiecePos.z - 10);
                }
                if (LocalNodePos.z == 5)
                {
                    NewPiecePos = new(NewPiecePos.x, NewPiecePos.y, NewPiecePos.z + 10);
                }
                MapParts.Add(Instantiate(Prefabs[PieceSelected], NewPiecePos, new Quaternion(0, 0, 0, 0)));
                Count = 0;
                //Make sure room isn't placed ontop of another one
                for (int m = 0; m < MapParts.Count; m++)
                {
                    if (MapParts[m].transform.position == MapParts[MapParts.Count - 1].transform.position && MapParts.Count > 1)
                    {
                        if (m != MapParts.Count - 1)
                        {
                            Destroy(MapParts[MapParts.Count - 1]);
                            //MapParts.RemoveAt(MapParts.Count - 1);
                            RoomFailed = true;
                            break;
                        }
                    }
                }

                //Old code
                CheckForNodes(MapParts[MapParts.Count - 1]);
                Count = 0;
                //Checks through all nodes to make sure rooms line up
                foreach (GameObject Node in Nodes)
                {
                    Collider N1, N2;
                    N1 = Nodes[0].GetComponent<Collider>();
                    N2 = Node.GetComponent<Collider>();
                    Vector3 NodeFlip = Node.transform.localPosition;
                    if (Node.transform.localPosition.x != 0)
                    {
                        NodeFlip.x = -NodeFlip.x;
                    }
                    if (Node.transform.localPosition.z != 0)
                    {
                        NodeFlip.z = -NodeFlip.z;
                    }
                    //This line was changed
                    if (Count > 0)
                    {
                        //N1.bounds.Intersects(N2.bounds) && Nodes[0].transform.localPosition == NodeFlip &&
                        if (Nodes[0].transform.position == Node.transform.position)
                        {
                            Destroy(Nodes[0]);
                            Destroy(Nodes[Count]);
                            Nodes.RemoveAt(0);
                            Nodes.RemoveAt(Count - 1);
                            NodesTouch = true;
                            break;
                        }
                    }

                    if (Count == Nodes.Count)
                    {
                        NodesTouch = false;
                        break;
                    }
                    Count++;
                }
                if (NodesTouch == false)
                {
                    Destroy(MapParts[MapParts.Count - 1]);
                    MapParts.RemoveAt(MapParts.Count - 1);
                    if (n == RetryLimit)
                    {
                        SpawnWall();
                        break;
                    }
                }
                else
                {
                    RoomsGenerated++;
                    break;
                }
            }
        }
        Debug.Log(RoomsGenerated);
    }

    void CheckForNodes(GameObject? Target)
    {
        if (Target)
        {
            foreach (Transform child in Target.transform)
            {
                if (child.CompareTag("Node"))
                {
                    Nodes.Add(child.gameObject);
                }
            }
            return;
        }
        Nodes.Clear();
        Nodes.AddRange(GameObject.FindGameObjectsWithTag("Node"));
        Debug.Log(Nodes.Count);
        return;
    }

    void SpawnWall()
    {
        GameObject Wall;
        Vector3 LocalNodePos = Nodes[0].transform.localPosition;
        if (LocalNodePos.x == -5)
        {
            Wall = Instantiate(WallX, Nodes[0].transform.parent);
        }
        if (LocalNodePos.x == 5)
        {
            Wall = Instantiate(WallX, Nodes[0].transform.parent);
            Wall.transform.localPosition = new(-Wall.transform.localPosition.x, Wall.transform.localPosition.y);
        }
        if (LocalNodePos.z == -5)
        {
            Wall = Instantiate(WallZ, Nodes[0].transform.parent);
        }
        if (LocalNodePos.z == 5)
        {
            Wall = Instantiate(WallX, Nodes[0].transform.parent);
            Wall.transform.localPosition = new(Wall.transform.localPosition.x, Wall.transform.localPosition.y, -Wall.transform.localPosition.z);
        }
        Debug.Log("Wall spawned!");
    }
}
