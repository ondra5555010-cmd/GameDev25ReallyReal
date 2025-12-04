using UnityEngine;
using TMPro;

public class FloatingTextScript : MonoBehaviour
{
    public float lifetime = 1.2f;
    public float floatSpeed = 1.2f;
    public Vector3 initialOffset = new Vector3(0, 1.6f, 0);
    public bool faceCamera = true;

    private TextMeshProUGUI tmp;
    private float t = 0f;
    private Transform cam;

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>(true);
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    public void Show(string message, Color color, Transform parent = null)
    {
        tmp.text = message;
        tmp.color = color;
        t = 0f;

        if (parent != null)
            transform.position = parent.position + initialOffset;
        else
            transform.position = transform.position + initialOffset;

        gameObject.SetActive(true);
        Destroy(gameObject, lifetime);
    }

    void LateUpdate()
    {
        t += Time.deltaTime;

        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);

        // Fade out
        if (tmp != null)
        {
            float alpha = Mathf.Clamp01(1f - (t / lifetime));
            var c = tmp.color;
            c.a = alpha;
            tmp.color = c;
        }

        // Billboard
        if (faceCamera && cam != null)
        {
            transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);
        }
    }
}