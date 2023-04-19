using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private RectTransform.Axis movementDirection;
    [SerializeField, Range(0f, 60f)] private float movementDuration = 20f; 
    [SerializeField, Range(1f, 10f)] private float intervalToGoBack = 2f;
    [SerializeField] private float positionOffset = 0.15f;
    
    private Vector3 initialPosition;
    private Sequence movingTween = null;

    void Start()
    {
        initialPosition = transform.position;    
    }

    public void GoToDestination()
    {
        movingTween?.Kill();
    
        movingTween = (movementDirection == RectTransform.Axis.Horizontal) ?
        HorizontalMovement() : VerticalMovement();

        movingTween.Play();
    }

    private Sequence VerticalMovement()
    {
        Sequence internalSequence = DOTween.Sequence();
        internalSequence.Append(transform.DOLocalMoveY(destinationPoint.position.y + positionOffset, movementDuration).SetEase(Ease.OutSine));
        internalSequence.Append(transform.DOLocalMoveY(destinationPoint.position.y, movementDuration * 0.1f).SetEase(Ease.OutQuart));
        internalSequence.AppendInterval(intervalToGoBack);
        internalSequence.Append(transform.DOLocalMoveY(initialPosition.y, movementDuration * 0.5f).SetEase(Ease.InOutExpo));

        return internalSequence;
    }

    private Sequence HorizontalMovement()
    {
        Sequence internalSequence = DOTween.Sequence();
        internalSequence.Append(transform.DOLocalMoveX(destinationPoint.position.x + positionOffset, movementDuration).SetEase(Ease.OutSine));
        internalSequence.Append(transform.DOLocalMoveX(destinationPoint.position.x, movementDuration * 0.1f).SetEase(Ease.OutQuart));
        internalSequence.AppendInterval(intervalToGoBack);
        internalSequence.Append(transform.DOLocalMoveX(initialPosition.x, movementDuration * 0.5f).SetEase(Ease.InOutExpo));

        return internalSequence;
    }

    private void OnDestroy()
    {
        movingTween?.Kill();
        movingTween = null;
    }
}
