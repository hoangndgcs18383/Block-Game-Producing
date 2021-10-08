using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Variables

    private int x;
    private int y;

    private BlockType type;

    #endregion
    #region References

    private Grid grid;
    private MoveableBlock moveableBlock;
    private ColorBlock colorBlock;

    #endregion
    #region Properties

    public int X
    {
        get { return x; }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }

    public BlockType BlockType
    {
        get { return type; }
    }

    public Grid GridRef
    {
        get { return grid; }
    }
    public MoveableBlock MoveableBlock
    {
        get { return moveableBlock; }
    }

    public ColorBlock ColorBlock
    {
        get { return colorBlock; }
    }
    #endregion

    public void Init(int _x, int _y, Grid _grip, BlockType _type)
    {
        x = _x;
        y = _y;
        grid = _grip;
        type = _type;
    }

    private void Awake()
    {
        moveableBlock = GetComponent<MoveableBlock>();
        colorBlock = GetComponent<ColorBlock>();
    }

    public bool IsMovable()
    {
        return moveableBlock != null;
    }

    public bool IsColored()
    {
        return colorBlock != null;
    }
}
