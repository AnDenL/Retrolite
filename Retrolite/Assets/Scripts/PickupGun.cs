using UnityEngine;

public class PickupGun : MonoBehaviour
{
    public LayerMask weaponLayer;

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

            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, 0.01f, weaponLayer);
            
            if(colliders.Length != 0)
            {
                if (Vector2.Distance(transform.position, mousePos) <= 2)Weapons.PickUp(colliders[0], true);
            }
            else {
                colliders = Physics2D.OverlapCircleAll(transform.position, 1f, weaponLayer);
                if (colliders.Length != 0)Weapons.PickUp(colliders[0], true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, 0.01f, weaponLayer);
            
            if(colliders.Length != 0)
            {
                if (Vector2.Distance(transform.position, mousePos) <= 2)Weapons.PickUp(colliders[0], false);
            }
            else {
                colliders = Physics2D.OverlapCircleAll(transform.position, 1f, weaponLayer);
                if (colliders.Length != 0)Weapons.PickUp(colliders[0], false);
            }
        }
    }
}