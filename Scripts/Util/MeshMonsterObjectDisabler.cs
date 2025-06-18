using UnityEngine;

public class MeshMonsterObjectDisabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Monster monster))
        {
            monster.gameObject.SetActive(false);
        }
    }
}