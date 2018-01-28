using System.Collections;
using System.Collections.Generic;
using TasiYokan.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        new SingleAudio("Enochian_Magic", AudioLayer.Bgm).Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        print("Enter exit");
        if (other.transform.parent.CompareTag("Player"))
        {
            SceneManager.LoadScene("Ending", LoadSceneMode.Single);
        }
    }
}
