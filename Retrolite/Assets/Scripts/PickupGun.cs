using UnityEngine;

public class PickupGun : MonoBehaviour
{
    public LayerMask weaponLayer;

    private WeaponsList Weapons;

    private void Start()
    {
        Weapons = GetComponent<WeaponsList>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PickUp(true);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp(false);
        }
    }

    private void PickUp(bool hand)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, 0.01f, weaponLayer);
        
        if(colliders.Length != 0)
        {
            if (Vector2.Distance(transform.position, mousePos) <= 2)Weapons.PickUp(colliders[0], false);
        }
        else 
        {
            colliders = Physics2D.OverlapCircleAll(transform.position, 1f, weaponLayer);
            if (colliders.Length != 0)Weapons.PickUp(colliders[0], hand);
        }
    }
}