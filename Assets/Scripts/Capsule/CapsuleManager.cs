using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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


    public static CapsuleManager Instance;
    public Material TargetColor;
    public Material startCapsuleColor;
    public int[] CapsulePhases;

    public bool[] isColored;

    public float capsuleGrowSpeed = 10;
    public float finishCircleGrowSpeed = 10;
    public float capsuleFlightSpeed = 10;
    public float capsuleFlightHight = 3;

    public bool isFinished = false;

    public Vector3 startCapsulePos;

    private float circleRadius = 0.5f;

    public Transform endAnimStarter;

    private int CrossCapsuleNumber = 0;

    public bool isStarted = false;
    public static bool isRecalc = false;

    public BrusherRotation brusher;

    private Vector3 lastBrusherPos = new Vector3(10000, 0, 10000);
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
    private Square square;
    public void FixedUpdate()
    {
        if (!isStarted) return;
        if (!isFinished)
        {
            if (isRecalc) return;
            float angle = brusher.Angle * (BrusherRotation.isSwitched ? -1 : -1);
            var brusherStickSize = brusher.StickSize;
            float x = -0.5f * brusherStickSize.x;
            float y = +0.5f * brusherStickSize.y;
            Vector3 Offset = brusher.StickPosition;
            Vector2 leftUp = new Vector2((float)(x * Math.Cos(radToAngle * angle) - y * Math.Sin(radToAngle * angle)), (float)(x * Math.Sin(radToAngle * angle) + y * Math.Cos(radToAngle * angle)));

            x = 0.5f * brusherStickSize.x;
            y = 0.5f * brusherStickSize.y;
            Vector2 rightUp = new Vector2((float)(x * Math.Cos(radToAngle * angle) - y * Math.Sin(radToAngle * angle)), (float)(x * Math.Sin(radToAngle * angle) + y * Math.Cos(radToAngle * angle)));

            x = -0.5f * brusherStickSize.x;
            y = -0.5f * brusherStickSize.y;
            Vector2 leftDown = new Vector2((float)(x * Math.Cos(radToAngle * angle) - y * Math.Sin(radToAngle * angle)), (float)(x * Math.Sin(radToAngle * angle) + y * Math.Cos(radToAngle * angle)));

            x = +0.5f * brusherStickSize.x;
            y = -0.5f * brusherStickSize.y;
            Vector2 rightDown = new Vector2((float)(x * Math.Cos(radToAngle * angle) - y * Math.Sin(radToAngle * angle)), (float)(x * Math.Sin(radToAngle * angle) + y * Math.Cos(radToAngle * angle)));

            leftUp += new Vector2(Offset.x, Offset.z);
            rightUp += new Vector2(Offset.x, Offset.z);
            leftDown += new Vector2(Offset.x, Offset.z);
            rightDown += new Vector2(Offset.x, Offset.z);
            square = new Square(new PointInQuadrilateral.Point(leftUp.x, leftUp.y), new PointInQuadrilateral.Point(rightUp.x, rightUp.y), new PointInQuadrilateral.Point(leftDown.x, leftDown.y), new PointInQuadrilateral.Point(rightDown.x, rightDown.y));

            Debug.DrawLine(new Vector3(leftDown.x, 2, leftDown.y), new Vector3(leftUp.x, 2, leftUp.y));
            Debug.DrawLine(new Vector3(leftUp.x, 2, leftUp.y), new Vector3(rightUp.x, 2, rightUp.y));
            Debug.DrawLine(new Vector3(rightUp.x, 2, rightUp.y), new Vector3(rightDown.x, 2, rightDown.y));
            Debug.DrawLine(new Vector3(rightDown.x, 2, rightDown.y), new Vector3(leftDown.x, 2, leftDown.y));
            Debug.DrawLine(new Vector3(rightDown.x, 2, rightDown.y), new Vector3(leftDown.x, 2, leftDown.y));

            for (int i = 0; i < _targetCapsulesTransform.Count; i++)
            {

                if (CapsulePhases[i] != 0)
                {
                    continue;
                }

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
            lastBrusherPos = brusher.CirclePositions[1];
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
            await UniTask.Delay(500);
            isFinished = true;
            await UniTask.Delay(1300);
            LevelManager.instance.Finish();
        }
    }
    const double radToAngle = Math.PI / 180;

    private bool IsNear(Transform transform)
    {
        // return false;
        //  if (outPixelPainted[index]) return;
        bool retVal = false;
        PointInQuadrilateral.Point capsulePos = new PointInQuadrilateral.Point(transform.position.x, transform.position.z);



        if (PointInQuadrilateral.IsPointInside(square.LD, square.LU, square.RU, square.RD, capsulePos))
        {
            return true;
        }
        if (PointInQuadrilateral.IsPointInsideCircle(new PointInQuadrilateral.Point(brusher.CirclePositions[0].x, brusher.CirclePositions[0].z), brusher.CircleSize, capsulePos))
        {
            return true;
        }
        if (PointInQuadrilateral.IsPointInsideCircle(new PointInQuadrilateral.Point(brusher.CirclePositions[1].x, brusher.CirclePositions[1].z), brusher.CircleSize, capsulePos))
        {
            return true;
        }
        return retVal;
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
        lastBrusherPos = brusher.CirclePositions[1];
        brusher.Reload();
        CrossSpawner.Clear();
        brusher.ForcedDown();
        isFinished = false;
        RecalcTargetCapsules();
    }

    public void RecalcTargetCapsules()
    {

        isRecalc = true;
        Vector3 pos = new Vector3(brusher.CirclePositions[0].x, brusher.CirclePositions[0].y, brusher.CirclePositions[0].z);
        float radius = brusher.transform.localScale.z * 0.3f;
        Debug.Log(radius);
        // radius = radius*radius;
        _targetCapsulesTransform.RemoveRange(0, _targetCapsulesTransform.Count);
        for (int i = 0; i < _allCapsulesTransform.Count; i++)
        {
            Transform capsule = _allCapsulesTransform[i];
            // if (Math.Abs(capsule.position.x - pos.x) <= radius && Math.Abs(capsule.position.z - pos.z) <= radius)
            // {
                _targetCapsulesTransform.Add(_allCapsulesTransform[i]);
            // }
        }
        if (lastBrusherPos == new Vector3(10000, 0, 10000))
            lastBrusherPos = brusher.CirclePositions[1];
        isRecalc = false;
    }
    
   private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(brusher.CirclePositions[0], brusher.CircleSize);
        Gizmos.DrawWireSphere(brusher.CirclePositions[1], brusher.CircleSize);

        // Gizmos.color = Color.green;
        // int segments = 20;
        // Vector3 prev = startPos;
        // for (int i = 1; i <= segments; i++)
        // {
        //     float t = i / (float)segments;
        //     Vector3 point = CalculateBezierPoint(t, startPos, controlPoint, target);
        //     Gizmos.DrawLine(prev, point);
        //     prev = point;
        // }

        // Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(controlPoint, 0.2f);
    }
}
