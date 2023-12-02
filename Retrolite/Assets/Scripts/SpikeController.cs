using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    public Animator[] spike;
    public float delay;
    private int i = 0;

    private void OnEnable()
    {
        spike[0].SetTrigger("Toggle");
        StartCoroutine(Toggle());
    }

    private IEnumerator Toggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            spike[i].SetTrigger("Toggle");
            if (i < spike.Length - 1) i++;
            else i = 0;
        }
    }
}
