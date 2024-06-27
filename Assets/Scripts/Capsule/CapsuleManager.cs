using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

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
    public BoxCollider BrusherStickSize;
    public float BrusherActivePartRadius;
    public Material TargetColor;
    public Material startCapsuleColor;
    public int[] CapsulePhases;

    public bool[] isColored;

    public float capsuleGrowSpeed = 10;
    public float finishCircleGrowSpeed = 10;
    public float capsuleFlightSpeed = 10;
    public float capsuleFlightHight = 3;


    private const double ANGULAR_COEFFICIENT = 180/Math.PI;

    public bool isFinished = false;

    public Vector3 startCapsulePos;

    private float circleRadius = 0.5f;

    public Transform endAnimStarter;

    private int CrossCapsuleNumber = 0;

    public bool isStarted = false;

    public Brusher brusher;
    public void Init(){
        TargetColor = LevelManager.instance.CapsuleMaterials[1];
        startCapsuleColor = LevelManager.instance.CapsuleMaterials[0];
        Instance = this;
        _capsules = this.GetComponentsInChildren<CapsuleClass>();
        Instance._capsulesAmount = _capsules.Length;
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
        startCapsulePos = _allCapsulesTransform[0].position;

        CrossCapsuleNumber = UnityEngine.Random.Range(0, transform.childCount - 1);
        Debug.Log(CrossCapsuleNumber);

    }

    public void  Update(){
    if(!isStarted) return;
    if(!isFinished)
    {
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
                    _capsules[i].ParticlePlay();
                    ColorCapsule(capsuleRenderers[i],TargetColor);
                    Recalculate();
                    if(i == CrossCapsuleNumber){
                        CrossSpawner.SpawnCrossesInPos(_allCapsulesTransform[i].transform);
                    }
                    isColored[i] = true;
                }
                var scale = _allCapsulesTransform[i].localScale;
                scale.x += Time.deltaTime*capsuleGrowSpeed*(CapsulePhases[i]==1?1:-1);
                scale.y += Time.deltaTime*capsuleGrowSpeed*(CapsulePhases[i]==1?1:-1);
                scale.z += Time.deltaTime*capsuleGrowSpeed*(CapsulePhases[i]==1?1:-1);
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
    else{
            circleRadius += Time.deltaTime*finishCircleGrowSpeed;
            for(int i = 0; i < _allCapsulesTransform.Length;i++){      
                 _allCapsulesTransform[i].localScale = new Vector3(1,1,1);
                 if(CapsulePhases[i]>=3){
                    continue;
                } 
                if(IsNearToCircle(_allCapsulesTransform[i])){
                    CapsulePhases[i] = 3;
                }
            }

        for(int i = 0; i < _allCapsulesTransform.Length;i++){
                 if(CapsulePhases[i]<3){
                    continue;
                }
                var position = _allCapsulesTransform[i].position;
                position.y += Time.deltaTime*capsuleFlightSpeed*(CapsulePhases[i]==3?1:-1);
                if(position.y >= capsuleFlightHight){
                    position.y = capsuleFlightHight;
                    CapsulePhases[i]= 4;
                }
                else if(position.y < startCapsulePos.y){
                    position.y = startCapsulePos.y;
                }
                _allCapsulesTransform[i].position = position;
            }
    }



    }

    public async void Recalculate() {
        _capsulesCounter += 1;
        if(_capsulesCounter == _capsulesAmount)
        {
            brusher.EndAnim();
            await Task.Delay(500);
            isFinished = true;
            LevelManager.instance.Finish();
        }
    }
    

     private bool IsNear(Transform transform){

        if((Mathf.Abs(transform.position.x - BrusherStick.position.x ) < (BrusherStickSize.size.x*BrusherStickSize.transform.localScale.x*1.3*Math.Cos(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT) +
        BrusherStickSize.size.z*BrusherStickSize.transform.localScale.z*Math.Sin(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT)/2)
         &&
        (Mathf.Abs(transform.position.z - BrusherStick.position.z ) < BrusherStickSize.size.x*BrusherStickSize.transform.localScale.x*1.3*Math.Sin(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT) +
        BrusherStickSize.size.z*BrusherStickSize.transform.localScale.z*Math.Cos(BrusherStick.eulerAngles.y*ANGULAR_COEFFICIENT)/2)) ||
        ((Mathf.Abs(transform.position.x - BrusherActivePart.position.x ) < BrusherActivePartRadius*1.2) &&
            Mathf.Abs(transform.position.z - BrusherActivePart.position.z) < BrusherActivePartRadius*1.2))

        {
            return true;
        }
        return false;
    }

    private bool IsNearToCircle(Transform transform){

        if(  Math.Sqrt(Math.Pow(endAnimStarter.position.x - transform.position.x,2) + Math.Pow(endAnimStarter.position.z - transform.position.z,2))  <= circleRadius)
        {
            return true;
        }
        return false;
    }


    private void ColorCapsule(MeshRenderer mesh,Material mat){
        mesh.material = mat;
    }

    public void Reload(){
        isStarted = false;
        _capsulesCounter = 0;
        for(int i = 0; i < CapsulePhases.Length;i++){
            CapsulePhases[i] = 0;
            isColored[i] = false;
            var scale = new Vector3(1,1,1);
            _allCapsulesTransform[i].transform.localScale = scale;
            ColorCapsule(capsuleRenderers[i],startCapsuleColor);
        }
        circleRadius = 1;
        CrossCapsuleNumber = UnityEngine.Random.Range(0, transform.childCount - 1);
        Debug.Log(CrossCapsuleNumber);
        brusher.Reload();
        CrossSpawner.Clear();
        brusher.ForcedDown();
        isFinished = false;
    }   
}
