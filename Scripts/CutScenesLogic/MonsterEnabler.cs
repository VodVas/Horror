using UnityEngine;

public class MonsterEnabler : MonoBehaviour 
{
    [SerializeField] private GameObject _monster;
    [SerializeField] private CutsceneActivator _triggerCounter;

    //private void OnEnable()
    //{
    //    _triggerCounter.EndOf1Scene += SetActive;
    //}

    //private void OnDisable()
    //{
    //    _triggerCounter.EndOf1Scene -= SetActive;
    //}

    public void SetActive()
    {
        _monster.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            _monster.SetActive(false);
        }
    }
}