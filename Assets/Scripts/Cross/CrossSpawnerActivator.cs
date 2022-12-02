using UnityEngine;

public class CrossSpawnerActivator : MonoBehaviour
{ 
    void Start()
    {
        transform.GetChild(Random.Range(0, transform.childCount-1)).GetChild(0).GetComponent<CrossSpawner>()._enabled = true;
    }

}
