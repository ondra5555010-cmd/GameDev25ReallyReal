using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void PlayerIsVisible(bool isVisible)
    {
        animator.SetBool("PlayerIsVisible", isVisible);
    }
}
