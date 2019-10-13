using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Параметры
    [ColorUsageAttribute(true, true)] public Color juiceColor;
    public float bleedIntencity = 1;
    static GameObject m_bleedPrefab;
    static GameObject m_piesesPrefab;

    // Служебные переменные
    static Health m_instance;

    private void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
    }

    // Запускается при старте
    void Start()
    {

    }

    public static void SetPrefabs(GameObject bleedPrefab, GameObject piesesPrefab)
    {
        m_bleedPrefab = bleedPrefab;
        m_piesesPrefab = piesesPrefab;
    }

    public static Health GetInstance()
    {
        return m_instance;
    }

    void playDamageEffects(Vector3 point)
    {
        // Эффект сокотечения
        if (m_bleedPrefab)
        {
            GameObject curObj = Instantiate(m_bleedPrefab, point, Quaternion.identity);

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

        // Эффект попадания
        if (m_piesesPrefab)
        {
            GameObject curObj = Instantiate(m_piesesPrefab, point, Quaternion.identity);

            // Восстановление масштаба
            ParticleSystem ps = curObj.GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule shape = ps.shape;
            shape.texture = GetComponent<SpriteRenderer>().sprite.texture;

            // Определение свойств частиц
            Vector3 scale = curObj.transform.localScale;
            curObj.transform.parent = transform;
            curObj.transform.localScale = scale;
        }
    }

    // Нанесение урона / лечение
    public void addDamage(int damage, Vector3 point)
    {
        print(damage);
        if (damage > 0) playDamageEffects(point);
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
