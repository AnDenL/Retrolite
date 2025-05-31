using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeapon : MonoBehaviour
{
    [SerializeField] private WeaponItem _weaponItem;
    [SerializeField] private GameObject[] _bullets;
    private void Start()
    {
        _weaponItem = Instantiate(_weaponItem);
        _weaponItem.BulletPrefab = _bullets[Random.Range(0,_bullets.Length)];
        _weaponItem.ReloadTime = Random.Range(1.5f, 4);
        _weaponItem.Spread = Random.Range(0.0f, 25);
        _weaponItem.Range = RandomExpression(9, 18);
        _weaponItem.BulletSpeed = RandomExpression(0, 18);
        _weaponItem.FireSpeed = RandomExpression(0, 18);
        _weaponItem.Damage = RandomExpression(0, 20);
        _weaponItem.Magazine = Random.Range(1, 31);
        GetComponent<PickUpableItem>().Item = _weaponItem;
    }

    private string RandomExpression(int min, int max)
    {
        int length = Random.Range(1, 4);
        string expression = "";
        for (int i = 0; i < length; i++)
        {
            int r = Random.Range(min,max);

            switch(r)
            {
                case 0:
                    expression += "-5";
                    break;
                case 1:
                    expression += "-4";
                    break;
                case 2:
                    expression += "-3";
                    break;
                case 3:
                    expression += "-2";
                    break;
                case 4:
                    expression += "-1";
                    break;
                case 5:
                    expression += "0";
                    break;
                case 6:
                    expression += "0,5";
                    break;
                case 7:
                    expression += "1";
                    break;
                case 8:
                    expression += "2";
                    break;
                case 9:
                    expression += "3";
                    break;
                case 10:
                    expression += "4";
                    break;
                case 11:
                    expression += "5";
                    break;
                case 12:
                    expression += "R";
                    break;
                case 13:
                    expression += "P";
                    break;
                case 14:
                    expression += "E";
                    break;
                case 15:
                    expression += "N";
                    break;
                case 16:
                    expression += "M";
                    break;
                case 17:
                    expression += "S";
                    break;
                case 18:
                    expression += "I";
                    break;
                case 19:
                    expression += "K";
                    break;
                case 20:
                    expression += "D";
                    break;
                case 21:
                    expression += "H";
                    break;
                
            }
            expression += " ";
            if(i < length - 1)
            {
                int o = Random.Range(0,4);

                switch(o)
                {
                    case 0:
                        expression += "+";
                        break;
                    case 1:
                        expression += "-";
                        break;
                    case 2:
                        expression += "*";
                        break;
                    case 3:
                        expression += "/";
                        break;
                }
                expression += " ";
            }
        }
        return expression;
    }
}
