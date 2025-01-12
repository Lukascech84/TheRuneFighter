using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [HideInInspector] public static float elapsedTime;

    public Text timerText; // Připojte Textový UI element

    private void Start()
    {
        elapsedTime += Time.time;
        DisplayTime(timerText, elapsedTime);
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

