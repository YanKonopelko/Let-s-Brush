using UnityEngine;

public class CrossSpawner : MonoBehaviour
{
    // [SerializeField] private GameObject CrossPrefab;
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Brusher"))
    //     {
    //         CrossPrefab = gameObject.GetComponent<CapsuleClass>().crossPrefab;
    //         GameObject.Find("Brusher").GetComponent<BrusherPowerUp>().TurnOnCrosses();
    //         var cross = Instantiate(CrossPrefab,transform.position,Quaternion.identity);
    //         cross.GetComponent<Animation>().Play("CrossAnimation 1");
    //         cross = Instantiate(CrossPrefab, transform.position, Quaternion.identity);
    //         cross.GetComponent<Animation>().Play("CrossAnimation 2");
    //         cross = Instantiate(CrossPrefab, transform.position, Quaternion.identity);
    //         cross.GetComponent<Animation>().Play("CrossAnimation 3");
    //         Destroy(this);
    //     } 
    // }
}
