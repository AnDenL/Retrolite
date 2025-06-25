using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveData CurrentSave;

    [SerializeField] private DataMode dataMode;

    private void Awake()
    {
        switch (dataMode)
        {
            case DataMode.Run:
                CurrentSave = new SaveData(100f, 100f, new GunData(2, 8, GunType.Pistol, BulletType.Bullet, new BulletData()), 0, 50, 0);
                break;
            case DataMode.Tutorial:
                CurrentSave = new SaveData(1, 100f, new GunData(), 0, 0, 0);
                break;
            case DataMode.LevelSelect:
                CurrentSave = new SaveData(100f, 100f, new GunData(), 0, 0, 0);
                break;
        }
    }
}

public class SaveData
{
    public float PlayerHealth;
    public float PlayerMaxHealth;
    public GunData PlayerWeapon;
    public int PlayerMoney;
    public int PlayerCode;
    public int PlayerLives;

    public SaveData(float health, float maxHealth, GunData weapon, int money, int code, int lives)
    {
        this.PlayerHealth = health;
        this.PlayerMaxHealth = maxHealth;
        this.PlayerWeapon = weapon;
        this.PlayerMoney = money;
        this.PlayerCode = code;
        this.PlayerLives = lives;
    }
}

enum DataMode
{
    Run,
    Tutorial,
    LevelSelect
}
