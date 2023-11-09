using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
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
        //Sets the seed for the map
        Random.InitState(Seed);
        //Spawns basic platform
        MapParts.Add(Instantiate(Prefabs[1], new Vector3(), new Quaternion(0,0,0,0)));
        //Grabs nodes from basic platform
        CheckForNodes(MapParts[0]);
        //Start generating the map!
        GenerateSpawnArea();
    }

    void GenerateSpawnArea()
    {
        for (int i = 0; i < RoomLimit; i++)
        {
            for (int r = 0; r < RetryLimit; r++)
            {
                bool IllegalPiece = false;
                bool NodesConnect = false;
                int PieceSelected = Random.Range(0, Prefabs.Length);               
                Vector3 CurrentNodesLocalPos = Nodes[0].transform.localPosition;
                Vector3 NodeParrentPos = Nodes[0].transform.parent.position;
                Vector3 NewPiecePos = new();
                //Prefab placement based on local node position
                if (CurrentNodesLocalPos.x == -5)
                {
                    NewPiecePos = new(NodeParrentPos.x - 10, NodeParrentPos.y, NodeParrentPos.z);
                }
                if (CurrentNodesLocalPos.x == 5)
                {
                    NewPiecePos = new(NodeParrentPos.x + 10, NodeParrentPos.y, NodeParrentPos.z);
                }
                if (CurrentNodesLocalPos.z == -5)
                {
                    NewPiecePos = new(NodeParrentPos.x, NodeParrentPos.y, NodeParrentPos.z - 10);
                }
                if (CurrentNodesLocalPos.z == 5)
                {
                    NewPiecePos = new(NodeParrentPos.x, NodeParrentPos.y, NodeParrentPos.z + 10);
                }

                GameObject PieceOfMap = Instantiate(Prefabs[PieceSelected], NewPiecePos, new Quaternion(0, 0, 0, 0));
                List<GameObject> NewPieceNodes = new();
                foreach (Transform child in PieceOfMap.transform)
                {
                    if (child.CompareTag("Node"))
                    {
                        NewPieceNodes.Add(child.gameObject);
                    }
                }

                foreach (GameObject MapPiece in MapParts)
                {
                    //See if the new map piece is colliding with another piece
                    Bounds BoundsOfNewPiece = PieceOfMap.GetComponent<BoxCollider>().bounds;
                    if (MapPiece.GetComponent<BoxCollider>().bounds.Intersects(BoundsOfNewPiece))
                    {
                        IllegalPiece = true;
                        //This leaves big gaps in the map!
                        Destroy(Nodes[0]);
                        Nodes.RemoveAt(0);
                        break;
                    }
                }

                foreach (GameObject Node in NewPieceNodes)
                {
                    //Makes sure that the nodes are connecting and a prefab can be placed there
                    if (Node.transform.position == Nodes[0].transform.position)
                    {
                        NodesConnect = true;
                        Destroy(Node);
                        NewPieceNodes.Remove(Node);
                        break;
                    }
                }

                //If this room is a valid, remove the nodes used, spawn it in and add any nodes it has to the list.
                if (!IllegalPiece && NodesConnect)
                {
                    Destroy(Nodes[0]);
                    Nodes.RemoveAt(0);
                    Nodes.AddRange(NewPieceNodes);
                    MapParts.Add(PieceOfMap);
                    Debug.Log("A piece was added on requested piece: " + (i + 1));
                    break;
                }
                else
                {
                    //This destroyies the node and sends an error to console for debugging
                    if(r == RetryLimit - 1)
                    {
                        Destroy(Nodes[0]);
                        Nodes.RemoveAt(0);
                        Debug.LogError("Requested piece completely failed!: " + (i + 1));
                    }
                    Destroy(PieceOfMap);
                }
            }
        }
        //Just a debugging thing to see if all rooms properly spawned
        Debug.Log(MapParts.Count);
    }

    #nullable enable
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
    #nullable disable
}
