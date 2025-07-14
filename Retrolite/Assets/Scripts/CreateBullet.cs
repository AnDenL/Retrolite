using UnityEngine;

public class CreateBullet : MonoBehaviour
{
    [SerializeField] float shootCooldown = 3f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Vector2 direction;

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
