using UnityEngine;
using System;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Color[] CapsuleColor;
    public Color PlatfomColor;
    public ParticleSystem CrossParticle;
    [SerializeField] private LevelConfig config;
    public static Action onParamsChange;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
        SetParams();
    }

    public void SetParams()
    {
        CapsuleColor = config.GetCapsuleColors();
        PlatfomColor = config.GetPlatformColors();
        CrossParticle = config.GetCrossParticles();
        onParamsChange?.Invoke();
    }
    
}
