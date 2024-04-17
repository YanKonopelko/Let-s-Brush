using UnityEngine;

public class CapsuleClass : MonoBehaviour
{
    public void ParticlePlay(){
        this.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }
}
