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
                m_flowerPrefab = Resources.Load<GameObject>("Flower_Pref");
            return m_flowerPrefab;
        }
    }

    private PlayerMove m_contactPlayer;
    public bool hasLanded = false;

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
            hasLanded = true;
            StartCoroutine(MoveCameraToFlowerCenter(other.transform.forward));
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
        Vector3 newPos = Quaternion.AngleAxis(UnityEngine.Random.Range(-30, 30), Vector3.up) * _faceDir.normalized * 20 + transform.position + Vector3.down * 5f;
        GameObject newFlower = GameObject.Instantiate(FlowerPrefab,
                               newPos,
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
    }
}
