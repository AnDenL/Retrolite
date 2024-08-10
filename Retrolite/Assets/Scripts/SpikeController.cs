using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    public Animator[] spike;
    public float delay;
    private int i = 0;
    private bool active = false;

    private void OnEnable()
    {
        
        StartCoroutine(Toggle());
    }

    private IEnumerator Toggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            spike[i].SetBool("Toggle", active);
            if (i < spike.Length - 1) i++;
            else 
            {
                active = !active;
                i = 0;
            }
        }
    }
}
