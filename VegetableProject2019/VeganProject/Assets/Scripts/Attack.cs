using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // Служебные переменные
    public Transform attachHandTransform;
    public Transform guideHandTransform;
    Animator m_animator;

    GameObject m_weapon;
    Transform m_weaponHandleTransform;
    DamageDealer m_weaponDamageDealer;
    string m_animType;

    AnimationClip[] clips;
    float damageDealerCooldown;
    float damageDealerTimer = 0;

    bool isAttacking = false;

    Weapon interactionWeapon;

    // Запускается при старте
    void Start()
    {
        m_animator = GetComponent<Animator>();
        clips = m_animator.runtimeAnimatorController.animationClips;
    }

    // Инициализация параметров анимации и оружия при его подборе
    public void pickUpWeapon(Weapon weapon)
    {
        weapon.SetPicked(true);
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
        if(m_weapon)
        {
            m_weapon?.GetComponent<Weapon>().SetPicked(false);
            if (m_weapon) m_weapon.transform.eulerAngles = new Vector3(0, 0, 0);
            m_weapon = null;
            m_weaponHandleTransform = null;
            if (m_weaponDamageDealer) m_weaponDamageDealer.isActive = false;
            m_weaponDamageDealer = null;
            m_animType = "Attack";
            damageDealerCooldown = 0;
            damageDealerTimer = 0;
        }
    }

    // Вызывается каждый кадр с параметрами ввода пользователя
    public void input(float vertical, bool space)
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
                attachHandTransform.position.x + m_weapon.transform.position.x - m_weaponHandleTransform.position.x,
                attachHandTransform.position.y + m_weapon.transform.position.y - m_weaponHandleTransform.position.y,
                (attachHandTransform.position.z + guideHandTransform.position.z) / 2f
                );
            Vector2 relativePos = attachHandTransform.position - guideHandTransform.position;
            m_weapon.transform.rotation = Quaternion.LookRotation(Vector3.Cross(relativePos, -attachHandTransform.transform.up), relativePos);
            //m_weapon.transform.rotation = Quaternion.LookRotation(Vector3.Cross(relativePos, -m_attachPointTransform.transform.up), relativePos);
            Debug.DrawLine(guideHandTransform.position, attachHandTransform.position, Color.blue, Time.deltaTime); // Визуализация направления оружия
            // Атака при нажатии на кнопку
            if (space && !isAttacking)
            {
                m_animator.SetTrigger(m_animType);
                isAttacking = true;
                m_weaponDamageDealer.isActive = true;
                damageDealerTimer = damageDealerCooldown;
            }
        }

        // Подбирание предметов
        if (vertical < 0 && interactionWeapon)
        {
            dropWeapon();
            pickUpWeapon(interactionWeapon);
        }
    }

    // Для взаимодействия с другими объектами
    void OnTriggerEnter2D(Collider2D collision)
    {
        Weapon weap = collision.gameObject.GetComponent<Weapon>();
        if (!interactionWeapon && weap && !weap.GetPicked())
        {
            interactionWeapon = weap;
        }
    }

    // Для невозможности воздействия с предметами издалека
    void OnTriggerExit2D(Collider2D collision)
    {
        Weapon weap = collision.gameObject.GetComponent<Weapon>();
        if (weap == interactionWeapon)
        {
            interactionWeapon = null;
        }
    }

    // Вызов при смерти
    private void OnDestroy()
    {
        dropWeapon();
    }
}
