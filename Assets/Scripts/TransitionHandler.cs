using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionHandler : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject gameover;
    public GameObject LevelStart;
    public TextMeshProUGUI WorldStage;
    public TextMeshProUGUI Lifes;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager.transitionType == 1)
        {
            StartCoroutine(LevelStartScreen());
        }
        if (gameManager.transitionType == 2)
        {
            StartCoroutine(GameOverScreen());
        }
    }

    public IEnumerator GameOverScreen()
    {
        LevelStart.SetActive(false);
        yield return new WaitForSeconds(4f);
        gameManager.MainMenu();
    }

    public IEnumerator LevelStartScreen()
    {
        gameover.SetActive(false);
        WorldStage.text = $"World {gameManager.world}-{gameManager.stage}";
        Lifes.text = $"x {gameManager.lifes}";
        yield return new WaitForSeconds(2.5f);
        gameManager.LoadStage();

    }

}
