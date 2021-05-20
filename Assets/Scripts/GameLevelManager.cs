using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using SimpleJson;

public class GameLevelManager : MonoBehaviour
{
    [Header("Cards Logic")]
    [SerializeField] private CardTable _cardTable;
    [SerializeField] private TimerAnimation _timer;

    private List<Card> _chosenCards = new List<Card>();
    private bool _canOpenCards;
    private Coroutine _cardOpenWaitingCoroutine;

    [Space(10)]
    [Header("Card Collection Logic")]
    [SerializeField] private CardsCounter[] _cardsCounters;

    private Dictionary<string, int> _collectedCards = new Dictionary<string, int>();

    public static Action<string, int> OnCollectedAmountChanged;

    [Space(10)]
    [Header("Quests Logic")]
    [SerializeField] private QuestItem[] _questItems;
    private int _minimumCardsRequired;
    private int _maximumCardsRequired;
    private int _maximumCardTypes;
    private int _activeQuestsAmount;
    private int _questsCompleted;

    private List<CardData> _availableCards;
    private int _collectedBonuses;

    private const float NewQuestGenerationDelay = 10f;

    private void OnEnable()
    {
        Card.OnCardClicked += OnCardChosen;
        QuestItem.OnClickItem += CompleteQuest;
    }

    private void OnDisable()
    {
        Card.OnCardClicked -= OnCardChosen;
        QuestItem.OnClickItem -= CompleteQuest;
    }

    private void Start()
    {
        _canOpenCards = true;
        _timer.DisableTimer();

        // FIXME: парсить список карт на уровне из EnvironmentDataStorage
        _availableCards = CardsDataStorage.Instance.GetData();

        _cardTable.SetAvailableCards(_availableCards);

        for (int i = 0; i < _availableCards.Count; i++)
        {
            CardData availableCard = _availableCards[i];
            _collectedCards.Add(availableCard.Id, 0);
            _cardsCounters[i].InitCounter(availableCard);
        }

        //FIXME: уровень должен определяться по состоянию профиля
        int gameLevelNumber = 12;

        LevelDifficulty levelDifficulty = GameLevelsConfig.Instance.GetLevelDifficulty(gameLevelNumber);
        var questsConfig = QuestsConfig.Instance;

        _minimumCardsRequired = questsConfig.GetMinimumCardsRequiredForQuest(levelDifficulty);
        _maximumCardsRequired = questsConfig.GetMaximumCardsRequiredForQuest(levelDifficulty);
        _maximumCardTypes = questsConfig.GetMaximumCardTypesRequiredForQuest(levelDifficulty);

        foreach (var questItem in _questItems)
        {
            questItem.ResetItem();
        }

        StartCoroutine(QuestGenerationProcess());
    }

    #region CardLogic
    private void OnCardChosen(Card lastCard)
    {
        if (_canOpenCards == false || lastCard.IsOpened)
        {
            return;
        }

        lastCard.OpenCard();

        if (_cardOpenWaitingCoroutine != null)
        {
            StopCoroutine(_cardOpenWaitingCoroutine);
            _cardOpenWaitingCoroutine = null;
        }

        if (_chosenCards.Count == 0 || _chosenCards[0].CardData.Id.Equals(lastCard.CardData.Id))
        {
            _chosenCards.Add(lastCard);

            _cardOpenWaitingCoroutine = StartCoroutine(StartCardOpenTimer(lastCard));

            return;
        }

        StopOpenCards(lastCard);
    }

    private IEnumerator StartCardOpenTimer(Card lastCard)
    {
        _timer.EnableTimer();

        float waitTime = 0f;

        while (waitTime < Constants.CardChooseTime)
        {
            _timer.UpdateTimer(waitTime, Constants.CardChooseTime);

            yield return new WaitForEndOfFrame();

            waitTime += Time.deltaTime;
        }

        StopOpenCards(lastCard);
    }

    private void StopOpenCards(Card lastCard)
    {
        _timer.DisableTimer();

        _canOpenCards = false;

        if (_chosenCards.Count == 1)
        {
            StartCoroutine(FinishCardChosing(() =>
            {
                _chosenCards[0].CloseCard();

                if (lastCard.Equals(_chosenCards[0]) == false)
                {
                    lastCard.CloseCard();
                }

                _chosenCards.Clear();
            }));
        }
        else
        {
            StartCoroutine(FinishCardChosing(() =>
            {
                AddCollectedCards();

                int[] countedCardIndexes = new int[_chosenCards.Count];

                for (int i = 0; i < _chosenCards.Count; i++)
                {
                    Card chosenCard = _chosenCards[i];
                    countedCardIndexes[i] = chosenCard.CardIndex;
                }

                _cardTable.ResetCountedCards(countedCardIndexes);

                lastCard.CloseCard();
                _chosenCards.Clear();
            }));
        }
    }

    private IEnumerator FinishCardChosing(Action callback)
    {
        yield return new WaitForSecondsRealtime(Constants.CardAnimationDuration + 0.3f);

        callback?.Invoke();
        _canOpenCards = true;
    }
    #endregion

    #region CardCollectionLogic
    private void AddCollectedCards()
    {
        if (_chosenCards.Count <= 0)
        {
            return;
        }

        string currentBonusId = _chosenCards[0].CardData.Id;
        int currentBonusesAmount = _collectedCards[currentBonusId] + _chosenCards.Count;

        _collectedCards[currentBonusId] = currentBonusesAmount;

        OnCollectedAmountChanged?.Invoke(currentBonusId, currentBonusesAmount);
    }

    private void SpendCollectedCards(string cardId, int cardsAmount)
    {
        _collectedCards[cardId] -= cardsAmount;

        OnCollectedAmountChanged(cardId, _collectedCards[cardId]);
    }
    #endregion

    #region QuestsLogic
    private void AddQuest()
    {
        if (_activeQuestsAmount >= _questItems.Length)
        {
            return;
        }

        int cardTypesForQuest = Math.Min(UnityEngine.Random.Range(1, _maximumCardTypes + 1), _availableCards.Count);
        Dictionary<string, int> questInfo = new Dictionary<string, int>();

        for (int i = 0; i < cardTypesForQuest; i++)
        {
            int randomCard = UnityEngine.Random.Range(0, cardTypesForQuest);
            int amount = UnityEngine.Random.Range(_minimumCardsRequired, _maximumCardsRequired + 1);

            string chosenCardId = _availableCards[randomCard].Id;

            if (questInfo.ContainsKey(chosenCardId) == false)
            {
                questInfo.Add(chosenCardId, 0);
            }

            questInfo[chosenCardId] += amount;
        }

        QuestItem item = GetEmptyQuestItem();
        item.InitQuest(new QuestInfo(questInfo));

        _activeQuestsAmount++;
    }

    private void CompleteQuest(QuestInfo questInfo)
    {
        if (questInfo.IsCompleted(_collectedCards) == false)
        {
            return;
        }

        foreach (var cardItem in _questItems)
        {
            if (cardItem.Quest == questInfo)
            {
                foreach (var requiredCard in questInfo.CardsRequired)
                {
                    SpendCollectedCards(requiredCard.Key, requiredCard.Value);

                    _collectedBonuses += requiredCard.Value;
                }

                Debug.Log("Bonuses collected: " + _collectedBonuses);

                cardItem.ResetItem();
                break;
            }
        }

        _questsCompleted++;
        _activeQuestsAmount--;
    }

    private IEnumerator QuestGenerationProcess()
    {
        yield return new WaitForSeconds(2f);

        AddQuest();

        while (true)
        {
            yield return new WaitForSeconds(NewQuestGenerationDelay);

            if (_activeQuestsAmount < _questItems.Length)
            {
                AddQuest();
            }
        }
    }

    private QuestItem GetEmptyQuestItem()
    {
        foreach (var questItem in _questItems)
        {
            if (questItem.Quest == null)
            {
                return questItem;
            }
        }

        return null;
    }
    #endregion
}
