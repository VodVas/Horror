using UnityEngine;

public class BloodHeadTriggerActivator : MonoBehaviour
{
    [SerializeField] private GameObject _beatEffect;
    [SerializeField] private GameObject _particleBlood;
    [SerializeField] private GameObject _bloodPoint;
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
            _bloodPoint.SetActive(true);
        }
    }

    public void Spawn()
    {
        GameObject bloodInstance = Instantiate(_particleBlood, _position.position, _position.rotation);
        _particleBlood.gameObject.SetActive(true);

        Destroy(bloodInstance, 3f);
    }
}