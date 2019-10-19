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

    // Параметры
    public Transform[] primeterSlices;
    public Transform[] coreSlices;

    // Служебные переменные
    int health;
    Texture2D texture;
    static GameObject m_bleedPrefab;
    static GameObject m_piesesPrefab;
    static float m_slicesGravity;
    static float m_slicesScatter;
    static float m_slicesMaxRotation;
    static float m_slicesScale;
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

        if (0 < primeterSlices.Length)
            texture = primeterSlices[0].GetComponent<SpriteRenderer>().sprite.texture;
    }

    // Инициализация префабов эффектов
    public static void Set(GameObject bleedPrefab, GameObject piesesPrefab, float slicesGravity, float slicesScatter, float slicesMaxRotation, float slicesScale, Image playerHpBar, Text playerHpText)
    {
        m_bleedPrefab = bleedPrefab;
        m_piesesPrefab = piesesPrefab;
        m_slicesGravity = slicesGravity;
        m_slicesScatter = slicesScatter;
        m_slicesMaxRotation = slicesMaxRotation;
        m_slicesScale = slicesScale;
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
        foreach (Transform slice in primeterSlices)
        {
            if (slice)
            {
                float distance = (slice.position - point).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = slice;
                }
            }
        }
        Vector3 position;
        if (nearest)
        {
            Debug.DrawLine(point, nearest.position, Color.yellow, 5f); // Указывание на отделяемый фрагмент
            nearest.position = new Vector3(nearest.position.x, nearest.position.y, nearest.position.z - 1);
            position = nearest.position;
            tearPiece(nearest.gameObject);
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
        piece.transform.localScale = piece.transform.localScale * m_slicesScale;
        if (!nearestRigidBody)
        {
            nearestRigidBody = piece.AddComponent<Rigidbody2D>();
        }
        nearestRigidBody.gravityScale = m_slicesGravity;
        nearestRigidBody.AddForce(Random.insideUnitCircle.normalized * m_slicesScatter, ForceMode2D.Impulse);
        nearestRigidBody.AddTorque((Random.value - 0.5f) * m_slicesMaxRotation, ForceMode2D.Impulse);

        for (int i = 0; i < primeterSlices.Length; i++)
        {
            if (primeterSlices[i] == piece)
            {
                primeterSlices[i] = null;
            }
        }
        for (int i = 0; i < coreSlices.Length; i++)
        {
            if (coreSlices[i] == piece)
            {
                coreSlices[i] = null;
            }
        }

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
        for (int i = primeterSlices.Length - 1; i >= 0; i--)
        {
            if (primeterSlices[i])
                tearPiece(primeterSlices[i].gameObject);
        }
        for (int i = coreSlices.Length - 1; i >= 0; i--)
        {
            if (coreSlices[i])
                tearPiece(coreSlices[i].gameObject);
        }
        Destroy(gameObject);
    }
}
