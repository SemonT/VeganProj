﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Служебные переменные
    public string WeaponType;

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