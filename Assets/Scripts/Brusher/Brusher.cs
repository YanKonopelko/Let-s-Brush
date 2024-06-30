using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Brusher : MonoBehaviour
{
    public static float distance = 6.4f;
    public static bool AnimationNow;

    public int _pickedCrosses = 0;
    [SerializeField] private UnityEngine.UI.Image[] _crosses;
    private float _buffDuration = 4.5f;
    // private float AnimationDuration = 0.4f;
    private float AnimationDuration = 0.5f;

    private Vector3 startPosition  = new Vector3(0,1.84f,0);

    public static bool isRotate = false;

    private BrusherRotation rot;

    private void Awake()
    {
        rot = GetComponent<BrusherRotation>();
    }

    public void EndAnim()
    {
        var obj = GetComponent<BrusherRotation>()._rotationObject[0];
        var pos = obj.localPosition;
        var Seq = DOTween.Sequence(); 
        Seq.Append(transform.GetChild(1).DOLocalMoveX(pos.x, AnimationDuration));
        Seq.Join(transform.GetChild(2).DOLocalMoveX(pos.x, AnimationDuration));
        Seq.Join(transform.GetChild(2).DOScaleZ(1, AnimationDuration));
        AnimationNow = true;
    }

    public bool CheckFloorAtThePoint(Transform point)
    {
        RaycastHit hit;
        Debug.DrawRay(point.position, point.TransformDirection(Vector3.down),new Color(1,1,1));
        return Physics.Raycast(point.position, point.TransformDirection(Vector3.down), out hit, 5f);
    }


     
    public void PickUp()
    {
        var newColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        _crosses[_pickedCrosses].color = newColor;
        _pickedCrosses += 1;
        if(_pickedCrosses >= 3)
        {
            _crosses[0].color = new Color(125 / 255f,108 / 255f,109 / 255f);
            _crosses[2].color = new Color(125 / 255f,108 / 255f,109 / 255f);
            _crosses[1].color = new Color(125 / 255f,108 / 255f,109 / 255f);
            _pickedCrosses = 0;
            _crosses[0].gameObject.SetActive(false);
            _crosses[1].gameObject.SetActive(false);
            _crosses[2].gameObject.SetActive(false);
            BrusherUp();
        }
    }
    public IEnumerator BrusherDown()
    {
        yield return new WaitForSeconds(_buffDuration);

        if (!AnimationNow)
        {
            distance = 6.4f;
            AnimationNow = true;
            var obj = GetComponent<BrusherRotation>()._rotationObject[0];
            var pos = obj.localPosition;
            Vector3 pos1 = new Vector3(0, 0, 0);
            pos1.x = (BrusherRotation.isSwitched ? 1 : -1) * distance + pos.x;

            var Seq = DOTween.Sequence();
            Seq.Append(transform.GetChild(1).DOLocalMoveX(pos1.x, AnimationDuration));
            Vector3 newpos = new Vector3(0, 0, 0);
            newpos.x = (BrusherRotation.isSwitched ? 1 : -1) * distance/2 + pos.x;
            Seq.Join(transform.GetChild(2).DOLocalMoveX(newpos.x, AnimationDuration));
            Seq.Join(transform.GetChild(2).DOScaleZ(200, AnimationDuration));


       
            StartCoroutine(Anim(false));
        }
    }
    private void BrusherUp()
    {
        if (!AnimationNow)
        {
            distance = 10.3f;
            AnimationNow = true;
            var Seq = DOTween.Sequence();
            Seq.Append(transform.GetChild(1).DOLocalMoveX(BrusherRotation.isSwitched ? 10.3f : -4.3f, AnimationDuration));
            Seq.Join(transform.GetChild(2).DOLocalMoveX(BrusherRotation.isSwitched ? 5.2f : 1.2f, AnimationDuration));
            Seq.Join(transform.GetChild(2).DOScaleZ(400, AnimationDuration));
            StartCoroutine(Anim(true));
            StartCoroutine(BrusherDown());
        }
    }
     IEnumerator Anim(bool brusherIsUp)
    {
        yield return new WaitForSeconds(AnimationDuration+0.1f);
        // if(brusherIsUp)
        CapsuleManager.Instance.RecalcTargetCapsules();
        AnimationNow = false;
    }
    public void TurnOnCrosses()
    {
        _crosses[0].gameObject.SetActive(true);
        _crosses[1].gameObject.SetActive(true);
        _crosses[2].gameObject.SetActive(true);
    }

    public void Reload(){
        isRotate = false;
        rot.Reload();
        transform.position = startPosition;

        // Task task = Task.Delay(100);
        // await task;
        _crosses[0].color = new Color(125 / 255f,108 / 255f,109 / 255f);
        _crosses[2].color = new Color(125 / 255f,108 / 255f,109 / 255f);
        _crosses[1].color = new Color(125 / 255f,108 / 255f,109 / 255f);
        _pickedCrosses = 0;
        _crosses[0].gameObject.SetActive(false);
        _crosses[1].gameObject.SetActive(false);
        _crosses[2].gameObject.SetActive(false);
        //Debug.Log(transform.position);
    }
    public void ForcedDown(){
        StopAllCoroutines();
        distance = 6.4f;
        var obj = GetComponent<BrusherRotation>()._rotationObject[0];
        var pos = obj.localPosition;
        Vector3 pos1 = transform.GetChild(1).transform.localPosition;
        pos1.x = (BrusherRotation.isSwitched ? 1 : -1) * distance + pos.x;

        transform.GetChild(1).transform.localPosition = pos1;
        Vector3 newpos = transform.GetChild(2).localPosition;
        newpos.x = (BrusherRotation.isSwitched ? 1 : -1) * distance/2 + pos.x;
        transform.GetChild(2).transform.localPosition = newpos;
        transform.GetChild(2).transform.localScale = new Vector3(transform.GetChild(2).transform.localScale.x,transform.GetChild(2).transform.localScale.y,200);
    }
}
