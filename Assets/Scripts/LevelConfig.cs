using System;
using UnityEngine;

[CreateAssetMenu(order = 51)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private Color[] CapsuleStartColors;
    [SerializeField] private Color[] CapsuleFilledColors;
    [SerializeField] private Color[] PlatformColors;
    [SerializeField] private ParticleSystem[] CrossParticles;

    [SerializeField] private Material CapsuleStartMaterial;
    [SerializeField] private Material CapsuleFiledMaterial;

    [SerializeField] private Color FloorColor;



    public Color[] GetCapsuleColors()
    {
        var colors = new Color[2];
        Array.Copy(CapsuleFilledColors,colors,2);
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