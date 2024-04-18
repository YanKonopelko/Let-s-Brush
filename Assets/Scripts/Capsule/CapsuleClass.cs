using UnityEngine;

public class CapsuleClass : MonoBehaviour
{
    [SerializeField] private ParticleSystem ParticleSystem;
    [SerializeField] private AudioSource AudioSource;
    public void ParticlePlay(){
        ParticleSystem.Play();
        AudioSource.Play();
    }
}
