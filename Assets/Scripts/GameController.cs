using System.Collections;
using System.Collections.Generic;
using TasiYokan.Audio;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        new SingleAudio("BGM_LOOP", AudioLayer.Bgm).Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
