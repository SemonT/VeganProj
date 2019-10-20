using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyAI : MonoBehaviour
{
    // Параметры
    [SerializeField] float view_distance = 65f;

    // Служебные переменные
    Move moventer;

    Rigidbody2D m_rigidbody;
    Animator m_animator;
    Attack m_attack;

    Transform m_RCSGroundTransform;

    Transform m_TopEye;
    Transform m_BottomEye;
    Transform m_BackEye;

    GameObject target;

    float dur = 1;

    void Start()
    {
        moventer = GetComponent<Move>();

        m_rigidbody = GetComponent<Rigidbody2D>();

        m_animator = GetComponent<Animator>();
        m_attack = GetComponent<Attack>();

        Transform RCSParentTransform = transform.Find("RayCastSources");

        m_TopEye = RCSParentTransform.Find("FrontEyesTop");
        m_BottomEye = RCSParentTransform.Find("FrontEyesBottom");
        m_BackEye = RCSParentTransform.Find("BackEyes");

    }

    // Вызывается каждый кадр с параметрами 
    float MoveHorizontal(GameObject target)
    {
        float deltaX = transform.position.x - target.transform.position.x;

        float mod = Mathf.Abs(deltaX);

        if (mod < 13f && mod > 2f && m_attack.m_weapon.IsAttacking == false)
        {
            m_attack.Input(true, false, false);
        }

//        Vector2 horizon_move = new Vector2(deltaX, 0);
        if (mod < 11.2f && mod > 10.5f)
        {
            return 0;
        } else if (mod < 10.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.right * deltaX, moventer.speed * Time.deltaTime);
            return 0;
        }
        
        return deltaX * (-1);
    }


    float MoveVertical(GameObject target)
    {
        /*
        Physics2D.queriesHitTriggers = true;

        Vector2 vect = new Vector2(m_TopEye.position.x, m_TopEye.position.y + 0.5f);

        Vector2 direction = new Vector2(m_TopEye.position.x - view_distance * dur * 0.2f, m_TopEye.position.y - 5f);
        Debug.DrawLine(vect, direction, Color.magenta, Time.deltaTime);
        RaycastHit2D hitTop = Physics2D.Raycast(vect, direction, view_distance * 0.2f);
        */

        float deltaY = transform.position.y - target.transform.position.y;
        /*//print(deltaY);
        if (hitTop.collider)
            print(" " + hitTop.collider.gameObject);
        else
            print(null);
        */
        
        if (/*hitTop.collider && hitTop.collider.gameObject.GetComponent<PlatformEffector2D>() && */deltaY < -2f)
        {
            //print("Прыжок");
            return deltaY * (-1);
        }

        return 0;
    }

    GameObject FindTarget()
    {
        if (moventer.dir)
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
        //    print("Найдено Верхним глазом " + hitTop.collider.gameObject.name);
            return hitTop.collider.gameObject;
        }

        if (hitBottom.collider != null && hitBottom.collider.gameObject != null && hitBottom.collider.gameObject.GetComponent<Player>() != null)
        {
        //    print("Найдено Нижним глазом " + hitBottom.collider.gameObject.name);
            return hitBottom.collider.gameObject;
        }

        if (hitBack.collider != null && hitBack.collider.gameObject != null && hitBack.collider.gameObject.GetComponent<Player>() != null)
        {
        //    print("Найдено жеппой " + hitBack.collider.gameObject.name);
            return hitBack.collider.gameObject;
        }

        //print("Не Найдено");
        return transform.gameObject;
    }

    private void Update()
    {
        target = FindTarget();

        moventer.Input(MoveHorizontal(target), MoveVertical(target));
        
        m_attack.Input(false, false, true);

    }

}
