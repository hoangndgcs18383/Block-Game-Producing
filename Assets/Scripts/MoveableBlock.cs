using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : MonoBehaviour
{
    #region References
    private Block block;
    private IEnumerator moveCoroutine;
    #endregion
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

    public void Move(int newX, int newY, float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        block.X = newX;
        block.Y = newY;

        //move a tiny each frame, start position is current position
        Vector3 startPos = transform.position;
        //end position is the grid get position function
        Vector3 endPos = block.GridRef.GetWorldPosition(newX, newY);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            block.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        block.transform.position = endPos;
    }
}
