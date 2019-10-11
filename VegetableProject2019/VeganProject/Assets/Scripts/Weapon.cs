using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string WeaponType;

    // Получение типа оружия для определения анимации
    public string GetWeaponType()
    {
        return WeaponType;
    }
    // Запуск при старте
    void Start()
    {
        
    }
}
