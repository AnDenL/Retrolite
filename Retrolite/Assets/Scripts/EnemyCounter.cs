using System.Collections;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    private Money Player;

    public Health[] enemy;
    public GameObject room, reward;
    public ParticleSystem[] particles;
    public MusicController music;
    public int StartMusic, endMusic;

    void Start()
    {
        if(StartMusic != 0) 
        music = GameObject.Find("MusicController").GetComponent<MusicController>();;
        Player = GameObject.Find("Player").GetComponent<Money>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player"){
            room.SetActive(true);
            StartCoroutine(EnemyCheck());
        }
    }
    IEnumerator EnemyCheck()
    {
        int a = 0;
        if (music != null) music.ChangeMusic(StartMusic);
        while(a < enemy.Length)
        {
            a = 0;
            for (int i = 0; i < enemy.Length; i++){
                if(enemy[i].isDead == true) a++;
            }
            yield return null;
        }
        for (int i = 0;i < particles.Length;i++)
        {
            var emission = particles[i].emission;
            emission.enabled = false;
        }
        if (music != null) music.ChangeMusic(endMusic);
        if (reward != null) reward.SetActive(true);
        Player.AddMoney(Random.Range(1, 6));
        Destroy(gameObject,2f);
    }
}
