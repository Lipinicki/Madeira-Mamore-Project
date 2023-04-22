using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MovingPlatform : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private RectTransform.Axis movementDirection;
    [SerializeField, Range(0f, 60f)] private float movementDuration = 20f; 
    [SerializeField, Range(1f, 10f)] private float intervalToGoBack = 2f;
    [SerializeField] private float positionOffset = 0.15f;
    
    private bool isMoving = false;
    private Vector3 initialPosition;
    private Sequence movingTween = null;

    void Start()
    {
        initialPosition = transform.position;    
    }

    public void GoToDestination()
    {
        if (isMoving) return;
        Debug.Log("aaaaaaaaaaaaaaa");
        movingTween?.Kill();
    
        movingTween = (movementDirection == RectTransform.Axis.Horizontal) ?
        HorizontalMovement() : VerticalMovement();

        isMoving = true;
        movingTween.Play();
    }

    private Sequence VerticalMovement()
    {
        Sequence internalSequence = DOTween.Sequence();
        internalSequence.Append(transform.DOLocalMoveY(destinationPoint.position.y + positionOffset, movementDuration).SetEase(Ease.InSine));
        internalSequence.Append(transform.DOLocalMoveY(destinationPoint.position.y, movementDuration * 0.025f).SetEase(Ease.OutQuint));
        internalSequence.AppendInterval(intervalToGoBack);
        internalSequence.Append(transform.DOLocalMoveY(initialPosition.y, movementDuration * 0.5f).SetEase(Ease.OutQuart));
        internalSequence.OnComplete( () => isMoving = false);

        return internalSequence;
    }

    private Sequence HorizontalMovement()
    {
        Sequence internalSequence = DOTween.Sequence();
        internalSequence.Append(transform.DOLocalMoveX(destinationPoint.position.x + positionOffset, movementDuration).SetEase(Ease.InSine));
        internalSequence.Append(transform.DOLocalMoveX(destinationPoint.position.x, movementDuration * 0.025f).SetEase(Ease.OutQuint));
        internalSequence.AppendInterval(intervalToGoBack);
        internalSequence.Append(transform.DOLocalMoveX(initialPosition.x, movementDuration * 0.5f).SetEase(Ease.OutQuart));
        internalSequence.OnComplete( () => isMoving = false);

        return internalSequence;
    }

    private void OnDestroy()
    {
        movingTween?.Kill();
        movingTween = null;
        isMoving = false;
    }

    public void Interact()
    {
        GoToDestination();
    }
}
