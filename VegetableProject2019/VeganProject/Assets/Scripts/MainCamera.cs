using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Служебные переменные
    static MainCamera m_instance;

    // Вызывается при старте
    private void Start()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
    }

    // Обращение к единственному объекту этого класса
    public static MainCamera GetInstance()
    {
        return m_instance;
    }

    // Настройка
    public static void Set(Transform tr)
    {
        m_instance.gameObject.transform.position = new Vector3(tr.position.x, tr.position.y, -10);
    }

    // Вызов при уничтожении объектов этого класса
    private void OnDestroy()
    {
        if (m_instance == this)
        {
            m_instance = null;
        }
    }
}
