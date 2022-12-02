using UnityEngine;

public class GameCanvas : MonoBehaviour
{ 
    private static GameCanvas instance;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }
    
}
