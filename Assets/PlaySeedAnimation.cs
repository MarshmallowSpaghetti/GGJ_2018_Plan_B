using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySeedAnimation : MonoBehaviour {

    // Use this for initialization
    void Start() {
        SeedFlying();

    }

    // Update is called once per frame
   void  SeedFlying()
    {
        this.GetComponent<Animator>().Play("idle");
    }
}
