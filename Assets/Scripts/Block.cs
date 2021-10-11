using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Variables
    public int score;
    private int x;
    private int y;

    private BlockType type;

    #endregion
    #region References

    private Grid grid;
    private MoveableBlock moveableBlock;
    private ColorBlock colorBlock;
    private ClearableBlock clearableBlock;

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
    public ClearableBlock ClearableBlock
    {
        get { return clearableBlock; }
    }
    #endregion

    public void Init(int _x, int _y, Grid _grip, BlockType _type)
    {
        x = _x;
        y = _y;
        grid = _grip;
        type = _type;
    }
    private void OnMouseEnter()
    {
        grid.EnterBlock(this);
    }
    private void OnMouseUp()
    {
        grid.ReleaseBlock();
    }
    private void OnMouseDown()
    {
        grid.PressBlock(this);
    }

    private void Awake()
    {
        moveableBlock = GetComponent<MoveableBlock>();
        colorBlock = GetComponent<ColorBlock>();
        clearableBlock = GetComponent<ClearableBlock>();
    }

    public bool IsMovable()
    {
        return moveableBlock != null;
    }

    public bool IsColored()
    {
        return colorBlock != null;
    }
    public bool IsCleareble()
    {
        return clearableBlock != null;
    }
}
