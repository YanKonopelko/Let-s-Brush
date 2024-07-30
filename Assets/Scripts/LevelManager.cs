using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Color[] CapsuleColor;

    public Material[] CapsuleMaterials;


    public Color PlatfomColor;
    public ParticleSystem CrossParticle;

    public GameObject CrossPrefab;

    [SerializeField] private LevelConfig config;
    public static Action onParamsChange;

    public CapsuleManager CapsuleManager;
    public UIManager UIManager;
    public Pool pool = new Pool();

    void Awake()
    {
        CapsuleMaterials[0].color = CapsuleColor[0];
        CapsuleMaterials[1].color = CapsuleColor[1];
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
        SetParams();

        CapsuleManager.Init();
        UIManager.Init();
        GameObject[] objects = new GameObject[1];
        objects[0] = CrossPrefab;
        pool.PreparePool(objects,1);
        pool.GetFromPool(CrossPrefab);
    }

    public void SetParams()
    {
        // CapsuleColor = config.GetCapsuleColors();
        PlatfomColor = config.GetPlatformColors();
        CrossParticle = config.GetCrossParticles();
        onParamsChange?.Invoke();
    }
    
    public void Reload(){
        CapsuleManager.Reload();
        UIManager.Reload();
    }
    
     private float _finishTime = 2;
  
    private IEnumerator FinishScene(float time)
    {
        yield return new WaitForSeconds(time);
        if(SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCount-1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Finish(){
        StartCoroutine(FinishScene(_finishTime));
    }   
}
