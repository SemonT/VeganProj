using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // Параметры
    public float blockCooldown = 5f;
    public Transform attachHandTransform;
    public Transform guideHandTransform;

    // Служебные переменные
    Animator m_animator;

    [HideInInspector] public Weapon m_weapon;
    GameObject m_weaponObject;
    Rigidbody2D m_weaponRigidBody;
    Transform m_weaponHandleTransform;
    DamageDealer[] m_weaponDamageDealers;
    string m_attackAnimType;
    string m_stunAnimType;

    AnimationClip[] clips;
    float attackCooldown;
    float attackTimer = 0;
    float stunCooldown;
    float stunTimer = 0;
    public string m_blockAnimType;
    float m_blockFullCooldown;
    float m_blockTimer = 0f;

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
        m_weapon = weapon;
        m_weapon.isPicked = true;
        m_weaponObject = m_weapon.gameObject;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), m_weaponObject.GetComponent<Collider2D>());
        m_weaponHandleTransform = m_weapon.handle;
        m_weaponDamageDealers = m_weapon.damageDealers;
        m_attackAnimType = "Attack" + m_weapon.GetWeaponType();
        m_stunAnimType = "Stun";
        m_blockAnimType = "Block";
        m_blockFullCooldown = blockCooldown;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == m_attackAnimType)
            {
                attackCooldown = clip.length;
            } else if (clip.name == m_stunAnimType)
            {
                stunCooldown = clip.length;
            } else if(clip.name == m_blockAnimType)
            {
                m_blockFullCooldown += clip.length;
            }

        }
    }

    // Выбрасывание оружия
    public void dropWeapon()
    {
        if(m_weapon)
        {
            m_weapon.isPicked = false;
            m_weaponObject.transform.eulerAngles = new Vector3(0, 0, 0);
            m_weapon = null;
            m_weaponHandleTransform = null;
            foreach (DamageDealer dd in m_weaponDamageDealers)
            {
                dd.isActive = false;
            }
            m_weaponDamageDealers = null;
            m_attackAnimType = "Attack";
            attackCooldown = 0;
            attackTimer = 0;
            stunCooldown = 0;
            stunTimer = 0;
        }
    }

    // Вызывается каждый кадр с параметрами ввода пользователя
    public void Input(bool attackKeyDown, bool blockKeyDown, bool pickupKeyDown)
    {
        if (m_weapon)
        {
            if (m_blockTimer > 0)
            {
                m_blockTimer -= Time.deltaTime;
            }
            else if (blockKeyDown)
            {
                if (!m_weaponRigidBody) m_weaponRigidBody = m_weaponObject.AddComponent<Rigidbody2D>();
                m_weaponRigidBody.gravityScale = 0;
                m_animator.SetTrigger(m_blockAnimType);
                m_blockTimer = m_blockFullCooldown;
            }
            else if (m_weaponRigidBody)
            {
                Destroy(m_weaponObject.GetComponent<Rigidbody2D>());
            }

            if (stunTimer > 0)
            {
                stunTimer -= Time.deltaTime;
            }
            if (attackTimer > 0)
            {
                m_weapon.Input();
                if (m_weapon.StunTrigger)
                {
                    stunTimer = stunCooldown;
                    m_animator.SetTrigger("Stun");
                }
                attackTimer -= Time.deltaTime;
            }
            else if (m_weapon.IsAttacking)
            {
                m_weapon.IsAttacking = false;
            }
            // Крепление оружия к рукам
            m_weaponObject.transform.position = new Vector3(
                attachHandTransform.position.x + m_weaponObject.transform.position.x - m_weaponHandleTransform.position.x,
                attachHandTransform.position.y + m_weaponObject.transform.position.y - m_weaponHandleTransform.position.y,
                (attachHandTransform.position.z + guideHandTransform.position.z) / 2f
                );
            Vector2 relativePos = attachHandTransform.position - guideHandTransform.position;
            m_weaponObject.transform.rotation = Quaternion.LookRotation(Vector3.Cross(relativePos, -attachHandTransform.transform.up), relativePos);
            //m_weapon.transform.rotation = Quaternion.LookRotation(Vector3.Cross(relativePos, -m_attachPointTransform.transform.up), relativePos);
            Debug.DrawLine(guideHandTransform.position, attachHandTransform.position, Color.blue, Time.deltaTime); // Визуализация направления оружия
            // Атака при нажатии на кнопку
            if (attackKeyDown && attackTimer <= 0 && stunTimer <= 0 && m_blockTimer <= 0)
            {
                m_animator.SetTrigger(m_attackAnimType);
                m_weapon.IsAttacking = true;
                attackTimer = attackCooldown;
            }
        }

        // Подбирание предметов
        if (pickupKeyDown && interactionWeapon)
        {
            dropWeapon();
            pickUpWeapon(interactionWeapon);
        }
    }

    // Для взаимодействия с другими объектами
    void OnTriggerEnter2D(Collider2D collision)
    {
        Weapon weap = collision.gameObject.GetComponent<Weapon>();
        if (!interactionWeapon && weap && !weap.isPicked)
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
