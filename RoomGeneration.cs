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
        Random.InitState(Seed);
        MapParts.Add(Instantiate(Prefabs[1], new Vector3(), new Quaternion(0,0,0,0)));
        CheckForNodes(MapParts[0]);
        Debug.Log(Nodes.Count);
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
                        break;
                    }
                }

                foreach (GameObject Node in NewPieceNodes)
                {
                    if (Node.transform.position == Nodes[0].transform.position)
                    {
                        NodesConnect = true;
                        Destroy(Node);
                        NewPieceNodes.Remove(Node);
                        break;
                    }
                }

                if (!IllegalPiece && NodesConnect)
                {
                    Destroy(Nodes[0]);
                    Nodes.RemoveAt(0);
                    Nodes.AddRange(NewPieceNodes);
                    MapParts.Add(PieceOfMap);
                    break;
                }
                else
                {
                    if(r == RetryLimit - 1)
                    {
                        Destroy(Nodes[0]);
                        Nodes.RemoveAt(0);
                    }
                    Destroy(PieceOfMap);
                }
            }
        }
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
