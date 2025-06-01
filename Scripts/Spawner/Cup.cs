using UnityEngine;

public class Cup : Food
{
    [SerializeField] private Transform _selfLid;

    public override ItemType ItemType => ItemType.Cup;

    protected override void Awake()
    {
        base.Awake();

        if (_selfLid == null)
        {
            Debug.Log("Transform lid not Assign", this);
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Lid lid))
        {
            _selfLid.gameObject.SetActive(true);

            lid.TryGetComponent(out Food food);
            {
                food.ReturnToPool();
            }
        }
    }

    protected override void ResetSpecificState()
    {
        _selfLid.gameObject.SetActive(false);
    }
}