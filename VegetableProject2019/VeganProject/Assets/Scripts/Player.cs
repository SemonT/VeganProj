using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Параметры
    public float jumpForce = 20f;
    public float speed = 10f;
    
    // Служебные переменные
    static Player m_instance;
    Rigidbody2D m_rigidbody;
    Attack m_attack;
    Transform m_RCSGroundTransform;
    GameObject interactionObject;
    Animator m_animator;
    bool dir = false;
    float jumpCooldown = 1f;
    float jumpTimer = 0;

    // Вызов до старта
    private void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Вызов при старте
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

        m_attack = GetComponent<Attack>();

        Transform RCSParentTransform = transform.Find("RayCastSources");
        if (RCSParentTransform) {
            m_RCSGroundTransform = RCSParentTransform.Find("Ground");
        }

        m_animator = GetComponent<Animator>();
    }

    // Обращение к единственному объекту этого класса
    public static Player GetInstance()
    {
        return m_instance;
    }

    // Вызывается каждый кадр с параметрами ввода пользователя
    public void input(float horisontal, float vertical, bool space)
    {
        // Обновление таймеров
        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;

        // Ходьба
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.right * horisontal, speed * Time.deltaTime);

        // Говнокод begin
        float anim_param;
        if (horisontal == 0)
            anim_param = -1;
        else
            anim_param = 1;

        m_animator.SetFloat("State", anim_param);
        // Говнокод end

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
        Debug.DrawLine(m_RCSGroundTransform.position, m_RCSGroundTransform.position - transform.up * m_RCSGroundTransform.localScale.y, Color.green, Time.deltaTime); // Визуализация рейкаста
        RaycastHit2D hit = Physics2D.Raycast(m_RCSGroundTransform.position, -m_RCSGroundTransform.up, m_RCSGroundTransform.localScale.x);
        if (hit.collider && hit.collider.gameObject.GetComponent<PlatformEffector2D>())
        {
            if (vertical > 0 && jumpTimer <= 0)
            {
                m_rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = jumpCooldown;
            }
        }

        // Атака
        m_attack?.input(space);

        // Подбирание предметов
        if (vertical < 0 && interactionObject)
        {
            Weapon weap = interactionObject.GetComponent<Weapon>();
            if (weap)
            {
                m_attack.dropWeapon();
                m_attack.pickUpWeapon(interactionObject.GetComponent<Weapon>());
                weap.SetPicked(true);
            }
        }
    }

    // Для взаимодействия с другими объектами
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactionObject)
        {
            interactionObject = collision.gameObject;
        }
    }

    // Для невозможности воздействия с предметами издалека
    void OnTriggerExit2D(Collider2D collision)
    {
        interactionObject = null;
    }

    // Вызов при уничтожении объектов этого класса
    private void OnDestroy()
    {
        if (m_instance == this)
        {
            m_instance = null;
        }
    }
}
