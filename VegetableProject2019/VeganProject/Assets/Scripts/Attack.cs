using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // Служебные переменные
    Transform m_attachPointTransform;
    Transform m_guidePointTransform;
    Animator m_animator;

    GameObject m_weapon;
    Transform m_weaponHandleTransform;
    DamageDealer m_weaponDamageDealer;
    string m_animType;

    AnimationClip[] clips;
    float damageDealerCooldown;
    float damageDealerTimer = 0;

    bool isAttacking = false;

    // Запускается при старте
    void Start()
    {
        Transform armsTransform = transform.Find("Arms");
        m_attachPointTransform = armsTransform.Find("AttachPoint");
        m_guidePointTransform = armsTransform.Find("GuidePoint");

        m_animator = GetComponent<Animator>();
        clips = m_animator.runtimeAnimatorController.animationClips;
    }

    // Инициализация параметров анимации и оружия при его подборе
    public void pickUpWeapon(Weapon weapon)
    {
        m_weapon = weapon.gameObject;
        m_weaponHandleTransform = m_weapon.transform.Find("Handle");
        m_weaponDamageDealer = m_weapon.transform.Find("DamageDealer").gameObject.GetComponent<DamageDealer>();
        m_animType = "Attack" + weapon.GetWeaponType();
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == m_animType)
            {
                damageDealerCooldown = clip.length;
                break;
            }
        }
    }

    // Выбрасывание оружия
    public void dropWeapon()
    {
        if (m_weapon) m_weapon.transform.eulerAngles = new Vector3(0, 0, 0);
        m_weapon = null;
        m_weaponHandleTransform = null;
        if (m_weaponDamageDealer) m_weaponDamageDealer.isActive = false;
        m_weaponDamageDealer = null;
        m_animType = "Attack";
        damageDealerCooldown = 0;
        damageDealerTimer = 0;
    }

    // Вызывается каждый кадр с параметрами ввода пользователя
    public void input(bool space)
    {
        if (m_weapon)
        {
            if (damageDealerTimer > 0)
            {
                damageDealerTimer -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
                m_weaponDamageDealer.isActive = false;
            }
            // Крепление оружия к рукам
            m_weapon.transform.position = new Vector3(
                m_attachPointTransform.position.x + m_weapon.transform.position.x - m_weaponHandleTransform.position.x,
                m_attachPointTransform.position.y + m_weapon.transform.position.y - m_weaponHandleTransform.position.y,
                (m_attachPointTransform.position.z + m_guidePointTransform.position.z) / 2f
                );

            Vector3 relativePos = m_attachPointTransform.position - m_guidePointTransform.position;
            m_weapon.transform.rotation = Quaternion.LookRotation(Vector3.Cross(relativePos, -m_attachPointTransform.transform.up), relativePos);
            Debug.DrawLine(m_guidePointTransform.position, m_attachPointTransform.position, Color.blue, Time.deltaTime); // Визуализация направления оружия
            // Атака при нажатии на кнопку
            if (space && !isAttacking)
            {
                m_animator.SetTrigger(m_animType);
                isAttacking = true;
                m_weaponDamageDealer.isActive = true;
                damageDealerTimer = damageDealerCooldown;
            }
        }
    }

    private void OnDestroy()
    {
        dropWeapon();
    }
}
