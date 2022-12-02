using UnityEngine;
using System.Collections;
public class CrossClass : MonoBehaviour
{
    private bool isDestroy = false;
    [SerializeField] private ParticleSystem _flyParticle;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Capsule") && !isDestroy)
        {
            isDestroy = true;
            StartCoroutine("Fly");
            StartCoroutine("ParticleSpawn");
            GameObject.Find("Brusher").GetComponent<BrusherPowerUp>().PickUp();
        }
        if (other.CompareTag("Roof"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
    IEnumerator Fly()
    {
        yield return new WaitForSeconds(0.02f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        StartCoroutine("Fly");
    }
    IEnumerator ParticleSpawn()
    {
        Instantiate(_flyParticle, new Vector3(transform.position.x, transform.position.y-0.3f, transform.position.z),Quaternion.identity);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine("ParticleSpawn");
        
    }
}
