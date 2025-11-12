using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableJump { public int start; public int end; }
 public enum JumpType
    {
        None,
        Ladder,
        Snake
    }
public class BoardManager : MonoBehaviour
{
    public int columns = 10;
    public int rows = 10;
    public float tileSize = 1f;
    public GameObject tilePrefab;
    public Transform tilesParent;
    public SerializableJump[] jumps;

    private Vector3 origin = Vector3.zero;
    private List<GameObject> tiles = new List<GameObject>();
    private Dictionary<int, int> jumpMap = new Dictionary<int, int>();
    private Dictionary<int, JumpType> jumpTypeMap = new Dictionary<int, JumpType>();

       void Start()
    {
        if (tilePrefab == null) Debug.LogError("Tile prefab missing!");
        if (tilesParent == null)
        {
            var go = new GameObject("Tiles");
            go.transform.parent = this.transform;
            tilesParent = go.transform;
        }
        GenerateBoard();
        BuildJumpMap();
    }

    public void BuildJumpMap()
    {
        jumpMap.Clear();
        jumpTypeMap.Clear();
        if (jumps != null)
        {
            foreach (var j in jumps)
            {
                AssignJump(j.start, j.end);
            }
        }
        if (jumpMap.Count == 0)
        {
            //Ladder Movement
            AssignJump(9, 27);
            AssignJump(18, 37);
            AssignJump(25, 54);
            AssignJump(28, 51);
            AssignJump(56, 64);
            AssignJump(68, 88);
            AssignJump(76, 97);
            AssignJump(79, 100);

            //Snake Movement
            AssignJump(16, 7);
            AssignJump(59, 17);
            AssignJump(67, 30);
            AssignJump(63, 19);
            AssignJump(93, 69);
            AssignJump(95, 75);
            AssignJump(99, 77);
        }
    }

    private void AssignJump(int start, int end)
    {
        if (start < 1 || end < 1 || start == end) return;
        jumpMap[start] = end;
        jumpTypeMap[start] = end > start ? JumpType.Ladder : JumpType.Snake;
    }

    public void GenerateBoard()
    {
        for (int i = 0; i<tilesParent.childCount; i++)
        {
            tilesParent.GetChild(i).gameObject.name =  $"Tile_{i+1}";

             tiles.Add(tilesParent.GetChild(i).gameObject);
        }
        
    }

    private int ComputeBoardNumber(int row, int col)
    {
        int r = row;
        int baseNum = r * columns;
        bool leftToRight = (r % 2 == 0);
        int boardNum = leftToRight ? baseNum + col + 1 : baseNum + (columns - col);
        return boardNum;
    }
    public Vector3 GetDefaultTilePosition()
    {
        return tiles[0].transform.position;
    }
    public Vector3 GetTilePosition(int tileNumber)
    {

        if (tileNumber < 1) tileNumber = 1;
        if (tileNumber > rows * columns) tileNumber = rows * columns;

        var found = tiles.Find(t => t.name == $"Tile_{tileNumber}");
        return found.transform.position; 
       
    }

    public int TileCount => rows * columns;

    public bool HasJumpAt(int tileNumber) => jumpMap.ContainsKey(tileNumber);

    public int GetJumpDestination(int tileNumber)
    {
        if (jumpMap.TryGetValue(tileNumber, out int dest)) return dest;
        return tileNumber;
    }
    public JumpType GetJumpType(int tileNumber)
    {
        if (jumpTypeMap.TryGetValue(tileNumber, out JumpType type)) return type;
        return JumpType.None;
    }

   

   
}
