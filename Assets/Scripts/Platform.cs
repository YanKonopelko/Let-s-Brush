using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private void OnEnable()
    {
        LevelManager.onParamsChange += ChangeColors;
    }

    private void OnDisable()
    {
        LevelManager.onParamsChange -= ChangeColors;
    }

    private void ChangeColors()
    {
        GetComponent<MeshRenderer>().materials[0].color= LevelManager.instance.PlatfomColor;
    }
}
