using Unity.VisualScripting;
using UnityEngine;

public class CrossSpawnerActivator : MonoBehaviour
{ 
    void Start()
    {
        var a = Random.Range(0, transform.childCount - 1);
        transform.GetChild(a).GetChild(0).AddComponent<CrossSpawner>();
        Debug.Log(transform.GetChild(a).name);
    }

}
