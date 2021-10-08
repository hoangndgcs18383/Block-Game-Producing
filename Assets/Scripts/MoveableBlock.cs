using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : MonoBehaviour
{
    private Block block;

    private void Awake()
    {
        block = GetComponent<Block>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(int newX, int newY)
    {
        block.X = newX;
        block.Y = newY;

        block.transform.position = block.GridRef.GetWorldPosition(newX, newY);
    }
}
