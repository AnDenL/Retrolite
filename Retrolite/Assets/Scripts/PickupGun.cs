using UnityEngine;

public class PickupGun : MonoBehaviour
{
    private WeaponsList Weapons;

    void Start()
    {
        Weapons = GetComponent<WeaponsList>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(transform.position, mousePos) <= 2){
                Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, 0.01f);
                
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Weapon"))
                    {
                        Weapons.PickUp(collider, true);
                        break;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(transform.position, mousePos) <= 2){
                Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, 0.01f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Weapon"))
                    {
                        Weapons.PickUp(collider, false);
                        break;
                    }
                }
            }
        }
    }
}