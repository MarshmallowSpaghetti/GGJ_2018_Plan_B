using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingletonMono<T> : MonoBehaviour where T: BaseSingletonMono<T>
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<T>();
            return m_instance;
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            m_instance = null;
        }
    }
}
