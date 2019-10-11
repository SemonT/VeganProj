using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 1;
    public bool isActive = false;

    // Нанесение урона объектам со здоровьем
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health)
            {
                print(damage);
                //health.addDamage(damage);
            }
        }
    }


    void Start()
    {

    }
}
