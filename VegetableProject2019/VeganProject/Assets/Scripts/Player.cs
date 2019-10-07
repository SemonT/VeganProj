using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpForce = 100;
    public float speed = 5;
    bool dir = false;

    static Player m_instance;
    Rigidbody2D m_rigidbody;
    Transform RCSParentTransform; // Используется только при инициализации (RCS - Ray Cast Source)
    Transform RCSGroundTransform;

    // Вызов до старта
    private void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
    }

    // Вызов при старте
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        RCSParentTransform = transform.Find("RayCastSources");
        RCSGroundTransform = RCSParentTransform.Find("Ground");
    }

    // Обращение к объекту этого класса
    public static Player GetInstance()
    {
        return m_instance;
    }

    // Вызывается каждый кадр с параметрами ввода пользователя
    public void input(float horisontal, bool space)
    {
        // Ходьба
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.right * horisontal, speed * Time.deltaTime);

        // Поворот
        if (horisontal != 0)
        {
            if (horisontal > 0 && !dir || horisontal < 0 && dir)
            {
                dir = !dir;
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }

        // Прыжки
        Debug.DrawLine(RCSGroundTransform.position, RCSGroundTransform.position - transform.up * RCSGroundTransform.localScale.y, Color.green, Time.deltaTime); // Визуализация рейкаста
        RaycastHit2D hit = Physics2D.Raycast(RCSGroundTransform.position, -RCSGroundTransform.up, RCSGroundTransform.localScale.x);
        if (hit.collider && hit.collider.gameObject.GetComponent<PlatformEffector2D>())
        {
            if (space)
            {
                m_rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
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
