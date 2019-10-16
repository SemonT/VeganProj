using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    // Параметры
    public string nextSceneName = "MainMenu";


    // При попадании игрока
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player.GetInstance().gameObject)
        {
            GameSystem.GetInstance().LoadLevel(nextSceneName);
        }
    }
}
