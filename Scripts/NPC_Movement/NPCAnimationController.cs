using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimationController : MonoBehaviour
{
    private const string IsWalking = "IsWalking";

    [SerializeField] private Animator _animator;

    private void Awake()
    {
        if (_animator == null)
        {
            Debug.Log("Animator not assign", this);
            enabled = false;
            return;
        }
    }

    public void OnWaypointReached()
    {
    }

    public void OnPathCompleted()
    {
        _animator.SetBool(IsWalking, false);
    }
}