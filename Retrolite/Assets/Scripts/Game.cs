using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class Game : MonoBehaviour
{
    public static Game instance;
    public static Player Player;
    public static WeaponBase Weapon;
    public static Inventory Inventory;
    public static Animator Fade; 
    public static bool Paused;

    public static float time;
    public static float kills;

    private void Awake()
    {
        instance = this;
        Fade = GameObject.Find("Fade").GetComponent<Animator>();
    }

    public static float Calculate(string expression, Bullet bullet)
    {
        List<float> numbers = new List<float>();
        List<char> operators = new List<char>();

        SplitByOperators(expression, bullet, out numbers, out operators); 

        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '*')
            {
                numbers[i] *= numbers[i + 1];
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--; 
            }
            else if (operators[i] == '/')
            {
                numbers[i] /= numbers[i + 1];
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--; 
            }
        }

        float result = numbers[0];

        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '+')
            {
                result += numbers[i + 1];
            }
            else if (operators[i] == '-')
            {
                result -= numbers[i + 1];
            }
        }

        return result;
    }

    private static void SplitByOperators(string text, Bullet bullet, out List<float> numbers, out List<char> operators)
    {
        numbers = new List<float>(); 
        operators = new List<char>(); 

        string[] splitByOperators = text.Split(" ");

        foreach (string part in splitByOperators)
        {
            switch(part)
            {
                default:
                    float n = 0;
                    if(float.TryParse(part, out n))
                        numbers.Add(n);
                    break;
                case "+":
                    operators.Add('+');
                    break;
                case "-":
                    operators.Add('-');
                    break;
                case "*":
                    operators.Add('*');
                    break;
                case "/":
                    operators.Add('/');
                    break;
                case "R":
                    numbers.Add(UnityEngine.Random.Range(-5f,5f));
                    break;
                case "H":
                    if(bullet.Health != null) numbers.Add(bullet.Health.HealthPercent());
                    else numbers.Add(0);
                    break;
                case "P":
                    numbers.Add(Player.HealthPercent());
                    break;
                case "E":
                    numbers.Add(Weapon.EchoDamage);
                    break;
                case "N":
                    numbers.Add(bullet.Number);
                    break;
                case "D":
                    numbers.Add(Vector2.Distance(bullet.StartPosition, bullet.transform.position));
                    break;
                case "M":
                    numbers.Add(Convert.ToSingle(Player.Money) / 10);
                    break;
                case "S":
                    numbers.Add(bullet.Speed);
                    break;
                case "I":
                    numbers.Add(Player.SanityPercent());
                    break;
                case "K":
                    numbers.Add(kills / 10);
                    break;
            }
        }
    }

    public void StartCharge(float f, ActiveItem item)
    {
        item.Charged = false;
        StartCoroutine(Timer(f,item));
    }
    private IEnumerator Timer(float time, ActiveItem item)
    {
        float f = 1;
        item.Fill.enabled = true;
        while (f > 0)
        {
            item.Fill.fillAmount = f;
            f -= Time.deltaTime / time;
            yield return null;
        }
        
        item.Fill.enabled = false;
        item.Charged = true;
    }
}