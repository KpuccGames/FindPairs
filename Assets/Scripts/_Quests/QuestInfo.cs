using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo
{
    public Dictionary<string, int> CardsRequired;

    public QuestInfo(Dictionary<string, int> requiredCards)
    {
        CardsRequired = requiredCards;
    }

    public int GetQuestBonus()
    {
        int bonus = 0;

        foreach (var cardsInfo in CardsRequired)
        {
            bonus += cardsInfo.Value;
        }

        return bonus;
    }

    public bool IsCompleted(Dictionary<string, int> collectedCards)
    {
        foreach (var cardRequiredPair in CardsRequired)
        {
            if (collectedCards.ContainsKey(cardRequiredPair.Key) == false)
            {
                return false;
            }

            int collectedCardsAmount = collectedCards[cardRequiredPair.Key];

            if (collectedCardsAmount < cardRequiredPair.Value)
            {
                return false;
            }
        }

        return true;
    }
}
