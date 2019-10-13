using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    // Параметры
    public Camera mainCamera;
    public GameObject bleedPrefab;
    public GameObject piesesPrefab;

    // Служеблые переменные
    Player player; // Скрипт игрока

    // Вызов при старте
    void Start()
    {
        player = Player.GetInstance();
        Health.SetPrefabs(bleedPrefab, piesesPrefab);
    }

    // Вызов каждый кадр
    void Update()
    {
        // Преследование камеры
        mainCamera.transform.position = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, -10);
        // Передача параметров ввода игровому персонажу
        player.input(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKeyDown(KeyCode.Space));
    }
}
