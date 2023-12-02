using System;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    public int money, bonus;
    public Text text;
    public ParticleSystem particles;
    private void Start() {
        text.text = "Money: " + Convert.ToString(money);
    }
    public void AddMoney(int m)
    {
        money += m + bonus;
        var emission = particles.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0, m + bonus));
        particles.Play();
        if(money > 1000) money = 1000;
        text.text = "Money: " + Convert.ToString(money);
    }
    public bool Buy(int cost) 
    {
        if(cost > money) return false;
        else {
            money -= cost;
            text.text = "Money: " + Convert.ToString(money);
            return true;
        }
    }
}
