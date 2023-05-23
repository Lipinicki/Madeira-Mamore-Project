using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class InteractableDoorLockBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private bool useLever = false; // if true the door will open using a lever instead of a key
    [ConditionalField(nameof(useLever), inverse: true)][SerializeField] private Inventory playerInventory;
    [ConditionalField(nameof(useLever), inverse: true)][SerializeField] private string keyItemName;

    [Space(10), Header("Aniamtions")]
    [SerializeField] private Animator animator;
    [SerializeField] private string doorAnimationName;

    [Space(10)]
    [SerializeField] private UnityEvent OnDoorOpenned;

	void IInteractable.Interact()
	{
        if (playerInventory?.GetItem(keyItemName) == null && !useLever)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Item: " + keyItemName + "not found");
#endif
            return;
        }

        animator.Play(Animator.StringToHash(doorAnimationName));
        OnDoorOpenned?.Invoke();
        this.enabled = false;
	}
}
