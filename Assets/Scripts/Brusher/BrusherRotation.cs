using UnityEngine;
using UnityEngine.SceneManagement;


public class BrusherRotation : Brusher
{
    [SerializeField] private float _rotationSpeed = 175;
    static public bool isSwitched = true;
    private GameObject _camera; 
    public Transform[] _rotationObject; 

    [SerializeField] public Vector2 localPoses;
    private Vector3 direction = Vector3.up;
    private void Start()
    {
        AnimationNow = true;
        isSwitched = true;
        distance = 6.4f;
        _camera = Camera.main.gameObject;
        _camera.GetComponent<CameraController>().player = _rotationObject[0];

    }

    private void ChangeDirection()
    {
        if(AnimationNow) return;
        isSwitched = !isSwitched;

        var pos = _rotationObject[0].localPosition;
        Vector3 newPos = new Vector3()
        {
            x = (isSwitched ? -1 : 1) * Brusher.distance + pos.x
        };
        _rotationObject[0].localPosition = newPos;
        _rotationObject[1].localPosition = pos;
        
        if (!transform.GetComponent<FloorCheker>().CheckFloorAtThePoint(_rotationObject[0]))
        {
            AnimationNow = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Update()
    {
        if ((Input.GetKeyDown("k") || ( Input.touchCount!=0 && Input.GetTouch(0).phase == TouchPhase.Began)) && !AnimationNow)
        {
            ChangeDirection();
        }
        transform.RotateAround(_rotationObject[0].position, direction * (isSwitched?1:-1), _rotationSpeed * Time.deltaTime);
    }
}
