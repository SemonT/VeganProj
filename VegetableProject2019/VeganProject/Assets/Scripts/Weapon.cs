using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Параметры
    public Transform handle;
    public DamageDealer[] damageDealers;
    public string WeaponType;

    // Служебные переменные
    private bool m_isAttacking;
    public bool IsAttacking
    {
        set
        {
            m_isAttacking = value;
            SetDamageDealers(value);
        }
        get {
            return m_isAttacking;
        }
    }
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
    [HideInInspector] public bool isPicked = false;

    // Установка всех наносителей урона
    void SetDamageDealers(bool state)
    {
        foreach (DamageDealer dd in damageDealers)
        {
            dd.isActive = state;
        }
    }

    // Вызывается множество раз при атаке
    public void Input()
    {
        foreach (DamageDealer dd in damageDealers)
        {
            if (!dd.isActive) {
                SetDamageDealers(false);
                if (dd.StunTrigger)
                {
                    StunTrigger = true;
                }
                break;
            }
        }
    }

    // Получение типа оружия для определения анимации
    public string GetWeaponType()
    {
        return WeaponType;
    }
}
