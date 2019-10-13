using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyAI : MonoBehaviour
{
    // Параметры
    public GameObject target;

    //[SerializeField] float jumpForce = 10f;
    [SerializeField] float speed = 7f;
    [SerializeField] float view_distance = 65f;

    // Служебные переменные
    Rigidbody2D m_rigidbody;
    Animator m_animator;
    Attack m_attack;

    Transform m_RCSGroundTransform;

    Transform m_TopEye;
    Transform m_BottomEye;
    Transform m_BackEye;

    bool dir = false;
    float dur = 1;
    //float jumpCooldown = 1f;
    //float jumpTimer = 0;

    void Start()
    {
//      m_rigidbody = GetComponent<Rigidbody2D>();

        m_animator = GetComponent<Animator>();
        m_attack = GetComponent<Attack>();

        Transform RCSParentTransform = transform.Find("RayCastSources");

        m_TopEye = RCSParentTransform.Find("FrontEyesTop");
        m_BottomEye = RCSParentTransform.Find("FrontEyesBottom");
        m_BackEye = RCSParentTransform.Find("BackEyes");

    }

    // Вызывается каждый кадр с параметрами 
    void MoveHorizontal(GameObject target)
    {
        float delta = transform.position.x - target.transform.position.x;

        m_animator.SetFloat("Move", Mathf.Abs(delta));

        //print("Дельта " + Mathf.Abs(delta));

        if(Mathf.Abs(delta) < 12f && Mathf.Abs(delta) > 2f)
        {
            //print("Атакую ");
            m_attack.input(true);
        }
        

        if (delta < 0 && !dir || delta > 0 && dir)
        {
            dir = !dir;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        Vector3 horiz_axis = new Vector3(0, 0, 0);
        if (delta > 10f)
        {
            horiz_axis = (target.transform.position - transform.position).normalized;
        }
        else if (delta < 8f)
        {
            horiz_axis = (transform.position - target.transform.position).normalized;
        }
        transform.position = Vector2.MoveTowards(transform.position, transform.position + horiz_axis, speed * Time.deltaTime);
    }

    GameObject FindTarget()
    {
        if (dir)
            dur = -1f;
        else
            dur = 1f;

        Physics2D.queriesHitTriggers = false;

        Vector2 direction = new Vector2(m_TopEye.position.x - view_distance * dur, m_TopEye.position.y - 9f);
        Debug.DrawLine(m_TopEye.position, direction, Color.red, Time.deltaTime);
        RaycastHit2D hitTop = Physics2D.Raycast(m_TopEye.position, direction, view_distance);

        direction = new Vector2(m_BottomEye.position.x - view_distance * dur, m_BottomEye.position.y + 9f);
        Debug.DrawLine(m_BottomEye.position, direction, Color.blue, Time.deltaTime);
        RaycastHit2D hitBottom = Physics2D.Raycast(m_TopEye.position, direction, view_distance);

        direction = new Vector2(m_BackEye.position.x + (view_distance * 0.3f * dur), m_BackEye.position.y);
        Debug.DrawLine(m_BackEye.position, direction, Color.green, Time.deltaTime);
        RaycastHit2D hitBack = Physics2D.Raycast(m_BackEye.position, direction, view_distance * 0.3f);

        


        if (hitTop.collider != null && hitTop.collider.gameObject != null && hitTop.collider.gameObject.GetComponent<Player>() != null)
        {
            //print("Найдено Верхним глазом " + hitTop.collider.gameObject.name);
            return hitTop.collider.gameObject;
        }

        if (hitBottom.collider != null && hitBottom.collider.gameObject != null && hitBottom.collider.gameObject.GetComponent<Player>() != null)
        {
            //print("Найдено Нижним глазом " + hitBottom.collider.gameObject.name);
            return hitBottom.collider.gameObject;
        }

        if (hitBack.collider != null && hitBack.collider.gameObject != null && hitBack.collider.gameObject.GetComponent<Player>() != null)
        {
            //print("Найдено жеппой " + hitBack.collider.gameObject.name);
            return hitBack.collider.gameObject;
        }

        //print("Не Найдено");
        return transform.gameObject;
    }

    private void Update()
    {

        m_attack.input(false);

        MoveHorizontal(FindTarget());


    }

    // Для взаимодействия с другими объектами
    void OnTriggerEnter2D(Collider2D collision)
    {
        Weapon weap = collision.gameObject.GetComponent<Weapon>();
        if (weap && !weap.GetPicked())
        {
            m_attack.pickUpWeapon(weap);
            weap.SetPicked(true);
        }
    }

}
