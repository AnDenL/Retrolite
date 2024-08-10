using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public static WeaponsList List;
    public static Health PlayerHealth;
    public static GameObject Player;
    public static Money Money;
    public static SanitySystem Sanity;
    public static bool Paused;

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

        if(numbers.Count == 0) return 0;

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
                    if(bullet.Health == null) numbers.Add(0);
                    else numbers.Add(bullet.Health.HealthPercent());
                    break;
                case "P":
                    numbers.Add(PlayerHealth.HealthPercent());
                    break;
                case "E":
                    if(bullet == null) numbers.Add(0);
                    else numbers.Add(bullet.Echo);
                    break;
                case "N":
                    if(bullet == null) numbers.Add(0);
                    else numbers.Add(bullet.Number);
                    break;
                case "D":
                    if(bullet == null) numbers.Add(0);
                    else numbers.Add(Vector2.Distance(bullet.startPos, bullet.transform.position));
                    break;
                case "M":
                    numbers.Add(Convert.ToSingle(Money.money / 10));
                    break;
                case "S":
                    numbers.Add(Sanity.SanityPercent());
                    break;
                case "K":
                    if(bullet == null) numbers.Add(0);
                    else numbers.Add(bullet.Gun.kills / 10);
                    break;
            }
        }
    }

}
