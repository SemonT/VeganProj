using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    // Служеблые переменные
    Player playerScript; // Скрипт игрока
    public Camera mainCamera;

    // Вызов при старте
    void Start()
    {
        playerScript = Player.GetInstance();
    }

    // Вызов каждый кадр
    void Update()
    {
        // Преследование камеры
        mainCamera.transform.position = new Vector3(playerScript.gameObject.transform.position.x, playerScript.gameObject.transform.position.y, -10);
        // Передача параметров ввода игровому персонажу
        playerScript.input(Input.GetAxis("Horizontal"), Input.GetKeyDown(KeyCode.Space));
    }
}
