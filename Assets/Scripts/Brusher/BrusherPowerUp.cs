using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class  BrusherPowerUp : Brusher
{
    public int _pickedCrosses = 0;
    [SerializeField] private Image[] _crosses;
    private float _buffDuration = 4.5f;
    private float AnimationDuration = 0.4f;
    public void PickUp()
    {
        var newColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        _crosses[_pickedCrosses].color = newColor;
        _pickedCrosses += 1;
        if(_pickedCrosses == 3)
        {
            Destroy(_crosses[0]);
            Destroy(_crosses[1]);
            Destroy(_crosses[2]);
            BrusherUp();
        }
    }
    IEnumerator BrusherDown()
    {
        yield return new WaitForSeconds(_buffDuration);

        if (!AnimationNow)
        {
            var obj = GetComponent<BrusherRotation>()._rotationObject[0];
            var pos = obj.localPosition;
            Vector3 newpos = new Vector3(0, 0, 0);
            newpos.x = (BrusherRotation.isSwitched ? 1 : -1) * distance + pos.x;

            var Seq = DOTween.Sequence();
            Seq.Append(transform.GetChild(1).DOLocalMoveX(newpos.x, AnimationDuration));
            newpos.x = (BrusherRotation.isSwitched ? 1 : -1) * distance/2 + pos.x;
            Seq.Join(transform.GetChild(2).DOLocalMoveX(newpos.x, AnimationDuration));
            Seq.Join(transform.GetChild(2).DOScaleZ(200, AnimationDuration));
            AnimationNow = true;
            StartCoroutine(Anim(false));
        }
    }
    private void BrusherUp()
    {
        if (!AnimationNow)
        {
            var Seq = DOTween.Sequence();
            Seq.Append(transform.GetChild(1).DOLocalMoveX(BrusherRotation.isSwitched ? 10.3f : -4.3f, AnimationDuration));
            Seq.Join(transform.GetChild(2).DOLocalMoveX(BrusherRotation.isSwitched ? 5.2f : 1.2f, AnimationDuration));
            Seq.Join(transform.GetChild(2).DOScaleZ(400, AnimationDuration));
            AnimationNow = true;
            StartCoroutine(Anim(true));
            StartCoroutine(BrusherDown());
        }
    }
    IEnumerator Anim(bool brusherIsUp)
    {
        yield return new WaitForSeconds(AnimationDuration);
        if(brusherIsUp)
            distance = 10.3f;
        else
            distance = 6.3f;
        AnimationNow = false;
    }
    public void TurnOnCrosses()
    {
        _crosses[0].gameObject.SetActive(true);
        _crosses[1].gameObject.SetActive(true);
        _crosses[2].gameObject.SetActive(true);
    }
}
