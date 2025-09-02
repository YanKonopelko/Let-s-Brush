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
    
    // Optimization: Cache frequently used values
    private Square _cachedSquare;
    private Vector2 _cachedLeftUp, _cachedRightUp, _cachedLeftDown, _cachedRightDown;
    private Vector3 _cachedBrusherOffset;
    private float _cachedAngle;
    private Vector2 _cachedBrusherStickSize;
    private bool _squareNeedsUpdate = true;
    
    // Optimization: Cache circle positions and sizes
    private Vector3 _cachedCirclePos0, _cachedCirclePos1;
    private float _cachedCircleSize;
    private bool _circlesNeedUpdate = true;
    
    // Optimization: Cache capsule positions for collision detection
    private Vector2[] _capsulePositions;
    private bool _capsulePositionsNeedUpdate = true;
    
    // Optimization: Reduce update frequency
    private float _updateTimer = 0f;
    private const float UPDATE_INTERVAL = 0.016f; // ~60 FPS
    
    // Optimization: Pre-calculated constants
    private const double RAD_TO_ANGLE = Math.PI / 180;
    private const float HALF = 0.5f;
    
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
        
        // Initialize capsule positions cache
        _capsulePositions = new Vector2[_capsulesAmount];
        
        for (int i = 0; i < _capsulesAmount; i++)
        {
            isColored[i] = false;
            transforms[i] = _capsules[i].GetComponent<Transform>();
            capsuleRenderers[i] = _capsules[i].GetComponent<MeshRenderer>();
            CapsulePhases[i] = 0;
            ColorCapsule(capsuleRenderers[i], startCapsuleColor);
            
            // Cache initial capsule positions
            _capsulePositions[i] = new Vector2(transforms[i].position.x, transforms[i].position.z);
        }
        _allCapsulesTransform = transforms.ToList();
        startCapsulePos = _allCapsulesTransform[0].position;
        _targetCapsulesTransform = _allCapsulesTransform;
        CrossCapsuleNumber = UnityEngine.Random.Range(0, transform.childCount - 1);

    }
    private Square square;
    public void FixedUpdate()
    {
        if (!isStarted) return;
        
        // Optimization: Reduce update frequency
        _updateTimer += Time.fixedDeltaTime;
        if (_updateTimer < UPDATE_INTERVAL)
        {
            // Still update capsule scaling and flight even if collision detection is skipped
            UpdateCapsuleScaling();
            if (isFinished)
            {
                UpdateFinishAnimation();
            }
            return;
        }
        _updateTimer = 0f;
        
        if (!isFinished)
        {
            if (isRecalc) return;
            
            // Optimization: Only recalculate when brusher has moved significantly
            if (Vector3.Distance(lastBrusherPos, brusher.CirclePositions[1]) > 0.1f)
            {
                _squareNeedsUpdate = true;
                _circlesNeedUpdate = true;
                lastBrusherPos = brusher.CirclePositions[1];
            }
            
            // Optimization: Update collision detection only when needed
            if (_squareNeedsUpdate || _circlesNeedUpdate)
            {
                UpdateCollisionGeometry();
            }
            
            // Optimization: Update capsule positions cache
            if (_capsulePositionsNeedUpdate)
            {
                UpdateCapsulePositions();
            }

            // Optimization: Batch collision detection
            CheckCollisions();

            UpdateCapsuleScaling();
        }
        else
        {
            UpdateFinishAnimation();
        }
    }
    
    private void UpdateCollisionGeometry()
    {
        if (_squareNeedsUpdate)
        {
            _cachedAngle = brusher.Angle * (BrusherRotation.isSwitched ? -1 : -1);
            _cachedBrusherStickSize = new Vector2(brusher.StickSize.x, brusher.StickSize.y);
            _cachedBrusherOffset = brusher.StickPosition;
            
            // Pre-calculate trigonometric values
            float cosAngle = (float)Math.Cos(RAD_TO_ANGLE * _cachedAngle);
            float sinAngle = (float)Math.Sin(RAD_TO_ANGLE * _cachedAngle);
            
            // Calculate square corners with pre-calculated values
            float x = -HALF * _cachedBrusherStickSize.x;
            float y = HALF * _cachedBrusherStickSize.y;
            _cachedLeftUp = new Vector2(
                x * cosAngle - y * sinAngle,
                x * sinAngle + y * cosAngle
            );

            x = HALF * _cachedBrusherStickSize.x;
            y = HALF * _cachedBrusherStickSize.y;
            _cachedRightUp = new Vector2(
                x * cosAngle - y * sinAngle,
                x * sinAngle + y * cosAngle
            );

            x = -HALF * _cachedBrusherStickSize.x;
            y = -HALF * _cachedBrusherStickSize.y;
            _cachedLeftDown = new Vector2(
                x * cosAngle - y * sinAngle,
                x * sinAngle + y * cosAngle
            );

            x = HALF * _cachedBrusherStickSize.x;
            y = -HALF * _cachedBrusherStickSize.y;
            _cachedRightDown = new Vector2(
                x * cosAngle - y * sinAngle,
                x * sinAngle + y * cosAngle
            );

            // Add offset
            _cachedLeftUp += new Vector2(_cachedBrusherOffset.x, _cachedBrusherOffset.z);
            _cachedRightUp += new Vector2(_cachedBrusherOffset.x, _cachedBrusherOffset.z);
            _cachedLeftDown += new Vector2(_cachedBrusherOffset.x, _cachedBrusherOffset.z);
            _cachedRightDown += new Vector2(_cachedBrusherOffset.x, _cachedBrusherOffset.z);
            
            // Create square only when needed
            _cachedSquare = new Square(
                new PointInQuadrilateral.Point(_cachedLeftUp.x, _cachedLeftUp.y),
                new PointInQuadrilateral.Point(_cachedRightUp.x, _cachedRightUp.y),
                new PointInQuadrilateral.Point(_cachedLeftDown.x, _cachedLeftDown.y),
                new PointInQuadrilateral.Point(_cachedRightDown.x, _cachedRightDown.y)
            );
            
            _squareNeedsUpdate = false;
        }
        
        if (_circlesNeedUpdate)
        {
            _cachedCirclePos0 = brusher.CirclePositions[0];
            _cachedCirclePos1 = brusher.CirclePositions[1];
            _cachedCircleSize = brusher.CircleSize;
            _circlesNeedUpdate = false;
        }
        
        // Debug drawing (only in editor)
        #if UNITY_EDITOR
        Debug.DrawLine(new Vector3(_cachedLeftDown.x, 2, _cachedLeftDown.y), new Vector3(_cachedLeftUp.x, 2, _cachedLeftUp.y));
        Debug.DrawLine(new Vector3(_cachedLeftUp.x, 2, _cachedLeftUp.y), new Vector3(_cachedRightUp.x, 2, _cachedRightUp.y));
        Debug.DrawLine(new Vector3(_cachedRightUp.x, 2, _cachedRightUp.y), new Vector3(_cachedRightDown.x, 2, _cachedRightDown.y));
        Debug.DrawLine(new Vector3(_cachedRightDown.x, 2, _cachedRightDown.y), new Vector3(_cachedLeftDown.x, 2, _cachedLeftDown.y));
        #endif
    }
    
    private void UpdateCapsulePositions()
    {
        // Optimization: Only update positions for capsules that are still in phase 0 (not yet processed)
        for (int i = 0; i < _allCapsulesTransform.Count; i++)
        {
            if (CapsulePhases[i] == 0)
            {
                Vector3 pos = _allCapsulesTransform[i].position;
                _capsulePositions[i] = new Vector2(pos.x, pos.z);
            }
        }
        _capsulePositionsNeedUpdate = false;
    }
    
    private void CheckCollisions()
    {
        for (int i = 0; i < _targetCapsulesTransform.Count; i++)
        {
            if (CapsulePhases[i] != 0)
            {
                continue;
            }

            if (IsNearOptimized(i))
            {
                int index = _allCapsulesTransform.IndexOf(_targetCapsulesTransform[i]);
                CapsulePhases[index] = 1;
            }
        }
    }
    
    private void UpdateCapsuleScaling()
    {
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
    }
    
    private void UpdateFinishAnimation()
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

    // Optimization: Use cached positions and avoid object creation
    private bool IsNearOptimized(int capsuleIndex)
    {
        Vector2 capsulePos = _capsulePositions[capsuleIndex];

        // Check square collision
        if (PointInQuadrilateral.IsPointInside(_cachedSquare.LD, _cachedSquare.LU, _cachedSquare.RU, _cachedSquare.RD, 
            new PointInQuadrilateral.Point(capsulePos.x, capsulePos.y)))
        {
            return true;
        }
        
        // Check circle collisions with cached values
        if (PointInQuadrilateral.IsPointInsideCircle(
            new PointInQuadrilateral.Point(_cachedCirclePos0.x, _cachedCirclePos0.z), 
            _cachedCircleSize, 
            new PointInQuadrilateral.Point(capsulePos.x, capsulePos.y)))
        {
            return true;
        }
        
        if (PointInQuadrilateral.IsPointInsideCircle(
            new PointInQuadrilateral.Point(_cachedCirclePos1.x, _cachedCirclePos1.z), 
            _cachedCircleSize, 
            new PointInQuadrilateral.Point(capsulePos.x, capsulePos.y)))
        {
            return true;
        }
        
        return false;
    }

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
        // Optimization: Use squared distance to avoid square root calculation
        float dx = endAnimStarter.position.x - transform.position.x;
        float dz = endAnimStarter.position.z - transform.position.z;
        float squaredDistance = dx * dx + dz * dz;
        float squaredRadius = radius * radius;
        
        return squaredDistance <= squaredRadius;
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
        
        // Reset optimization flags
        _squareNeedsUpdate = true;
        _circlesNeedUpdate = true;
        _capsulePositionsNeedUpdate = true;
        _updateTimer = 0f;
        
        // Force update all capsule positions after reload
        ForceUpdateCapsulePositions();
        
        RecalcTargetCapsules();
    }
    
    private void ForceUpdateCapsulePositions()
    {
        for (int i = 0; i < _allCapsulesTransform.Count; i++)
        {
            Vector3 pos = _allCapsulesTransform[i].position;
            _capsulePositions[i] = new Vector2(pos.x, pos.z);
        }
    }

    public void RecalcTargetCapsules()
    {

        // isRecalc = true;
        // Vector3 pos = new Vector3(brusher.CirclePositions[0].x, brusher.CirclePositions[0].y, brusher.CirclePositions[0].z);
        // float radius = brusher.transform.localScale.z * 0.3f;
        // Debug.Log(radius);
        // // radius = radius*radius;
        // _targetCapsulesTransform.RemoveRange(0, _targetCapsulesTransform.Count);
        // for (int i = 0; i < _allCapsulesTransform.Count; i++)
        // {
        //     Transform capsule = _allCapsulesTransform[i];
        //     // if (Math.Abs(capsule.position.x - pos.x) <= radius && Math.Abs(capsule.position.z - pos.z) <= radius)
        //     // {
        //         _targetCapsulesTransform.Add(_allCapsulesTransform[i]);
        //     // }
        // }
        // if (lastBrusherPos == new Vector3(10000, 0, 10000))
        //     lastBrusherPos = brusher.CirclePositions[1];
        // isRecalc = false;
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
