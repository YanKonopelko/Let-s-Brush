using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var bridge = new NativeArray<int>(2,Allocator.TempJob);
        bridge[0] = 11;
        var factorJob = new FactorialJob(){
            Bridge = bridge
        };
        Debug.Log("Before Job");
        JobHandle factHandle = factorJob.Schedule();
        factHandle.Complete();
        Debug.Log($"After Job {bridge[0]} and {bridge[1]}");

        bridge.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
