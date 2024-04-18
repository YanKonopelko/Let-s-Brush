using UnityEngine;
using System.Collections;
public class CrossClass : MonoBehaviour
{
    private bool isDestroy;
    [SerializeField] private ParticleSystem _flyParticle;

    private void OnEnable()
    {
        LevelManager.onParamsChange += ChangeParticle;
    }

    private void OnDisable()
    {
        LevelManager.onParamsChange -= ChangeParticle;
    }

    private void ChangeParticle()
    {
        _flyParticle = LevelManager.instance.CrossParticle;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brusher") && !isDestroy)
        {
            isDestroy = true;
            StartCoroutine("Fly");
            StartCoroutine("ParticleSpawn");
            GameObject.Find("Brusher").GetComponent<Brusher>().PickUp();
        }
        if (other.CompareTag("Roof"))
        {
            CrossSpawner.Pop(this.gameObject);
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
