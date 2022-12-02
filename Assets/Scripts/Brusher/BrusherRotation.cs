using UnityEngine;
using UnityEngine.SceneManagement;


public class BrusherRotation : Brusher
{
    [SerializeField] private float _rotationSpeed = 175;
    static public bool isSwitched = true;
    private GameObject _camera; 
    public Transform[] _rotationObject; 
    private void Start()
    {
        AnimationNow = true;
        isSwitched = true;
        distance = 6.3f;
        _camera = Camera.main.gameObject;
        _camera.GetComponent<CameraController>().player = _rotationObject[0];

    }

    private void ChangeDirection()
    {
        isSwitched = !isSwitched;

        var pos = _rotationObject[0].localPosition;
        Vector3 newPos = new Vector3()
        {
            x = (isSwitched ? -1 : 1) * distance + pos.x
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
        transform.RotateAround(_rotationObject[0].position, Vector3.up * (isSwitched?1:-1), _rotationSpeed * Time.deltaTime);
    }
}
