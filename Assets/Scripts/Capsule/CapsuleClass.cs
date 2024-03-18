using UnityEngine;

public class CapsuleClass : MonoBehaviour
{
    private bool isColored = false;
    [SerializeField] private Animation anim;
    [SerializeField] public GameObject crossPrefab;
    private Color startColor;
    private Color changedColor;
    private CapsuleManager capsuleManager = null;

    public void Init(CapsuleManager CapsuleManager){
        capsuleManager = CapsuleManager;
        GetComponent<MeshRenderer>().materials[0].color = LevelManager.instance.CapsuleColor[0];
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
                capsuleManager.Recalculate();
            }
            anim.Play("CapsuleAnim");
        }
    }

}
