using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Enum
public enum BlockType
{
    NORMAL,
    COUNT
}

#endregion

public class Grid : MonoBehaviour
{

    #region Variables
    [Header("Dim")]
    public int xDim;
    public int yDim;

    [Header("Position of World Map ")]
    public float offset = 2.0f;
    #endregion

    #region References
    [System.Serializable]
    public struct BlockPrefab
    {
        public BlockType type;
        public GameObject prefab;
    }
    //which has keys of Block type and values of game object  
    private Dictionary<BlockType, GameObject> blockPrefabDictionary;

    //define array the struct that we can edit in the inspector
    public BlockPrefab[] blockPrefabs;
    //create a bg
    public GameObject backgroundPrefab;
    //create 2d array
    private Block[,] blocks;
    #endregion

    void Start()
    {
        blockPrefabDictionary = new Dictionary<BlockType, GameObject>();
        //loop though all the prefabs in our block prefabs array
        for(int i = 0; i < blockPrefabs.Length; ++i)
        {
            if (!blockPrefabDictionary.ContainsKey(blockPrefabs[i].type))
            {
                //check Dictionary not null with contain key, if it is null the Dictionary add a new key value to Dictionanry.
                blockPrefabDictionary.Add(blockPrefabs[i].type, blockPrefabs[i].prefab);
            }
        }
        //loop all the rows from 0 to x dim and all the column from 0 to y dim
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                GameObject backgound = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                backgound.transform.parent = transform;
            }
        }
        //instantiate game object array to have dimension the same as gid
        blocks = new Block[xDim, yDim];
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                GameObject newBlock = (GameObject)Instantiate(blockPrefabDictionary[BlockType.NORMAL], Vector3.zero, Quaternion.identity);
                newBlock.name = "Block(" + x + "," + y + ")";
                newBlock.transform.parent = transform;
                //store the block to new block object in block array
                blocks[x, y] = newBlock.GetComponent<Block>();
                blocks[x, y].Init(x, y, this, BlockType.NORMAL);

                if(blocks[x, y].IsMovable())
                {
                    blocks[x, y].MoveableBlock.Move(x, y);
                }

                if (blocks[x, y].IsColored())
                {
                    blocks[x, y].ColorBlock.SetColor((ColorBlock.ColorType)Random.Range(0, blocks[x, y].ColorBlock.NumColors));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / offset + x,
            transform.position.y + yDim / offset - y);
    }
}
