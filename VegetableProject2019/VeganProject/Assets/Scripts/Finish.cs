using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    // Параметры
    public string nextSceneName = "MainMenu";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // При попадании игрока
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player.GetInstance().gameObject)
        {
            LoadNextLevel();
        }
    }
}
