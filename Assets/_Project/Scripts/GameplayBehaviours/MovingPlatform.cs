using UnityEngine;
using DG.Tweening;
using MyBox;
using System.Collections;

public class MovingPlatform : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private RectTransform.Axis movementDirection;
    [SerializeField, Range(1f, 60f)] private float movementDuration = 20f; 
    [SerializeField, Range(1f, 60f)] private float intervalToGoBack = 2f;
    [SerializeField] private float positionOffset = 0.15f;
    [SerializeField] private bool activateRelatedObject = false;
    [ConditionalField(nameof(activateRelatedObject))][SerializeField] private GameObject relatedObject = null;
    [SerializeField] AudioSource audioSrc;

    private bool isMoving;
    private Vector3 initialPosition;
    
    private Sequence movingTween;
    private Tween returnTween;

    void Awake()
    { 
        isMoving = false;
        movingTween = null;
        relatedObject.SetActive(false);
        initialPosition = transform.position;  
    }

    public void SetPlatformMovement()
    {
        if (isMoving)
        {
            StopMovementAndReturnToOrigin();
        }
        else
        {
            GoToDestination();
        }
    }

    public void GoToDestination()
    {
        if (isMoving) return;
        movingTween?.Kill();
        
        movingTween = movementDirection == RectTransform.Axis.Vertical ? VerticalMovement() : HorizontalMovement();

        isMoving = true;
        relatedObject.SetActive(true);
        
        audioSrc.Play();
        movingTween.Play();
    }

    private Sequence VerticalMovement()
    {
        Sequence internalSequence = DOTween.Sequence();
        internalSequence.Append(transform.DOLocalMoveY(destinationPoint.position.y + positionOffset, movementDuration).SetEase(Ease.InSine));
        internalSequence.Append(transform.DOLocalMoveY(destinationPoint.position.y, movementDuration * 0.025f).SetEase(Ease.OutQuint));
        internalSequence.AppendCallback(StopAndFadeAudio);
        internalSequence.AppendInterval(intervalToGoBack);
        internalSequence.AppendCallback( () => audioSrc.Play());
        internalSequence.Append(transform.DOLocalMoveY(initialPosition.y, movementDuration * 0.5f).SetEase(Ease.OutQuart));
        internalSequence.OnComplete(EndAnimation);

        return internalSequence;
    }

    private Sequence HorizontalMovement()
    {
        Sequence internalSequence = DOTween.Sequence();
        internalSequence.Append(transform.DOLocalMoveX(destinationPoint.position.x + positionOffset, movementDuration).SetEase(Ease.InSine));
        internalSequence.Append(transform.DOLocalMoveX(destinationPoint.position.x, movementDuration * 0.025f).SetEase(Ease.OutQuint));
        internalSequence.AppendCallback(StopAndFadeAudio);
        internalSequence.AppendInterval(intervalToGoBack);
        internalSequence.AppendCallback( () => audioSrc.Play());
        internalSequence.Append(transform.DOLocalMoveX(initialPosition.x, movementDuration * 0.5f).SetEase(Ease.OutQuart));
        internalSequence.OnComplete(EndAnimation);

        return internalSequence;
    }

    private void StopMovementAndReturnToOrigin()
    {
        if (!isMoving) return;
        
        movingTween?.Kill();
        movingTween = null;
        isMoving = false;        
        PlayReturningTween(movementDirection);
    }

    private void EndAnimation()
    {
        isMoving = false;
        relatedObject.SetActive(false);
        StopAndFadeAudio();
    }

    private void OnDestroy()
    {
        movingTween?.Kill();
        returnTween?.Kill();
        movingTween = null;
        returnTween = null;
        isMoving = false;
    }

    public void Interact()
    {
        SetPlatformMovement();
    }

    private void PlayReturningTween(RectTransform.Axis direction)
    {
        returnTween?.Kill();
        returnTween = direction == RectTransform.Axis.Vertical ?
        transform.DOLocalMoveY(initialPosition.y, movementDuration * 0.5f).SetEase(Ease.OutQuart).OnComplete(EndAnimation) :
        transform.DOLocalMoveX(initialPosition.x, movementDuration * 0.5f).SetEase(Ease.OutQuart).OnComplete(EndAnimation) ;
    }

    private void StopAndFadeAudio() => StartCoroutine(audioSrc.FadeOutSound(0.55f));

}
