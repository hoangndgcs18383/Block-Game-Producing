using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLine : ClearableBlock
{
    public bool isRow;

    public override void Clear()
    {
        base.Clear();

        if (isRow)
        {
            block.GridRef.ClearRow(block.Y);
        }
        else
        {
            block.GridRef.ClearColumn(block.X);
        }
    }
}
