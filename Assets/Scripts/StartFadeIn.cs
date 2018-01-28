using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFadeIn : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.Out, "Ending"));
    }
}
