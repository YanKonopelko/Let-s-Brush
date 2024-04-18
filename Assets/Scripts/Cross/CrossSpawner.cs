using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrossSpawner : MonoBehaviour
{
    private static GameObject CrossPrefab;  

    private static List<GameObject> Crosses = new List<GameObject>();  

    public static void SpawnCrossesInPos(Transform pos){
        CrossPrefab = LevelManager.instance.CrossPrefab;
        GameObject.Find("Brusher").GetComponent<Brusher>().TurnOnCrosses();
        var cross1 = Instantiate(CrossPrefab,pos.position,Quaternion.identity);
        Crosses.Add(cross1);
        cross1.GetComponent<Animation>().Play("CrossAnimation 1");
        var cross2 = Instantiate(CrossPrefab, pos.position, Quaternion.identity);
        Crosses.Add(cross2);
        cross2.GetComponent<Animation>().Play("CrossAnimation 2");
        var cross3 = Instantiate(CrossPrefab, pos.position, Quaternion.identity);
        Crosses.Add(cross3);
        cross3.GetComponent<Animation>().Play("CrossAnimation 3");
            
    }

    public static void Clear(){
        for(int i = 0; i < Crosses.Count;i++){
            if(Crosses[i])
                Crosses[i].SetActive(false);

        }
    }
    public static void Pop(GameObject gameObject){
        Crosses.Remove(gameObject);
    }
}
