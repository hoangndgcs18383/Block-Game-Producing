using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearableBlock : MonoBehaviour
{
    public AnimationClip clearAnim;

    private bool isBeingCleared = false;
    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    protected Block block;

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
    public virtual void Clear()
    {
        block.GridRef.level.OnBlockClear(block);

        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(clearAnim.name);
            yield return new WaitForSeconds(clearAnim.length);

            Destroy(gameObject);
            
        }
    }
}
