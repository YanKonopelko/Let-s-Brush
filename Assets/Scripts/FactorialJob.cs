using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct FactorialJob : IJob
{
    public NativeArray<int> Bridge;
    public void Execute(){
        Bridge[1] = Factorial(10);
    }

    private int Factorial(int num){
        if(num == 0)
            return 1;

        return num*Factorial(num-1);
    }

}