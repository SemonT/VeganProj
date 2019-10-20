using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // Параметры
    public int damage = 1;
    public bool isActive = false;
    public bool isAlwaysActive = false;

    // Служебные переменные
    Collider2D m_collider;
    private bool m_stunTrigger;
    public bool StunTrigger
    {
        private set
        {
            m_stunTrigger = value;
        }
        get
        {
            bool t = m_stunTrigger;
            m_stunTrigger = false;
            return t;
        }
    }

    // Запускается при старте
    void Start()
    {

    }

    // Нанесение урона объектам со здоровьем
    void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive || isAlwaysActive)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health)
            {
                // Отталкивание
                //Vector3 collisionObjectPos = collision.gameObject.transform.position;
                //Vector3 pushDir = transform.forward + transform.up; ;
                //collision.gameObject.GetComponent<Rigidbody2D>().AddForce(pushDir * 10, ForceMode2D.Impulse);

                // Стан атакующего
                if (health.stunAttacker)
                {
                    StunTrigger = true;
                }

                // Определение точки удара
                Vector3 thisPos = transform.position;
                Vector3 otherPos = collision.transform.position;
                Vector3 dir = otherPos - thisPos;
                RaycastHit2D[] results = new RaycastHit2D[1];
                ContactFilter2D filter = new ContactFilter2D();
                filter.useTriggers = false;
                Physics2D.Raycast(thisPos, dir, filter, results);
                RaycastHit2D hit = results[0];
                // Вызов функции нанесения урона
                if (hit.collider)
                {
                    Debug.DrawLine(thisPos, hit.point, Color.cyan, 3); // Визуализация рейкаста снаружи коллайдера
                    Debug.DrawLine(hit.point, otherPos, Color.red, 3);// Визуализация рейкаста внутри коллайдера
                    health.addDamage(damage, hit.point);
                }
            }
            isActive = false;
        }
    }
}
