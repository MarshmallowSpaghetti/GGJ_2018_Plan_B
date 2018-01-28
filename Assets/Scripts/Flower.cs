using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Transform flowerCenter;

    private GameObject m_flowerPrefab;

    public GameObject FlowerPrefab
    {
        get
        {
            if (m_flowerPrefab == null)
                m_flowerPrefab = Resources.Load<GameObject>("Flower_Pref");
            return m_flowerPrefab;
        }
    }

    public bool HasLanded
    {
        get
        {
            return m_hasLanded;
        }

        set
        {
            if(m_hasLanded == false && value == true)
            {
                Collider[] colliders = GetComponentsInChildren<Collider>();
                for(int i = 0;i <colliders.Length;++i)
                {
                    GameObject.Destroy(colliders[i]);
                }
            }
            m_hasLanded = value;
        }
    }

    private PlayerMove m_contactPlayer;
    private bool m_hasLanded = false;

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
        Action afterLanding = () =>
        {
            HasLanded = true;
            StartCoroutine(MoveCameraToFlowerCenter(other.transform.parent.forward));
        };

        if (other.transform.parent.CompareTag("Player"))
        {
            print("Enter flower area");
            m_contactPlayer = other.transform.parent.GetComponent<PlayerMove>();
            m_contactPlayer.StartWaitUntlLandOnSth(afterLanding);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_contactPlayer != null)
        {
            if(m_contactPlayer != null)
            {
                m_contactPlayer.StopWaitUnitLandOnSth();
            }
        }
    }

    private IEnumerator MoveCameraToFlowerCenter(Vector3 _dir)
    {
        print("Move camera to center");
        yield return new WaitForSeconds(0.5f);

        print("Draw camera backward");
        // Draw the camera backward to see the whole flower.
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(GrowNewOne(_dir));

        // Make player moveable again
        m_contactPlayer.SetLaunchable();
    }

    private IEnumerator GrowNewOne(Vector3 _faceDir)
    {
        Vector3 newPos = Quaternion.AngleAxis(UnityEngine.Random.Range(-30, 30), Vector3.up) * _faceDir.normalized * UnityEngine.Random.Range(5f,20f) + transform.position;
        GameObject newFlower = GameObject.Instantiate(FlowerPrefab,
                               newPos,
                               Quaternion.LookRotation(new Vector3(UnityEngine.Random.Range(-1,1),0, UnityEngine.Random.Range(-1,1))),
                               transform.parent);
        newFlower.GetComponent<Animator>().Play("bloom");

        yield return new WaitForSeconds(0.5f);

        Vector3 newPos1 = Quaternion.AngleAxis(UnityEngine.Random.Range(-60, -45), Vector3.up) * _faceDir.normalized * UnityEngine.Random.Range(5f, 20f) + transform.position;
        GameObject newFlower1 = GameObject.Instantiate(FlowerPrefab,
                               newPos1,
                               Quaternion.LookRotation(new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1))),
                               transform.parent);
        newFlower1.GetComponent<Animator>().Play("bloom");
        yield return new WaitForSeconds(0.5f);

        Vector3 newPos2 = Quaternion.AngleAxis(UnityEngine.Random.Range(45, 60), Vector3.up) * _faceDir.normalized * UnityEngine.Random.Range(5f, 20f) + transform.position;
        GameObject newFlower2 = GameObject.Instantiate(FlowerPrefab,
                               newPos2,
                               Quaternion.LookRotation(new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1))),
                               transform.parent);
        newFlower2.GetComponent<Animator>().Play("bloom");
        yield return new WaitForSeconds(0.5f);

        print("Grow done");
    }
}
