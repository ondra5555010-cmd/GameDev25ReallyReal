using UnityEngine;

public class RunSMB : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    if(navMeshaAgent!=null && animator!=null)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
