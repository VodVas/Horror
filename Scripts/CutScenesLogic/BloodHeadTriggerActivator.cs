using UnityEngine;

public class BloodHeadTriggerActivator : MonoBehaviour
{
    [SerializeField] private GameObject _beatEffect;
    [SerializeField] private GameObject _particleBlood;
    [SerializeField] private Transform _position;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Wall _))
        {
            _beatEffect.SetActive(true);
            Spawn();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Wall _))
        {
            _beatEffect.SetActive(false);
        }
    }

    public void Spawn()
    {
        Instantiate(_particleBlood, _position.position, _position.rotation);
    }
}