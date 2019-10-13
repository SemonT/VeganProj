using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    // Параметры
    public GameObject bleedPrefab;
    public GameObject piesesPrefab;
    public Image playerHpBar;
    public Text playerHpText;

    // Служеблые переменные
    Player player; // Скрипт игрока

    // Вызов до старта
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        player = Player.GetInstance();
        // Настройка систамы здоровья
        Health.Set(bleedPrefab, piesesPrefab, playerHpBar, playerHpText);
    }

    // Вызов каждый кадр
    void Update()
    {
        if (player)
        {
            // Передача параметров ввода игровому персонажу
            player.input(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKeyDown(KeyCode.Space));
        }
    }

    // Вызывается после физических изменений
    private void LateUpdate()
    {
        // Преследование камеры
        if (player)
        {
            MainCamera.Set(player.gameObject.transform);
        }
    }
}
