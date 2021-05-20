using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardsCounter : MonoBehaviour
{
    [SerializeField] private Image _bonusIcon;
    [SerializeField] private TextMeshProUGUI _bonusCounter;

    private CardData _cardData;

    private void OnEnable()
    {
        GameLevelManager.OnCollectedAmountChanged += UpdateCounter;
    }

    private void OnDisable()
    {
        GameLevelManager.OnCollectedAmountChanged -= UpdateCounter;
    }

    public void InitCounter(CardData cardData)
    {
        _cardData = cardData;

        _bonusIcon.sprite = _cardData.GetIcon();
        _bonusCounter.text = "0";
    }

    private void UpdateCounter(string cardId, int bonusAmount)
    {
        if (_cardData.Id.Equals(cardId) == false)
        {
            return;
        }

        _bonusCounter.text = bonusAmount.ToString();
    }
}
