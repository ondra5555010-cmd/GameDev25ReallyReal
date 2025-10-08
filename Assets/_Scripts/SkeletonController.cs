using UnityEngine;

[RequireComponent(typeof(Animator))]
// kdykoliv nebude na game objectu Animator, Unity ho přidá automaticky
// pokud je Animator, nic se nestane
public class SkeletonController : MonoBehaviour
{
    Animator _animator;

    [SerializeField]
    [Range(0.5f, 10f)]
    // atribut SerializeField umožní nastavit hodnotu v inspektoru, i když je pole private
    float speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponent<Animator>();
        // získáme referenci na Animator, nemusíme ji ani testovat, protože RequireComponent zajistí, že tam bude
    }

    // Update is called once per frame
    void Update()
    {
        float vx = Input.GetAxis("Horizontal");
        float vz = Input.GetAxis("Vertical");
        // získáme vstup z klávesnice WSAD nebo šipek
        if (vx != 0 || vz != 0)
        {

            _animator.SetBool("IsMoving", true);

            Vector3 movementX = speed * vx * Vector3.right;
            Vector3 movementZ = speed * vz * Vector3.forward;
            transform.Translate((movementX + movementZ) * Time.deltaTime);
        }
        else
            _animator.SetBool("IsMoving", false);
    }
}
