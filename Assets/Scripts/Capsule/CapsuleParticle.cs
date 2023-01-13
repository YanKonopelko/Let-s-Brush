using UnityEngine;

public class CapsuleParticle : MonoBehaviour
{
    private void Start()
    {
        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        settings.startColor = LevelManager.instance.CapsuleColor[1];
        settings.startColor = new Color(settings.startColor.color.r, settings.startColor.color.g,
            settings.startColor.color.b, 199);
    }
}
