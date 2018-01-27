using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingleton<T> where T: new()
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new T();
            return m_instance;
        }
    }
}
