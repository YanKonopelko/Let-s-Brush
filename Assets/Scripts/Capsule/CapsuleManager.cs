using System;
using UnityEngine;

public class CapsuleManager : MonoBehaviour
{
    private CapsuleClass[] _capsules = Array.Empty<CapsuleClass>();
    private Transform[] _allCapsulesTransform ;

    private MeshRenderer[] capsuleRenderers ;

    public int _capsulesCounter = 0;
    public int _capsulesAmount = 0; 

    [SerializeField] Transform BrusherStick;
    [SerializeField] Transform BrusherActivePart;


    public static CapsuleManager Instance;
    public float triggerDistance = 0.2f;
    public Vector2 BrusherStickSize;
    public float BrusherActivePartRadius;
    public Material TargetColor;
    public int[] CapsulePhases;

    public bool[] isColored;

    public float growSpeed = 10;

    private const double ANGULAR_COEFFICIENT = 180/Math.PI;

    private void Start(){
        Instance = this;
        _capsules = this.GetComponentsInChildren<CapsuleClass>();
        _capsulesAmount = _capsules.Length;
        isColored = new bool[_capsulesAmount];
        capsuleRenderers = new MeshRenderer[_capsulesAmount];
        var transforms = new Transform[_capsules.Length] ;
        CapsulePhases = new int[_capsulesAmount];
        for(int i =0; i < _capsulesAmount;i++){
            isColored[i] = false;
            transforms[i] = _capsules[i].GetComponent<Transform>();
            capsuleRenderers[i] = _capsules[i].GetComponent<MeshRenderer>();
            CapsulePhases[i] = 0;
        }
        _allCapsulesTransform = transforms;
    }

    public void  Update(){

        for(int i = 0; i < _allCapsulesTransform.Length;i++){
            if(CapsulePhases[i]!=0){
                continue;
            }
            if(IsNear(_allCapsulesTransform[i])){
                CapsulePhases[i] = 1;
            }
        }

        for(int i = 0; i < _allCapsulesTransform.Length;i++){
             if(CapsulePhases[i]==0){
                continue;
            }
            if(isColored[i] == false){
                // Recalculate();
                ColorCapsule(capsuleRenderers[i]);
                isColored[i] = true;
            }
            var scale = _allCapsulesTransform[i].localScale;
            scale.x += Time.deltaTime*growSpeed*(CapsulePhases[i]==1?1:-1);
            scale.y += Time.deltaTime*growSpeed*(CapsulePhases[i]==1?1:-1);
            scale.z += Time.deltaTime*growSpeed*(CapsulePhases[i]==1?1:-1);
            if(scale.x >= 1.5f  && CapsulePhases[i] == 1){
                scale.x = 1.5f;
                scale.y = 1.5f;
                scale.z = 1.5f;
                CapsulePhases[i] = 2;
            }
            else if(scale.x <= 1 && CapsulePhases[i] == 2){
                scale.x = 1;
                scale.y = 1;
                scale.z = 1;
                CapsulePhases[i] = 0;
            }
            _allCapsulesTransform[i].localScale = scale;
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
    

     private bool IsNear(Transform transform){

        if((Mathf.Abs(transform.position.x - BrusherStick.position.x ) < (BrusherStickSize.x*Math.Cos(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT) +
        BrusherStickSize.y*Math.Sin(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT)/2)
         &&
        (Mathf.Abs(transform.position.z - BrusherStick.position.z ) < BrusherStickSize.x*Math.Sin(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT) +
        BrusherStickSize.y*Math.Cos(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT)/2)) ||
        ((Mathf.Abs(transform.position.x - BrusherActivePart.position.x ) < BrusherActivePartRadius*1.2) &&
            Mathf.Abs(transform.position.z - BrusherActivePart.position.z) < BrusherActivePartRadius*1.2))

        {
            return true;
        }
        return false;
    }

    private void ColorCapsule(MeshRenderer mesh){
        mesh.material = TargetColor;
    }
}
