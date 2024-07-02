using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CapsuleManager : MonoBehaviour
{
    private CapsuleClass[] _capsules = Array.Empty<CapsuleClass>();
    private List<Transform> _allCapsulesTransform;

    private List<Transform> _targetCapsulesTransform = new List<Transform>();


    private MeshRenderer[] capsuleRenderers;

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


    private const float ANGULAR_COEFFICIENT = 57.29f;

    public bool isFinished = false;

    public Vector3 startCapsulePos;

    private float circleRadius = 0.5f;

    public Transform endAnimStarter;

    private int CrossCapsuleNumber = 0;

    public bool isStarted = false;
    public static bool isRecalc = false;

    public BrusherRotation brusher;

    private Vector3 lastBrusherPos = new Vector3(10000,0,10000);
    public void Init()
    {
        TargetColor = LevelManager.instance.CapsuleMaterials[1];
        startCapsuleColor = LevelManager.instance.CapsuleMaterials[0];
        Instance = this;
        _capsules = this.GetComponentsInChildren<CapsuleClass>();
        Instance._capsulesAmount = _capsules.Length;
        isColored = new bool[_capsulesAmount];
        capsuleRenderers = new MeshRenderer[_capsulesAmount];
        var transforms = new Transform[_capsules.Length];
        CapsulePhases = new int[_capsulesAmount];
        for (int i = 0; i < _capsulesAmount; i++)
        {
            isColored[i] = false;
            transforms[i] = _capsules[i].GetComponent<Transform>();
            capsuleRenderers[i] = _capsules[i].GetComponent<MeshRenderer>();
            CapsulePhases[i] = 0;
            ColorCapsule(capsuleRenderers[i], startCapsuleColor);
        }
        _allCapsulesTransform = transforms.ToList();
        startCapsulePos = _allCapsulesTransform[0].position;

        CrossCapsuleNumber = UnityEngine.Random.Range(0, transform.childCount - 1);

    }

    public void FixedUpdate()
    {
        if (!isStarted) return;
        if (!isFinished)
        {
            if (isRecalc) return;
            for (int i = 0; i < _targetCapsulesTransform.Count; i++)
            {

                if (CapsulePhases[i] != 0)
                {
                    continue;
                }

                // float angle = BrusherStick.eulerAngles.y * ANGULAR_COEFFICIENT;
                // float xSize = BrusherStick.transform.localScale.z/1.8f *0.02f;
                // float zSize = BrusherStick.transform.localScale.y/1.8f *0.02f;

                if (IsNear(_targetCapsulesTransform[i]))
                {
                    int index = _allCapsulesTransform.IndexOf(_targetCapsulesTransform[i]);
                    CapsulePhases[index] = 1;
                }
            }

            for (int i = 0; i < _allCapsulesTransform.Count; i++)
            {
                if (CapsulePhases[i] == 0)
                {
                    continue;
                }
                if (isColored[i] == false)
                {
                    _capsules[i].ParticlePlay();
                    ColorCapsule(capsuleRenderers[i], TargetColor);
                    Recalculate();
                    if (i == CrossCapsuleNumber)
                    {
                        CrossSpawner.SpawnCrossesInPos(_allCapsulesTransform[i].transform);
                    }
                    isColored[i] = true;
                }
                var scale = _allCapsulesTransform[i].localScale;
                scale.x += Time.deltaTime * capsuleGrowSpeed * (CapsulePhases[i] == 1 ? 1 : -1);
                scale.y += Time.deltaTime * capsuleGrowSpeed * (CapsulePhases[i] == 1 ? 1 : -1);
                scale.z += Time.deltaTime * capsuleGrowSpeed * (CapsulePhases[i] == 1 ? 1 : -1);
                if (scale.x >= 1.5f && CapsulePhases[i] == 1)
                {
                    scale.x = 1.5f;
                    scale.y = 1.5f;
                    scale.z = 1.5f;
                    CapsulePhases[i] = 2;
                }
                else if (scale.x <= 1 && CapsulePhases[i] == 2)
                {
                    scale.x = 1;
                    scale.y = 1;
                    scale.z = 1;
                    CapsulePhases[i] = 0;
                }
                _allCapsulesTransform[i].localScale = scale;
            }
            lastBrusherPos = brusher._rotationObject[1].position;
        }
        else
        {
            circleRadius += Time.deltaTime * finishCircleGrowSpeed;
            for (int i = 0; i < _allCapsulesTransform.Count; i++)
            {
                _allCapsulesTransform[i].localScale = new Vector3(1, 1, 1);
                if (CapsulePhases[i] >= 3)
                {
                    continue;
                }
                if (IsNearToCircle(_allCapsulesTransform[i], circleRadius))
                {
                    CapsulePhases[i] = 3;
                }
            }

            for (int i = 0; i < _allCapsulesTransform.Count; i++)
            {
                if (CapsulePhases[i] < 3)
                {
                    continue;
                }
                var position = _allCapsulesTransform[i].position;
                position.y += Time.deltaTime * capsuleFlightSpeed * (CapsulePhases[i] == 3 ? 1 : -1);
                if (position.y >= capsuleFlightHight)
                {
                    position.y = capsuleFlightHight;
                    CapsulePhases[i] = 4;
                }
                else if (position.y < startCapsulePos.y)
                {
                    position.y = startCapsulePos.y;
                }
                _allCapsulesTransform[i].position = position;
            }
        }


    }

    public async void Recalculate()
    {
        _capsulesCounter += 1;
        if (_capsulesCounter == _capsulesAmount)
        {
            brusher.EndAnim();
            await Task.Delay(500);
            isFinished = true;
            await Task.Delay(1300);
            LevelManager.instance.Finish();
        }
    }


    private bool IsNear(Transform transform)
    {

        // if ((Mathf.Abs(transform.position.x - BrusherStick.position.x) < xSize * Math.Cos(angle) + zSize * Math.Sin(angle) &&
        //  (Mathf.Abs(transform.position.z - BrusherStick.position.z) < xSize * Math.Sin(angle) + zSize * Math.Cos(angle))) ||

        // ((Mathf.Abs(transform.position.x - BrusherActivePart.position.x) < BrusherActivePartRadius) && Mathf.Abs(transform.position.z - BrusherActivePart.position.z) < BrusherActivePartRadius))
        // {
        //     return true;
        // }
        // return false;
        Vector3 capsulePos = transform.position;
        Vector3 rotObjPos =  new Vector3(brusher._rotationObject[0].position.x, brusher._rotationObject[0].position.y, brusher._rotationObject[0].position.z);;
        Vector3 curObjPos =  brusher._rotationObject[1].position;
        // float a = (rotObjPos.x - capsulePos.x)*(curObjPos.z-rotObjPos.z)-(curObjPos.x-rotObjPos.x)*(rotObjPos.z-capsulePos.z);
        // float b = (curObjPos.x - capsulePos.x)*(lastBrusherPos.z-curObjPos.z)-(lastBrusherPos.x-curObjPos.x)*(curObjPos.x-capsulePos.x);
        // float c = (lastBrusherPos.x-capsulePos.x)*(rotObjPos.z-lastBrusherPos.z)-(rotObjPos.x-lastBrusherPos.x)*(lastBrusherPos.z-capsulePos.z);

        // if(Math.Sign(a) == Math.Sign(b) && Math.Sign(b) == Math.Sign(c) ){
        //     return true;
        // }
        // else
        //     return false;


            // float a = (rotObjPos.x - capsulePos.x) * (curObjPos.z - rotObjPos.z) - (curObjPos.x - rotObjPos.x) * (rotObjPos.z - capsulePos.z);
            // float b = (curObjPos.x - capsulePos.x) * (lastBrusherPos.z - curObjPos.z) - (lastBrusherPos.x - curObjPos.x) * (curObjPos.z - capsulePos.z);
            // float c = (lastBrusherPos.x - capsulePos.x) * (rotObjPos.z - lastBrusherPos.z) - (rotObjPos.x - lastBrusherPos.x) * (lastBrusherPos.z - capsulePos.z);
            float a = (rotObjPos.x - capsulePos.x) * (curObjPos.z - rotObjPos.z) - (curObjPos.x - rotObjPos.x) * (rotObjPos.z - capsulePos.z);
            float b = (curObjPos.x - capsulePos.x) * (lastBrusherPos.z - curObjPos.z) - (lastBrusherPos.x - curObjPos.x) * (curObjPos.z - capsulePos.z);
            float c = (lastBrusherPos.x - capsulePos.x) * (rotObjPos.z - lastBrusherPos.z) - (rotObjPos.x - lastBrusherPos.x) * (lastBrusherPos.z - capsulePos.z);
            
            // Debug.Lowg("___________");
            if ((a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0) ||  ((Mathf.Abs(transform.position.x - BrusherActivePart.position.x) < BrusherActivePartRadius) && Mathf.Abs(transform.position.z - BrusherActivePart.position.z) < BrusherActivePartRadius))
            {
                return true;
            }
            else
            {
                return false;
            }

    }

    private bool IsNearToCircle(Transform transform, float radius)
    {

        if (Math.Sqrt(Math.Pow(endAnimStarter.position.x - transform.position.x, 2) + Math.Pow(endAnimStarter.position.z - transform.position.z, 2)) <= radius)
        {
            return true;
        }
        return false;
    }


    private void ColorCapsule(MeshRenderer mesh, Material mat)
    {
        mesh.material = mat;
    }

    public void Reload()
    {
        isStarted = false;
        _capsulesCounter = 0;
        for (int i = 0; i < CapsulePhases.Length; i++)
        {
            CapsulePhases[i] = 0;
            isColored[i] = false;
            var scale = new Vector3(1, 1, 1);
            _allCapsulesTransform[i].transform.localScale = scale;
            ColorCapsule(capsuleRenderers[i], startCapsuleColor);
        }
        circleRadius = 1;
        CrossCapsuleNumber = UnityEngine.Random.Range(0, transform.childCount - 1);
        lastBrusherPos = brusher._rotationObject[1].position;
        brusher.Reload();
        CrossSpawner.Clear();
        brusher.ForcedDown();
        isFinished = false;
        RecalcTargetCapsules();
    }

    public void RecalcTargetCapsules()
    {
        
        isRecalc = true;
        Vector3 pos = new Vector3(brusher._rotationObject[0].position.x, brusher._rotationObject[0].position.y, brusher._rotationObject[0].position.z);
        float radius = BrusherStick.transform.localScale.z * 0.3f ;
        Debug.Log(radius);
                // radius = radius*radius;
        _targetCapsulesTransform.RemoveRange(0, _targetCapsulesTransform.Count);
        for (int i = 0; i < _allCapsulesTransform.Count; i++)
        {
            Transform capsule = _allCapsulesTransform[i];
            if (Math.Abs(capsule.position.x - pos.x) <= radius && Math.Abs(capsule.position.z - pos.z) <= radius)
            {
                _targetCapsulesTransform.Add(_allCapsulesTransform[i]);
            }
        }
        if(lastBrusherPos == new Vector3(10000,0,10000))
            lastBrusherPos = brusher._rotationObject[1].position;
        isRecalc = false;
    }
}
