using UnityEngine;

//Now useless
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public static SaveData CurrentSave;

    [SerializeField] DataMode dataMode;

    private void Awake()
    {
        instance = this;

        switch (dataMode)
        {
            case DataMode.Run:
                CurrentSave = new SaveData(100f, 100f, new GunData("Basic pistol", 2, 5, 8, 2, GunType.Pistol, BulletType.Bullet, new BulletData()), 0, 50, 0);
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
        PlayerHealth = health;
        PlayerMaxHealth = maxHealth;
        PlayerWeapon = weapon;
        PlayerMoney = money;
        PlayerCode = code;
        PlayerLives = lives;
    }
}

enum DataMode
{
    Run,
    Tutorial,
    LevelSelect
}
