using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public CardData CardData { get; private set; }
    public int CardIndex { get; private set; }
    public bool IsOpened { get; private set; }

    [SerializeField] private Image _cardBack;
    [SerializeField] private Image _cardIcon;

    private Vector3 _rotationTarget = new Vector3(0, 90);
    private Coroutine _animationCoroutine;
    private string _animationId;

    public static event Action<Card> OnCardClicked;

    public void SetupCard(int index)
    {
        CardIndex = index;
        _cardIcon.gameObject.SetActive(false);
        _animationId = $"cardAnimation{CardIndex}";
    }

    public void InitCard(CardData cardData)
    {
        CardData = cardData;
        _cardIcon.sprite = CardData.GetIcon();

        _cardBack.enabled = true;
    }

    public void OnClickCard()
    {
        OnCardClicked?.Invoke(this);
    }

    public void OpenCard()
    {
        if (IsOpened)
        {
            return;
        }
        
        IsOpened = true;

        FinishAnimation();

        _animationCoroutine = StartCoroutine(AnimateOpening());
    }

    public void CloseCard()
    {
        IsOpened = false;

        FinishAnimation();

        _animationCoroutine = StartCoroutine(AnimateClosing());
    }

    private IEnumerator AnimateOpening()
    {
        Sequence openingSequence = DOTween.Sequence().SetId(_animationId);

        openingSequence.Append(transform.DORotate(_rotationTarget, Constants.CardAnimationDuration));

        yield return new WaitUntil(openingSequence.IsComplete);

        openingSequence.Append(transform.DORotate(Vector3.zero, Constants.CardAnimationDuration));

        _cardBack.enabled = false;
        _cardIcon.gameObject.SetActive(true);
    }

    private IEnumerator AnimateClosing()
    {
        Sequence openingSequence = DOTween.Sequence().SetId(_animationId);

        openingSequence.Append(transform.DORotate(_rotationTarget, Constants.CardAnimationDuration));

        yield return new WaitUntil(openingSequence.IsComplete);

        openingSequence.Append(transform.DORotate(Vector3.zero, Constants.CardAnimationDuration));

        _cardBack.enabled = true;
        _cardIcon.gameObject.SetActive(false);
    }

    private void FinishAnimation()
    {
        if (_animationCoroutine != null)
        {
            if (DOTween.IsTweening(_animationId))
            {
                DOTween.Complete(_animationId);
            }

            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }
}
