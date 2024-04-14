using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public struct CapsuleAnimateJob : IJobParallelForTransform
{
    public NativeArray<int> CapsulePhases;
    public float deltaTime;
    public float growSpeed;
    public float targetSize;


    // public MeshRenderer[] renderer; 


    public void Execute(int index, TransformAccess transform){
        if(CapsulePhases[index]==0){
            return;
        }
        var scale = transform.localScale;
        scale.x += deltaTime*growSpeed*(CapsulePhases[index]==1?1:-1);
        scale.y += deltaTime*growSpeed*(CapsulePhases[index]==1?1:-1);
        scale.z += deltaTime*growSpeed*(CapsulePhases[index]==1?1:-1);
        if(scale.x >= 1.5f  && CapsulePhases[index] == 1){
            scale.x = 1.5f;
            scale.y = 1.5f;
            scale.z = 1.5f;
            CapsulePhases[index] = 2;
        }
        else if(scale.x <= 1 && CapsulePhases[index] == 2){
            scale.x = 1;
            scale.y = 1;
            scale.z = 1;
            CapsulePhases[index] = 0;
            // return;
        }
        transform.localScale = scale;
    }


}
