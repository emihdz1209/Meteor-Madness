using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timer = 0;

    public TMP_Text timerText;

    private void Start()
    {
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        timerText.text = "Time: " + timer.ToString("f1");
    }

    public float GetCurrentTime()
    {
        return timer;
    }
}
