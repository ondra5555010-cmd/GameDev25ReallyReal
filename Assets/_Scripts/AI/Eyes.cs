using System.IO.Compression;
using UnityEngine;
using UnityEngine.Events;

public class Eyes : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    ConeCollider coneCollider;

    public UnityEvent<bool> PlrayerIsVisible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
       coneCollider = GetComponent<ConeCollider>();
    }
 
    void CheckVisibility(Collider other)
    {
        if (other.gameObject == player)
        {
            Vector3 position = transform.position;
            Vector3 playerPosition = player.transform.position;
            //playerPosition.y += 1.0f; // Adjust for player height
            Vector3 direction = (playerPosition - position).normalized;
            Debug.Log("Player Detected");

            Ray ray = new Ray(position, direction);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, coneCollider.Distance);
            Debug.DrawRay(position, direction * coneCollider.Distance, Color.red);

            if (hit.collider != null && hit.collider?.gameObject == player)
            {
                Debug.Log("Player in Sight");
                PlrayerIsVisible?.Invoke(true);
            }
            else
            {
                Debug.Log("Player Obstructed");
                PlrayerIsVisible?.Invoke(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        CheckVisibility(other);
        {
            Debug.Log("Player Detected");
        }
    }

    void OnTriggerStay(Collider other)
    {
        CheckVisibility(other);
        {
            Debug.Log("Player Detected");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player Lost");
            PlrayerIsVisible?.Invoke(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
