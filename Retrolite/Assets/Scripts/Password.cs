using System;
using UnityEngine;
using UnityEngine.UI;
public class Password : MonoBehaviour
{
    public Health[] keys;
    public TextMesh text;
    public ParticleSystem particles;
    private float nextTime = 0;
    private int[] password = new int[4];
    void Start()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            password[i] = UnityEngine.Random.Range(1,10);
            text.text += Convert.ToString(password[i]);
        }
    }  
    
    void Update () 
    {
        if (Time.time >= nextTime) 
        {
            PassCheck();
            nextTime += 0.5f; 
        }
    }
    void PassCheck()
    {
        int a = 0;
        for (int i = 0; i < keys.Length; i++){
            if(keys[i].healthPoint == password[i]) a++;
        }
        if(a == keys.Length){
            var emission = particles.emission;
            emission.enabled = false;
            Destroy(gameObject);
        }
    }
}
