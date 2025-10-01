using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform player;
    private int currentAngleIndex = 0;
    
    private Vector3[] angles = new Vector3[]
    {
        new Vector3(10, 10, -10),   // start: diagonal behind-right
        new Vector3(10, 10, 10),    // diagonal front-right
        new Vector3(-10, 10, 10),   // diagonal front-left
        new Vector3(-10, 10, -10)   // diagonal behind-left
    };
    
    
    void Update()
    {
        // Rotate left with Q
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentAngleIndex--;
            if (currentAngleIndex < 0) currentAngleIndex = angles.Length - 1;
        }

        // Rotate right with E
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentAngleIndex++;
            if (currentAngleIndex >= angles.Length) currentAngleIndex = 0;
        }
    }
    
    void LateUpdate()
    {
        if (player == null) return;

        transform.position = player.position + angles[currentAngleIndex];

        transform.LookAt(player.position);
    }
}