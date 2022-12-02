using UnityEngine;

public class CrossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _crossObject;
    public bool _enabled;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brusher")&& _enabled)
        {
            GameObject.Find("Brusher").GetComponent<BrusherPowerUp>().TurnOnCrosses();
            var cross = Instantiate(_crossObject,transform.position,Quaternion.identity);
            cross.GetComponent<Animation>().Play("CrossAnimation 1");
            var cross1 = Instantiate(_crossObject, transform.position, Quaternion.identity);
            cross1.GetComponent<Animation>().Play("CrossAnimation 2");
            var cross2 = Instantiate(_crossObject, transform.position, Quaternion.identity);
            cross2.GetComponent<Animation>().Play("CrossAnimation 3");
            Destroy(GetComponent<CrossSpawner>());
        }
    }
}
