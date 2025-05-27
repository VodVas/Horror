public class Test
{
    //using UnityEngine;
    //using VodVas.InterfaceSerializer;

    //public class QueueFoodController : MonoBehaviour
    //{
    //    [Header("Cup Settings")]
    //    [SerializeField] private Transform _targetCup;
    //    [SerializeField] private GameObject _cupPrefab;
    //    [SerializeField] private Transform _cupHoldPosition;

    //    [Header("Throw Settings")]
    //    [SerializeField] private float _throwForce = 10f;

    //    [Header("Pickup Settings")]
    //    [SerializeField] private float _pickupRadius = 1.5f;
    //    [SerializeField] private float _pickupAngle = 45f;

    //    [SerializeField, InterfaceConstraint(typeof(IInputProvider))]
    //    private MonoBehaviour _inputProvider;

    //    private GameObject _cupInHand;
    //    private bool _hasCupInHand = false;
    //    private IInputProvider _input => _inputProvider as IInputProvider;

    //    private void Update()
    //    {
    //        if (_input.GetPushInput())
    //        {
    //            if (_hasCupInHand)
    //            {
    //                ThrowCup();
    //            }
    //            else
    //            {
    //                TryTakeCup();
    //            }
    //        }
    //    }

    //    private void TryTakeCup()
    //    {
    //        if (_hasCupInHand) return;

    //        // Check table cup
    //        if (_targetCup != null && _targetCup.gameObject.activeSelf)
    //        {
    //            Vector3 toTarget = _targetCup.position - transform.position;
    //            if (toTarget.magnitude <= _pickupRadius)
    //            {
    //                float angle = Vector3.Angle(transform.forward, toTarget.normalized);
    //                if (angle <= _pickupAngle)
    //                {
    //                    TakeNewCup();
    //                    return;
    //                }
    //            }
    //        }

    //        Collider[] cups = Physics.OverlapSphere(transform.position, _pickupRadius, LayerMask.GetMask("Default"));
    //        GameObject closestCup = null;
    //        float minDist = Mathf.Infinity;

    //        foreach (Collider col in cups)
    //        {
    //            if (!col.CompareTag("Cup")) continue;

    //            Rigidbody rb = col.GetComponent<Rigidbody>();
    //            if (rb == null || rb.isKinematic) continue;

    //            Vector3 dir = col.transform.position - transform.position;
    //            float angle = Vector3.Angle(transform.forward, dir.normalized);
    //            if (angle > _pickupAngle) continue;

    //            float dist = dir.sqrMagnitude;
    //            if (dist < minDist)
    //            {
    //                minDist = dist;
    //                closestCup = col.gameObject;
    //            }
    //        }

    //        if (closestCup != null)
    //        {
    //            _cupInHand = closestCup;
    //            PickUpThrownCup();
    //        }
    //    }

    //    private void TakeNewCup()
    //    {
    //        if (_cupPrefab == null || _cupHoldPosition == null) return;

    //        _cupInHand = Instantiate(_cupPrefab, _cupHoldPosition.position, _cupHoldPosition.rotation, _cupHoldPosition);
    //        _cupInHand.SetActive(true);
    //        ResetCupInHand();
    //        _hasCupInHand = true;
    //    }

    //    private void PickUpThrownCup()
    //    {
    //        if (_cupInHand == null) return;

    //        _cupInHand.SetActive(true);
    //        ResetCupInHand();
    //        _hasCupInHand = true;
    //    }

    //    private void ResetCupInHand()
    //    {
    //        _cupInHand.transform.SetParent(_cupHoldPosition);
    //        _cupInHand.transform.localPosition = Vector3.zero;
    //        _cupInHand.transform.localRotation = Quaternion.identity;

    //        if (_cupInHand.TryGetComponent(out Rigidbody rb))
    //        {
    //            rb.isKinematic = true;
    //            rb.velocity = Vector3.zero;
    //            rb.angularVelocity = Vector3.zero;
    //        }

    //        _cupInHand.layer = LayerMask.NameToLayer("Weapon");
    //    }

    //    private void ThrowCup()
    //    {
    //        if (!_hasCupInHand || _cupInHand == null) return;

    //        _cupInHand.layer = LayerMask.NameToLayer("Default");
    //        if (_cupInHand.TryGetComponent(out Rigidbody rb))
    //        {
    //            rb.isKinematic = false;
    //            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
    //        }

    //        _cupInHand.transform.SetParent(null);
    //        _hasCupInHand = false;
    //        _cupInHand = null;
    //    }
    //}

}