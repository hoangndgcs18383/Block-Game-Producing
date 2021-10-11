using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Enum
public enum BlockType
{
    EMPTY,
    NORMAL,
    BUBBLE,
    ROW_CLEAR,
    COLUMN_CLEAR,
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
    [Header("Time fulfill the block")]
    public float fillTime;
    #endregion

    #region References
    [System.Serializable]
    public struct BlockPrefab
    {
        public BlockType type;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct BlockPosition
    {
        public BlockType type;
        public int x;
        public int y;
    };
    //which has keys of Block type and values of game object  
    private Dictionary<BlockType, GameObject> blockPrefabDictionary;

    //define array the struct that we can edit in the inspector
    public BlockPrefab[] blockPrefabs;
    public BlockPosition[] blockPositions;
    //create a bg
    public GameObject backgroundPrefab;
    //create 2d array
    private Block[,] blocks;

    private bool inverse = false;
    private bool gameOver = false;
    private Block pressedBlock;
    private Block enteredBlock;
    public Level level;

    #endregion

    void Awake()
    {
        blockPrefabDictionary = new Dictionary<BlockType, GameObject>();
        //loop though all the prefabs in our block prefabs array
        for (int i = 0; i < blockPrefabs.Length; ++i)
        {
            if (!blockPrefabDictionary.ContainsKey(blockPrefabs[i].type))
            {
                //check Dictionary not null with contain key, if it is null the Dictionary add a new key value to Dictionanry.
                blockPrefabDictionary.Add(blockPrefabs[i].type, blockPrefabs[i].prefab);
            }
        }
        //loop all the rows from 0 to x dim and all the column from 0 to y dim
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject backgound = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                backgound.transform.parent = transform;
            }
        }
        //instantiate game object array to have dimension the same as gid
        blocks = new Block[xDim, yDim];

        for(int i = 0; i < blockPositions.Length; i++)
        {
            if(blockPositions[i].x >= 0 && blockPositions[i].x < xDim
                && blockPositions[i].y >= 0 && blockPositions[i].y < yDim)
            {
                SpawnNewBlock(blockPositions[i].x, blockPositions[i].y, blockPositions[i].type);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewBlock(x, y, BlockType.EMPTY);
            }
        }


        StartCoroutine(Fill());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needsRefill = ClearAllValidMatches();
        }
        
    }

    public bool FillStep()
    {
        bool movedPiece = false;
        //loop thought all the columns in reverse order, from bottom to top. 
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                Block block = blocks[x, y];
                if (block.IsMovable())
                {
                    Block blockBelow = blocks[x, y + 1];

                    if (blockBelow.BlockType == BlockType.EMPTY) // check block if it emplty
                    {
                        Destroy(blockBelow.gameObject);
                        //Move the block down, fill time take the board to refill is the same amout of time take a block to move from
                        // one space to the next
                        block.MoveableBlock.Move(x, y + 1, fillTime);
                        blocks[x, y + 1] = block;
                        SpawnNewBlock(x, y, BlockType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDim)
                                {
                                    Block diagonalBlock = blocks[diagX, y + 1];
                                    if (diagonalBlock.BlockType == BlockType.EMPTY)
                                    {
                                        bool hasBlockAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            Block blockAbove = blocks[diagX, aboveY];

                                            if (blockAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!blockAbove.IsMovable() && blockAbove.BlockType != BlockType.EMPTY)
                                            {
                                                hasBlockAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasBlockAbove)
                                        {
                                            Destroy(diagonalBlock.gameObject);
                                            block.MoveableBlock.Move(diagX, y + 1, fillTime);
                                            blocks[diagX, y + 1] = block;
                                            SpawnNewBlock(x, y, BlockType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //loop thought all the cells in the row, 
        for (int x = 0; x < xDim; x++)
        {
            Block blockBelow = blocks[x, 0]; //the block Below will be a x and 0 row which is a top row.
            if (blockBelow.BlockType == BlockType.EMPTY) // if it's emplty, it will be created a new block
            {
                Destroy(blockBelow.gameObject);
                GameObject newBlock = (GameObject)Instantiate(blockPrefabDictionary[BlockType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newBlock.transform.parent = transform;

                blocks[x, 0] = newBlock.GetComponent<Block>();
                blocks[x, 0].Init(x, -1, this, BlockType.NORMAL);
                blocks[x, 0].MoveableBlock.Move(x, 0, fillTime);
                blocks[x, 0].ColorBlock.SetColor((ColorBlock.ColorType)Random.Range(0, blocks[x, 0].ColorBlock.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / offset + x,
            transform.position.y + yDim / offset - y);
    }

    public Block SpawnNewBlock(int x, int y, BlockType type)
    {

        GameObject newBlock = (GameObject)Instantiate(blockPrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity);
        newBlock.transform.parent = transform;

        blocks[x, y] = newBlock.GetComponent<Block>();
        blocks[x, y].Init(x, y, this, type);

        return blocks[x, y];
    }

    public bool IsAdjacent(Block block_1, Block block_2)
    {
        return (block_1.X == block_2.X && (int)Mathf.Abs(block_1.Y - block_2.Y) == 1) ||
            (block_1.Y == block_2.Y && (int)Mathf.Abs(block_1.X - block_2.X) == 1);
    }

    public void SwapBlock(Block block_1, Block block_2)
    {
        if (gameOver)
        {
            return;
        }
        if(block_1.IsMovable() && block_2.IsMovable())
        {
            blocks[block_1.X, block_1.Y] = block_2;
            blocks[block_2.X, block_2.Y] = block_1;

            if (GetMatch(block_1, block_2.X, block_2.Y) != null || GetMatch(block_2, block_1.X, block_1.Y) != null)
            {
                int block1X = block_1.X;
                int block1Y = block_1.Y;

                int block2X = block_2.X;
                int block2Y = block_2.Y;

                block_1.MoveableBlock.Move(block_2.X, block_2.Y, fillTime);
                block_2.MoveableBlock.Move(block1X, block1Y, fillTime);
                ClearAllValidMatches();
                Debug.Log("Matching succesful !!");

                if(block_1.BlockType == BlockType.ROW_CLEAR || block_1.BlockType == BlockType.COLUMN_CLEAR)
                {
                    ClearBlock(block1X, block1Y);
                }
                if (block_2.BlockType == BlockType.ROW_CLEAR || block_2.BlockType == BlockType.COLUMN_CLEAR)
                {
                    ClearBlock(block2X, block2Y);
                }

                pressedBlock = null;
                enteredBlock = null;

                StartCoroutine(Fill());
                level.OnMove();
            }
            else
            {
                Debug.Log("Can't not match !!");
                blocks[block_1.X, block_1.Y] = block_1;
                blocks[block_2.X, block_2.Y] = block_1;
            }
        }   
    }

    public void PressBlock(Block block)
    {
        pressedBlock = block;
    }
    public void EnterBlock(Block block)
    {
        enteredBlock = block;
    }

    public void ReleaseBlock()
    {
        if(IsAdjacent(pressedBlock, enteredBlock))
        {
            SwapBlock(pressedBlock, enteredBlock);
        }
    }

    public List<Block> GetMatch(Block block, int newX, int newY)
    {
        if (block.IsColored())
        {
            ColorBlock.ColorType color = block.ColorBlock.Color;
            List<Block> xBlocks = new List<Block>();
            List<Block> yBlocks = new List<Block>();
            List<Block> matchBlocks = new List<Block>();

            //check horizontal
            xBlocks.Add(block);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    {
                        //Left
                        x = newX - xOffset;
                    }
                    else
                    {
                        //Right
                        x = newX + xOffset;
                    }
                    //check block match is out of ranged
                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }
                    //check block is the same color become matching.
                    if (blocks[x, newY].IsColored() && blocks[x, newY].ColorBlock.Color == color)
                    {
                        xBlocks.Add(blocks[x, newY]);
                    }
                    else // or return
                    {
                        break;
                    }
                }
            }
            if (xBlocks.Count >= 3)
            {
                for (int i = 0; i < xBlocks.Count; i++)
                {
                    matchBlocks.Add(xBlocks[i]);
                }
            }

            //Traverse verticalll and horizontally if we found a match
            if (xBlocks.Count >= 3)
            {
                for (int i = 0; i < xBlocks.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;

                            if (dir == 0)
                            {
                                //check UP
                                y = newY - yOffset;
                            }
                            else
                            {
                                //check DOWM
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }
                            //check this adjacent block is colored and this block is matching. 
                            if (blocks[xBlocks[i].X, y].IsColored() && blocks[xBlocks[i].X, y].ColorBlock.Color == color)
                            { 
                                yBlocks.Add(blocks[xBlocks[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (yBlocks.Count < 2)
                    {
                        yBlocks.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < yBlocks.Count; j++)
                        {
                            matchBlocks.Add(yBlocks[j]);
                        }
                        break;
                    }
                }
            }
            if (matchBlocks.Count >= 3)
            {
                return matchBlocks;
            }
            //Didn't find anything going x;
            xBlocks.Clear();
            yBlocks.Clear();
            //check vertical
            yBlocks.Add(block);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    {
                        //Left
                        y = newY - yOffset;
                    }
                    else
                    {
                        //Right
                        y = newY + yOffset;
                    }
                    //check block match is out of ranged
                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }
                    //check block is the same color become matching.
                    if (blocks[newX, y].IsColored() && blocks[newX, y].ColorBlock.Color == color)
                    {
                        yBlocks.Add(blocks[newX, y]);
                    }
                    else // or return
                    {
                        break;
                    }
                }
            }
            if (yBlocks.Count >= 3)
            {
                for (int i = 0; i < yBlocks.Count; i++)
                {
                    matchBlocks.Add(yBlocks[i]);
                }
            }

            //Traverse verticalll and horizontally if we found a match
            if (yBlocks.Count >= 3)
            {
                for (int i = 0; i < yBlocks.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            {
                                //check Left
                                x = newX - xOffset;
                            }
                            else
                            {
                                //check Right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }
                            //check this adjacent block is colored and this block is matching. 
                            if (blocks[x, yBlocks[i].Y].IsColored() && blocks[x, yBlocks[i].Y].ColorBlock.Color == color)
                            {
                                xBlocks.Add(blocks[x, yBlocks[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (xBlocks.Count < 2)
                    {
                        xBlocks.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < xBlocks.Count; j++)
                        {
                            matchBlocks.Add(xBlocks[j]);
                        }
                        break;
                    }
                }
            }
            if (matchBlocks.Count >= 3)
            {
                return matchBlocks;
            }
            
        }
        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                if(blocks[x, y].IsCleareble())
                {
                    List<Block> match = GetMatch(blocks[x, y], x, y);

                    if(match != null)
                    {
                        BlockType specialBlockType = BlockType.COUNT;
                        Block randonBlock = match[Random.Range(0, match.Count)];
                        int specialBlockX = randonBlock.X;
                        int specialBlockY = randonBlock.Y;

                        if(match.Count == 4)
                        {
                            if(pressedBlock == null || enteredBlock == null)
                            {
                                specialBlockType = (BlockType)Random.Range((int)BlockType.ROW_CLEAR, (int)BlockType.COLUMN_CLEAR);
                            }
                            else if(pressedBlock.Y ==  enteredBlock.Y)
                            {
                                specialBlockType = BlockType.ROW_CLEAR;
                            }
                            else
                            {
                                specialBlockType = BlockType.COLUMN_CLEAR;
                            }
                        }

                        for(int i = 0; i< match.Count; i++)
                        {
                            if(ClearBlock(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if(match[i] == pressedBlock || match[i] == enteredBlock)
                                {
                                    specialBlockX = match[i].X;
                                    specialBlockY = match[i].Y;
                                }
                            }
                        }

                        if(specialBlockType != BlockType.COUNT)
                        {
                            Destroy(blocks[specialBlockX, specialBlockY]);

                            Block newBlock = SpawnNewBlock(specialBlockX, specialBlockY, specialBlockType);
                            if (specialBlockType == BlockType.ROW_CLEAR || specialBlockType == BlockType.COLUMN_CLEAR
                                && newBlock.IsColored() && match[0].IsColored())
                            {
                                newBlock.ColorBlock.SetColor(match[0].ColorBlock.Color);
                            }
                        }
                    }
                }
            }
        }
        return needsRefill;
    }
    public bool ClearBlock(int x, int y)
    {
        if(blocks[x, y].IsCleareble() && !blocks[x, y].ClearableBlock.IsBeingCleared)
        {
            blocks[x, y].ClearableBlock.Clear();
            SpawnNewBlock(x, y, BlockType.EMPTY);
            ClearObstacles(x, y);
            return true;
        }
        return false;
    }
    public void ClearObstacles(int x, int y)
    {
        //check the abjacent blocks x direction 
        for (int abjacentX = x -1; abjacentX <= x + 1; abjacentX++)
        {
            if(abjacentX != x && abjacentX >= 0 && abjacentX < xDim)
                if(blocks[abjacentX, y].BlockType == BlockType.BUBBLE && blocks[abjacentX, y].IsCleareble())
                {
                    blocks[abjacentX, y].ClearableBlock.Clear();
                    SpawnNewBlock(abjacentX, y, BlockType.EMPTY);
                }
        }
        //check the abjacent blocks y direction 
        for(int abjacentY = y - 1; abjacentY <= y + 1; abjacentY++)
        {
            if (abjacentY != y && abjacentY >= 0 && abjacentY < yDim)
                if (blocks[x, abjacentY].BlockType == BlockType.BUBBLE && blocks[x, abjacentY].IsCleareble())
                {
                    blocks[x, abjacentY].ClearableBlock.Clear();
                    SpawnNewBlock(x, abjacentY, BlockType.EMPTY);
                }
        }
    }

    public void ClearRow(int row)
    {
        for(int x = 0; x < xDim; x++)
        {
            ClearBlock(x, row);
        }
    }
    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearBlock(y, column);
        }
    }
    public void GameOver()
    {
        gameOver = true;
    }
}
