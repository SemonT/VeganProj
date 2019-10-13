using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Параметры
    public int maxHealth = 100;
    public float deathTime = 5;
    [ColorUsage(true, true)] public Color juiceColor;
    public float bleedIntencity = 1;
    public float slicesGravity = 10;
    public float slicesScatter = 20;

    // Служебные переменные
    int health;
    List<Transform> slicesTransforms;
    List<Transform> coresTransforms;
    Texture2D texture;
    static GameObject m_bleedPrefab;
    static GameObject m_piesesPrefab;
    static Image m_playerHpBar;
    static Text m_playerHpText;
    Image hpBar;
    Text hpText;

    // Запускается при старте
    void Start()
    {
        if (GetComponent<Player>())
        {
            hpBar = m_playerHpBar;
            hpText = m_playerHpText;
            if (hpText) hpText.text = maxHealth.ToString();
        }
        health = maxHealth;

        slicesTransforms = new List<Transform>();
        Transform slices = transform.Find("Skin")?.Find("slices");
        if (slices)
        {
            slices.GetComponentsInChildren(false, slicesTransforms);
            slicesTransforms.Remove(slices);
        }

        coresTransforms = new List<Transform>();
        Transform cores = transform.Find("Skin")?.Find("core_body");
        if (cores)
        {
            cores.GetComponentsInChildren(false, coresTransforms);
            coresTransforms.Remove(cores);
        }

        texture = coresTransforms[0].GetComponent<SpriteRenderer>().sprite.texture;
    }

    // Инициализация префабов эффектов
    public static void Set(GameObject bleedPrefab, GameObject piesesPrefab, Image playerHpBar, Text playerHpText)
    {
        m_bleedPrefab = bleedPrefab;
        m_piesesPrefab = piesesPrefab;
        m_playerHpBar = playerHpBar;
        m_playerHpText = playerHpText;
    }

    void playDamageEffects(Vector3 point, Vector3 tearPoint)
    {
        Vector3 piecesPoint = point;
        Vector3 bleedPoint = tearPoint;
        // Если ничего не отвалилось, эффект удара проигрывается в месте попадания
        if (bleedPoint == new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
        {
            bleedPoint = point;
        }
        // Эффект попадания
        if (m_piesesPrefab)
        {
            GameObject curObj = Instantiate(m_piesesPrefab, piecesPoint, Quaternion.identity);

            // Восстановление масштаба
            ParticleSystem ps = curObj.GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule shape = ps.shape;
            shape.texture = texture;

            // Определение свойств частиц
            Vector3 scale = curObj.transform.localScale;
            curObj.transform.parent = transform;
            curObj.transform.localScale = scale;
        }
        // Эффект сокотечения
        if (m_bleedPrefab)
        {
            GameObject curObj = Instantiate(m_bleedPrefab, bleedPoint, Quaternion.identity);

            // Восстановление масштаба
            Vector3 scale = curObj.transform.localScale;
            curObj.transform.parent = transform.Find("Skin");
            curObj.transform.localScale = scale;

            // Определение свойств частиц
            ParticleSystem ps = curObj.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = juiceColor;
            main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constantMin, main.startSize.constantMax * bleedIntencity);
        }
    }

    // Отрывание частей
    Vector3 tearPiece(Vector3 point)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;
        foreach (Transform child in slicesTransforms)
        {
            float distance = (child.position - point).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = child;
            }
        }
        Vector3 position;
        if (nearest)
        {
            Debug.DrawLine(point, nearest.position, Color.yellow, 5f); // Указывание на отделяемый фрагмент
            nearest.position = new Vector3(nearest.position.x, nearest.position.y, nearest.position.z - 1);
            tearPiece(nearest.gameObject);

            position = nearest.position;
        }
        else
        {
            position = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }
        return position;
    }

    // Отрывание части
    void tearPiece(GameObject piece)
    {
        Rigidbody2D nearestRigidBody = piece.GetComponent<Rigidbody2D>();

        piece.transform.parent = null;
        if (!nearestRigidBody)
        {
            nearestRigidBody = piece.AddComponent<Rigidbody2D>();
        }
        nearestRigidBody.gravityScale = slicesGravity;
        nearestRigidBody.AddForce(Random.insideUnitCircle.normalized * slicesScatter, ForceMode2D.Impulse);
        slicesTransforms.Remove(piece.transform);

        

        Destroy(piece, deathTime);
    }

    // Нанесение урона / лечение
    public void addDamage(int damage, Vector3 point)
    {
        if (damage > 0)
        {
            playDamageEffects(point, tearPiece(point));
        }
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            death();
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (hpBar)
        {
            hpBar.fillAmount = (float)health / maxHealth;
        }
        if (hpText)
        {
            hpText.text = health.ToString();
        }
    }

    // Смерть персонажа
    void death()
    {
        List<Transform> objectsTransforms = new List<Transform>();
        gameObject.GetComponentsInChildren(false, objectsTransforms);
        foreach (Transform objectTransform in objectsTransforms)
        {
            tearPiece(objectTransform.gameObject);
        }
        //Animator animator = GetComponent<Animator>();
        //if (animator) animator.enabled = false;
        Destroy(gameObject);
    }
}
