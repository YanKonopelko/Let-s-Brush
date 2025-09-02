using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CapsuleParticle : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _particleSystems;
    [SerializeField] private float _duration;
    private GameObject _prefab;
    public void Init(GameObject prefab)
    {
        _prefab = prefab;
    }

    public async void Play()
    {
        foreach (var ps in _particleSystems)
        {
            ParticleSystem.MainModule settings = ps.main;
            settings.startColor = LevelManager.instance.CapsuleColor[1];
            ps.Play();
        }
        await UniTask.Delay((int)(_duration * 1000));
        LevelManager.instance.pool.Release(_prefab, gameObject);
    }
}
