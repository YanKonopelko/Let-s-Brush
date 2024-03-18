using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using Unity.VisualScripting;

public class ScenesManager : MonoBehaviour
{
    private float _finishTime = 2;
    public static ScenesManager Instance;

    private void Start(){
        Instance = this;
    }

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
