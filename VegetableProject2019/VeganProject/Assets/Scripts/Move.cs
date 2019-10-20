using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Параметры
    public float speed = 10f;
    public GameObject jumpRayCastSource;
    public float jumpForce = 20f;

    // Служебные переменные
    Rigidbody2D m_rigidbody;
    Animator m_animator;
    bool dir = false;
    float jumpCooldown = 1f;
    float jumpTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    // Перемещение
    public void Input(float horisontal, float vertical)
    {
        // Обновление таймеров
        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;

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
        Debug.DrawLine(jumpRayCastSource.transform.position, jumpRayCastSource.transform.position - transform.up * jumpRayCastSource.transform.localScale.y, Color.green, Time.deltaTime); // Визуализация рейкаста
        RaycastHit2D hit = Physics2D.Raycast(jumpRayCastSource.transform.position, -jumpRayCastSource.transform.up, jumpRayCastSource.transform.localScale.x);
        if (hit.collider && hit.collider.gameObject.GetComponent<PlatformEffector2D>())
        {
            if (horisontal == 0)
            {
                m_animator.SetInteger("State", 1); // Стойка
            }
            else
            {
                m_animator.SetInteger("State", 2); // Ходьба
            }
            if (vertical > 0 && jumpTimer <= 0)
            {
                m_rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = jumpCooldown;
            }
        }
        else
        {
            m_animator.SetInteger("State", 3); // Падение
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
