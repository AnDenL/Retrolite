using UnityEngine;

public class CreateBullet : MonoBehaviour
{
    [SerializeField]
    private float shootCooldown = 3f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private Vector2 direction;

    private float shootTimer = 2f;

    private void Start()
    {
        shootTimer = Time.time + 1f;
    }

    private void Update()
    {
        if (shootTimer < Time.time)
        {
            shootTimer += shootCooldown;
            foreach (var spawnPoint in spawnPoints)
            {
                Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyBulletBase>().Shoot(direction);
            }
        }
    }
}
