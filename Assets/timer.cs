using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    private float elapsedTime = 0f;
    private bool isGameOver = false;

    public Text timerText; // Připojte Textový UI element
    public Text winScreenText; // Text na výherní obrazovce

    void Update()
    {
        if (!isGameOver)
        {
            elapsedTime += Time.deltaTime;
            DisplayTime(timerText, elapsedTime);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        winScreenText.text = "Dokončeno za: " + FormatTime(elapsedTime);
    }

    private void DisplayTime(Text textElement, float time)
    {
        textElement.text = "Čas: " + FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

