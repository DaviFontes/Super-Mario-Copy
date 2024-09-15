using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public GameManager gameManager;
    public AudioManager audioManager;
    public Slider volumeSlider;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); 
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Back()
    {
        gameManager.GetComponent<GameManager>().MainMenu();
    }

    public void Options()
    {
        GameObject slider = GameObject.FindGameObjectWithTag("PauseMenu");
        slider.GetComponentInChildren<Slider>().value = audioManager.effectsVolume;
    }

    public void Continue()
    {
        gameManager.GetComponent<GameManager>().ContinueGame();
    }

    public void StartGame()
    {
        gameManager.GetComponent<GameManager>().NewGame();
    }

    public void Exit()
    {
        gameManager.GetComponent<GameManager>().ExitGame();
    }

    public void VolumeChange()
    {
       audioManager.GetComponent<AudioManager>().SetVolume(volumeSlider.value);
    }
}
