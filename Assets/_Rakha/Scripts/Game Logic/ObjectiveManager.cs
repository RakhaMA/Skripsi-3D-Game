using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public bool PlayerInHospital;
    public bool PlayerInPark;
    public bool PlayerInBowlingAlley;

    public GameObject objectiveTextHospital;
    public GameObject objectiveTextPark;
    public GameObject objectiveTextBowlingAlley;
    public GameObject gameOverPanel;
    public TMP_Text statsText;

    public ProfilerController profilerController;

    private void Start()
    {
        Time.timeScale = 1;
        PlayerInHospital = false;
        PlayerInPark = false;
        PlayerInBowlingAlley = false;
    }

    private void Update()
    {
        if (PlayerInHospital && PlayerInPark && PlayerInBowlingAlley)
        {
            Debug.Log("All objectives completed!");
            GameOver();
        }
    }

    private void GameOver()
    {
        profilerController.isGameOver = true;
        gameOverPanel.SetActive(true);
        DisplayStats();
        Time.timeScale = 0;
    }

    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void DisplayStats()
    {
        if (profilerController != null)
        {
            statsText.text = profilerController.GetStatsSummary();
        }
    }
}
