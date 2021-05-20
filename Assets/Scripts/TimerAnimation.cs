using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TimerAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Image _timerBackground;

    public void EnableTimer()
    {
        _timerText.gameObject.SetActive(true);
        _timerBackground.gameObject.SetActive(true);
    }

    public void DisableTimer()
    {
        _timerText.gameObject.SetActive(false);
        _timerBackground.gameObject.SetActive(false);
    }

    public void UpdateTimer(float time, float totalSeconds)
    {
        float leftTime = totalSeconds - time; 
        float fillPercentage = leftTime / totalSeconds;

        _timerBackground.fillAmount = fillPercentage;
        _timerText.text = Mathf.CeilToInt(leftTime).ToString();
    }
}
