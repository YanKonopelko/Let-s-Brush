using UnityEngine;

public class CapsuleClass : MonoBehaviour
{
    private bool isColored = false;
    private Animation anim;
    [SerializeField] public GameObject crossPrefab;
    private Color startColor;
    private Color changedColor;
    private void Start()
    {
       anim = transform.GetComponent<Animation>();
       startColor = LevelManager.instance.CapsuleColor[0];
       changedColor = LevelManager.instance.CapsuleColor[1];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brusher"))
        {
            if (!isColored)
            {
                isColored = true;
                GetComponent<MeshRenderer>().materials[0].color = changedColor;
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                ScenesManager.onCapsuleCounterChange.Invoke();
            }
            anim.Play("CapsuleAnim");
        }
    }
    private void OnEnable()
    {
        LevelManager.onParamsChange += ChangeColors;
    }

    private void OnDisable()
    {
        LevelManager.onParamsChange -= ChangeColors;
    }

    private void ChangeColors()
    {
        GetComponent<MeshRenderer>().materials[0].color= LevelManager.instance.CapsuleColor[0];
        changedColor = LevelManager.instance.CapsuleColor[1];
    }
}
