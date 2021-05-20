using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTable : MonoBehaviour
{
    [SerializeField] private Card[] _cards;

    private List<CardData> _availableCards;

    private void Start()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            Card card = _cards[i];

            card.SetupCard(i);
        }
    }

    public void SetAvailableCards(List<CardData> availableCards)
    {
        _availableCards = availableCards;

        foreach (Card card in _cards)
        {
            InitCard(card);
        }
    }

    public void ResetCountedCards(int[] countedCards)
    {
        StartCoroutine(ReinitCards(countedCards));
    }

    private IEnumerator ReinitCards(int[] countedCards)
    {
        foreach (int cardIndex in countedCards)
        {
            _cards[cardIndex].CloseCard();
        }

        yield return new WaitForSecondsRealtime(Constants.CardAnimationDuration);

        foreach(int cardIndex in countedCards)
        {
            InitCard(_cards[cardIndex]);
        }
    }

    private void InitCard(Card card)
    {
        int availableCardDataIndex = Random.Range(0, _availableCards.Count);

        card.InitCard(_availableCards[availableCardDataIndex]);
    }
}
