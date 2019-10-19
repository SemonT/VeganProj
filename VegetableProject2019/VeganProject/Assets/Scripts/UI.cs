using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Служеблые переменные
    static UI m_instance;

    void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Обращение к единственному объекту этого класса
    public static UI GetInstance()
    {
        return m_instance;
    }

    // Update is called once per frame
    void Update()
    {
        
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
