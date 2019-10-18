using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    // Параметры
    public GameObject bleedPrefab;
    public GameObject piesesPrefab;
    public RectTransform canvas;
    public Image playerHpBar;
    public Text playerHpText;
    public float slicesGravity = 10;
    public float slicesScatter = 20;
    public float slicesMaxRotation = 30f;
    public float slicesScale = 1;

    // Служеблые переменные
    static string m_currentSceneName;
    static string m_mainMenuSceneName;
    static GameSystem m_instance;
    Player player; // Скрипт игрока

    // Вызов до старта
    void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
        //DontDestroyOnLoad(gameObject);
        //if (canvas) DontDestroyOnLoad(canvas);
        player = Player.GetInstance();
        // Настройка систамы здоровья
        Health.Set(bleedPrefab, piesesPrefab, slicesGravity, slicesScatter, slicesMaxRotation, slicesScale, playerHpBar, playerHpText);
    }

    // Обращение к единственному объекту этого класса
    public static GameSystem GetInstance()
    {
        return m_instance;
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

    // Загрузка уровня
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    // Выход из игры
    public void ExitGame()
    {
        Application.Quit();
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
