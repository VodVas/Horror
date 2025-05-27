using UnityEngine;

public class NPCTarget : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private GameObject moneyEffect;
    [SerializeField] private int rewardAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<IItem>();
        if (item?.State == ItemState.Thrown)
        {
            OnItemHit(item);
            Destroy(other.gameObject);
        }
    }

    private void OnItemHit(IItem item)
    {
        Debug.Log($"Hit with {item.Type}! Received {rewardAmount} coins!");

        if (hitSound)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

        if (moneyEffect)
        {
            var effect = Instantiate(moneyEffect, transform.position, transform.rotation);
            Destroy(effect, 2f);
        }
    }
}
