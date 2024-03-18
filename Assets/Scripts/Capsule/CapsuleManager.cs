using System;
using UnityEngine;

public class CapsuleManager : MonoBehaviour
{
    private CapsuleClass[] _capsules = Array.Empty<CapsuleClass>();
    public int _capsulesCounter = 0;
    public int _capsulesAmount = 0; 

    public static CapsuleManager Instance;

    private void Start(){
        Instance = this;
        _capsules = this.GetComponentsInChildren<CapsuleClass>();
        _capsulesAmount = _capsules.Length;
        foreach (var capsule in _capsules)
        {
            capsule.Init(this);
        }
    }

    public void Recalculate() {
        _capsulesCounter += 1;
        if(_capsulesCounter == _capsulesAmount)
        {
            GameObject.Find("EndAnimationStarter").GetComponent<EndAnimationStarter>().Follow(GameObject.Find("Brusher").transform.GetChild(0).position);
            GameObject.Find("EndAnimationStarter").GetComponent<Animation>().Play("EndAnimation");
            GameObject.Find("Brusher").GetComponent<BrusherEndAnim>().EndAnim();  
            ScenesManager.Instance.Finish();
        }
    }

}
