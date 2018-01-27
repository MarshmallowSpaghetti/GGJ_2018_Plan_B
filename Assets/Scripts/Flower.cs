using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    private GameObject m_flowerPrefab;

    public GameObject FlowerPrefab
    {
        get
        {
            if (m_flowerPrefab == null)
                m_flowerPrefab = Resources.Load<GameObject>("FlowerPref");
            return m_flowerPrefab;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.impulse.y < 0.5f)
    //    {
    //        print("Hit on the wall ");
    //        //collision.rigidbody.velocity = -collision.impulse * 10f;
    //        //collision.transform.position += collision.impulse.SetY(0) * 0.5f;
    //        return;
    //    }

    //    print("Reach flower " + this.name);
    //    GameObject.Instantiate(FlowerPrefab,
    //        transform.position
    //            + new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f)),
    //        Quaternion.identity,
    //        transform.parent);

    //}

    private void OnTriggerEnter(Collider other)
    {
        Action<Action> afterLanding = (_callback) =>
        {
            StartCoroutine(GrowNewOne(_callback));
        };

        if (other.transform.parent.CompareTag("Player"))
        {
            print("Enter flower area");
            other.transform.parent.GetComponent<PlayerMoveComponent>().StartWaitUntlLandOnSth(afterLanding);
        }
    }

    private IEnumerator GrowNewOne(Action _callbackFromPlayer)
    {
        GameObject newFlower = GameObject.Instantiate(FlowerPrefab,
                               transform.position
                                   + new Vector3(UnityEngine.Random.Range(-20f, 20f), 0, UnityEngine.Random.Range(0f, 40f)) + Vector3.down * 5f,
                               Quaternion.identity,
                               transform.parent);

        int cnt = 100;
        while (cnt > 0)
        {
            cnt--;
            newFlower.transform.position += Vector3.up * 0.05f;
            yield return null;
        }

        print("Grow done");
        if (_callbackFromPlayer != null)
            _callbackFromPlayer();
    }
}
