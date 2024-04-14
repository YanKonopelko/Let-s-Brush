using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public struct CapsuleJob : IJobParallelForTransform
{
    public Vector2 brusherActivePartPosition; 
    public Vector2 brusherStickPosition; 
    public Vector2 brusherStickSize; 
    public float brusherActivePartRadius;     
    public NativeArray<int> CapsulePhases;


    public void Execute(int index, TransformAccess transform){
        if(CapsulePhases[index]!=0){
            return;
        }
        if(IsNear(transform)){
            CapsulePhases[index] = 1;
        }

    }

    private bool IsNear(TransformAccess transform){

        if((Mathf.Abs(transform.position.x - brusherStickPosition.x ) < brusherStickSize.x &&
        Mathf.Abs(transform.position.z - brusherStickPosition.y ) < brusherStickSize.y) ||
        ((Mathf.Abs(transform.position.x - brusherActivePartPosition.x ) < brusherActivePartRadius) &&
            Mathf.Abs(transform.position.z - brusherActivePartPosition.y) < brusherActivePartRadius))

        {
            return true;
        }
        return false;
    }

}
