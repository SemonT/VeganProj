using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Служебные переменные
    bool isPicked = false;
    public string WeaponType;

    // Установка подобранности оружия
    public void SetPicked(bool p)
    {
        isPicked = p;
    }

    // Проверка на подобранность оружия
    public bool GetPicked()
    {
        return isPicked;
    }

    // Запуск при старте
    void Start()
    {

    }

    // Получение типа оружия для определения анимации
    public string GetWeaponType()
    {
        return WeaponType;
    }
}
