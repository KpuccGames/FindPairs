using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItem : MonoBehaviour
{
    [SerializeField] private QuestItemCardView[] _cardViews;

    public QuestInfo Quest { get; private set; }

    public static event Action<QuestInfo> OnClickItem;

    public void InitQuest(QuestInfo questInfo)
    {
        Quest = questInfo;

        int viewIndex = 0;

        foreach (var questsPart in Quest.CardsRequired)
        {
            CardData data = CardsDataStorage.Instance.GetById(questsPart.Key);

            _cardViews[viewIndex].gameObject.SetActive(true);
            _cardViews[viewIndex].Init(data, questsPart.Value);
            viewIndex++;
        }

        for (; viewIndex < _cardViews.Length; viewIndex++)
        {
            _cardViews[viewIndex].gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    public void TryCompleteQuest()
    {
        OnClickItem?.Invoke(Quest);
    }

    public void ResetItem()
    {
        Quest = null;
        gameObject.SetActive(false);
    }
}
