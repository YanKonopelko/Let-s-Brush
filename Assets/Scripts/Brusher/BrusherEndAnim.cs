using UnityEngine;
using DG.Tweening;

public class BrusherEndAnim : Brusher
{
    private float AnimationDuration = 0.5f;

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
}
