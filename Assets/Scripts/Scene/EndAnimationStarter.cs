using UnityEngine;

public class EndAnimationStarter : MonoBehaviour
{

    public void Follow(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, transform.position.y,pos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Capsule"))
        {
            other.transform.parent.GetComponent<Animation>().Play("FinishCapsuleAnim");
        }
    }
}
