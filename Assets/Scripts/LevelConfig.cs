using System;
using UnityEngine;

[CreateAssetMenu(order = 51)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private Color[] CapsuleColors;
    [SerializeField] private Color[] PlatformColors;
    [SerializeField] private ParticleSystem[] CrossParticles;

    public Color[] GetCapsuleColors()
    {
        var colors = new Color[2];
        Array.Copy(CapsuleColors,colors,2);
        return colors;
    }
    public Color GetPlatformColors()
    {
        var color = PlatformColors[0];
        return color;
    }
    public ParticleSystem GetCrossParticles()
    {
        var particle = CrossParticles[0];
        return particle;
    }
}