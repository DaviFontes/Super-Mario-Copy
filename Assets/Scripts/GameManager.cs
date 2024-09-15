using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private AudioManager audioManager;
    public GameObject statusUIPrefab;
    public GameObject floatingPoints;

    private GameObject statusUI;
    public int lifes;
    private int coins;
    private int score;
    private float timer;
    public int world;
    public int stage;

    public bool isPaused;
    private bool inHurry = false;
    private bool inGame = false;
    public int transitionType = 1;

    void Start()
    {        
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        audioManager = FindObjectOfType<AudioManager>();
        statusUI = Instantiate(statusUIPrefab, statusUIPrefab.transform.position, Quaternion.identity);
        DontDestroyOnLoad(statusUI);
    }

    void Update()
    {
        if (inGame)
        {
            TimerUpdate();

            if (Input.GetButtonDown("Cancel"))
            {
                if (!isPaused)
                {
                    PauseGame();
                    isPaused = true;
                }
                else
                {
                    ContinueGame();
                    isPaused = false;
                }

            }
            UpdateUI();
        }
    }

    public void NewGame()
    {
        lifes = 3;
        score = 0;
        coins = 0;
        world = 1;
        stage = 1;
        LoadStage();
    }

    public void LoadStage()
    {
        SceneManager.LoadScene($"{world}-{stage}");
        audioManager.PlayMusic("Theme");
        inGame = true;
        isPaused = false;
        inHurry = false;
        timer = 500;
    }

    void RestartStage()
    {
        transitionType = 1;
        SceneManager.LoadScene("Transition");
    }

    public IEnumerator StageClear()
    {
        inGame = false;
        score += (int)Mathf.Round(timer * 100);
        timer = 0;
        UpdateUI();

        audioManager.PauseMusic();
        audioManager.Play("FlagPole");
        yield return new WaitForSeconds(2.5f);
        audioManager.PlayMusic("StageClear");
        yield return new WaitForSeconds(6f);


        if (stage==5)
        {
            world++;
            stage = 1;
        }
        else
        {
            stage++;
        }

        
        transitionType = 1;
        if (stage == 3)
        {
            transitionType = 2;
        }
        SceneManager.LoadScene("Transition");
    }

    void GameOver()
    {
        transitionType = 2;
        SceneManager.LoadScene("Transition");
        audioManager.PlayMusic("GameOver");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        audioManager.PauseMusic();
        audioManager.Play("Pause");
        GameObject pause = GameObject.FindGameObjectWithTag("PauseMenu");
        RectTransform[] objects = pause.GetComponentsInChildren<RectTransform>(true);

        foreach (RectTransform obj in objects)
        {
            if (obj.gameObject.name == "Menu")
            {
                obj.gameObject.SetActive(true);
            } else if (obj.gameObject.name == "Slider")
            {
                obj.GetComponentInChildren<Slider>().value = audioManager.effectsVolume;
            }
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        audioManager.ResumeMusic();
        GameObject pause = GameObject.FindGameObjectWithTag("PauseMenu");
        RectTransform[] objects = pause.GetComponentsInChildren<RectTransform>(true);

        foreach (RectTransform obj in objects)
        {
            if (obj.gameObject.name == "Menu")
            {
                obj.gameObject.SetActive(false);
            }
        }
        isPaused = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        inGame = false;
        isPaused = false;
        SceneManager.LoadScene("Title Screen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AddCoin()
    {
        AddScore(200);
        if(coins == 99)
        {
            coins = 0;
            AddLife();
        } 
        else
        {
            coins++;
        }
    }

    public void AddLife()
    {
        audioManager.Play("1Up");
        if (lifes < 99)
            lifes++;
    }

    public void AddLife(Vector3 position)
    {
        audioManager.Play("1Up");
        if(lifes < 99)
            lifes++;

        GameObject text = Instantiate(floatingPoints, position, Quaternion.identity);
        text.GetComponentInChildren<TextMeshPro>().text = "1UP";
        Destroy(text, 1f);
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void AddScore(int points, Vector3 position)
    {
        score += points;
        GameObject text = Instantiate(floatingPoints, position, Quaternion.identity);
        text.GetComponentInChildren<TextMeshPro>().text = $"{points}";
        Destroy(text, 1f);
    }

    void TimerUpdate()
    {
        if (!isPaused)
        {
            timer -= Time.deltaTime;
        }

        if (timer < 100 && !inHurry)
        {
            FindObjectOfType<AudioManager>().Play("TimeWarning");
            inHurry = true;
        }

        if (timer < 0)
        {
            timer = 0;
            FindObjectOfType<PlayerBehaviour>().Die();
        }
    }

    void UpdateUI()
    {
        TextMeshProUGUI[] texts = statusUI.GetComponentsInChildren<TextMeshProUGUI>();

        foreach(TextMeshProUGUI text in texts)
        {
            if (text.gameObject.name.Equals("Score"))
            {
                text.text = FormatNumber(score, 6);
            } 
            else if (text.gameObject.name.Equals("Coins")) 
            {
                text.text = "x" + FormatNumber(coins, 2);
            }
            else if (text.gameObject.name.Equals("Stage"))
            {
                text.text = $"{world}-{stage}";
            }
            else if (text.gameObject.name.Equals("TimeLeft"))
            {
                text.text = FormatNumber((int)Mathf.Floor(timer), 3);
            }
        }
    } 

    string FormatNumber(int n, int size)
    {
        if (n >= Mathf.Pow(10, size))
        {
            string s = "";
            for(int i=0; i<size; i++)
            {
                s += "9";
            }
            return s;
        }

        string text = $"{n}";
        for (int i = text.Length; i < size; i++)
        {
            text = "0" + text;
        }
        return text;
    }

    public IEnumerator PlayerDeath()
    {
        inGame = false;
        audioManager.PlayMusic("Die");
        lifes--;

        yield return new WaitForSeconds(3f);

        if (lifes == 0)
        {
            GameOver();
        } else
        {
            RestartStage();
        }
    }
}
