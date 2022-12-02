using UnityEngine;

public class CapsuleClass : MonoBehaviour
{
    private bool isColored = false;
    private Animation anim;
    private GameObject SceneManager;
    [SerializeField] private ParticleSystem _particle;

    private void Start()
    {
       anim = transform.GetComponent<Animation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brusher"))
        {
            if (!isColored)
            {
                isColored = true;
                transform.GetComponent<MeshRenderer>().materials[0].color = Color.red;
                _particle.Play();
                ScenesManager.onCapsuleCounterChange.Invoke();
            }
            anim.Play("CapsuleAnim");
        }
    }
}
