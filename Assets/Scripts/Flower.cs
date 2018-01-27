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
                m_flowerPrefab = Resources.Load<GameObject>("Flower");
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.y < 0.5f)
        {
            print("Hit on the wall ");
            //collision.rigidbody.velocity = -collision.impulse * 10f;
            //collision.transform.position += collision.impulse.SetY(0) * 0.5f;
            return;
        }

        print("Reach flower " + this.name);
        GameObject.Instantiate(FlowerPrefab,
            transform.position
                + new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f)),
            Quaternion.identity,
            transform.parent);

    }
}
