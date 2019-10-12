using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Параметры
    [ColorUsageAttribute(true, true)] public Color juiceColor;
    public float bleedIntencity = 1;
    public GameObject bleedPrefab;
    public GameObject piesesPrefab;

    void showEffects(Vector3 point)
    {
        // Эффект сокотечения
        if (bleedPrefab)
        {
            GameObject curObj = Instantiate(bleedPrefab, point, Quaternion.identity);

            // Восстановление масштаба
            Vector3 scale = curObj.transform.localScale;
            curObj.transform.parent = transform;
            curObj.transform.localScale = scale;

            // Определение свойств частиц
            ParticleSystem ps = curObj.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = juiceColor;
            main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constantMin, main.startSize.constantMax * bleedIntencity);
        }

        // Эффект попадания
        if (piesesPrefab)
        {
            GameObject curObj = Instantiate(piesesPrefab, point, Quaternion.identity);

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
        if (damage > 0) showEffects(point);
    }

    // Запускается при старте
    void Start()
    {
        
    }
}
