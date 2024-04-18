using UnityEngine;

public class CrossSpawner : MonoBehaviour
{
    private static GameObject CrossPrefab;

    public static void SpawnCrossesInPos(Transform pos){

        CrossPrefab = LevelManager.instance.CrossPrefab;
        GameObject.Find("Brusher").GetComponent<Brusher>().TurnOnCrosses();
        var cross = Instantiate(CrossPrefab,pos.position,Quaternion.identity);
        cross.GetComponent<Animation>().Play("CrossAnimation 1");
        cross = Instantiate(CrossPrefab, pos.position, Quaternion.identity);
        cross.GetComponent<Animation>().Play("CrossAnimation 2");
        cross = Instantiate(CrossPrefab, pos.position, Quaternion.identity);
        cross.GetComponent<Animation>().Play("CrossAnimation 3");
            
        }
}
