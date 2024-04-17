using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleSpawner : MonoBehaviour
{
    [SerializeField] GameObject _capsulePrefab; 
    private BoxCollider _collider;

    // [SerializeField] private float distanceFromBorder = 1;
    // [SerializeField] private float distanceBetweenCapsules = 2.5f;

    // Start is called before the first frame update
    void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        float biggestSize = Math.Max(_collider.bounds.size.x,_collider.bounds.size.z);
        if(_collider.bounds.size.x > _collider.bounds.size.z){
            
        }
        else{

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
