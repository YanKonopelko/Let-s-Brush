using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ScenesManager : MonoBehaviour
{
    public int _capsulesAmount = 256; 
    public int _capsulesCounter = 0;

    public static Action onCapsuleCounterChange;

    private float _finishTime = 2;
    private void Awake()
    {
        onCapsuleCounterChange += Recalculate;
    }

    public void Recalculate() {
        _capsulesCounter += 1;
        if(_capsulesCounter == _capsulesAmount)
        {
            GameObject.Find("EndAnimationStarter").GetComponent<EndAnimationStarter>().Follow(GameObject.Find("Brusher").transform.GetChild(0).position);
            GameObject.Find("EndAnimationStarter").GetComponent<Animation>().Play("EndAnimation");
            GameObject.Find("Brusher").GetComponent<BrusherEndAnim>().EndAnim();  
            StartCoroutine( FinishScene(_finishTime));
        }
    }

    public IEnumerator FinishScene(float time)
    {
        yield return new WaitForSeconds(time);
        if(SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCount-1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        onCapsuleCounterChange -= Recalculate;
    }
}
