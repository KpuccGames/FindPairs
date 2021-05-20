using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class QuestItemCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cardsRequiredText;
    [SerializeField] private Image _cardIcon;

    public void Init(CardData card, int requiredAmount)
    {
        _cardIcon.sprite = card.GetIcon();
        _cardsRequiredText.text = requiredAmount.ToString();
    }
}
