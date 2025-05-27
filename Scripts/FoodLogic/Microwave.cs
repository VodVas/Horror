using System.Collections;
using UnityEngine;

public class Microwave : ProcessingStation
{
    [SerializeField] private AudioClip microwaveSound;

    protected override IEnumerator ProcessingCoroutine()
    {
        if (microwaveSound)
            AudioSource.PlayClipAtPoint(microwaveSound, transform.position);

        yield return base.ProcessingCoroutine();
    }
}
