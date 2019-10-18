using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Служебные переменные
    static Player m_instance;
    Move m_move;
    Attack m_attack;

    // Вызов до старта
    private void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
        //DontDestroyOnLoad(gameObject);
    }

    // Вызов при старте
    void Start()
    {
        m_move = GetComponent<Move>();
        m_attack = GetComponent<Attack>();
    }

    // Обращение к единственному объекту этого класса
    public static Player GetInstance()
    {
        return m_instance;
    }

    // Вызывается каждый кадр с параметрами ввода пользователя
    public void input(float horisontal, float vertical, bool space)
    {
        // Перемещение
        m_move?.MoveInput(horisontal, vertical);
        // Атака
        m_attack?.input(vertical, space);
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
