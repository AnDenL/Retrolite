using UnityEngine;

public class Murshroom : MonoBehaviour
{
    public int min,max;
    public GameObject bulletPrefab;
    public AudioClip sound;
    private GameObject curentBullet;

    public void SporeExplosion(){
        for (int i = 0;i < Random.Range(min,max);i++){
            curentBullet = Instantiate(bulletPrefab, transform.parent);
            float r = Random.Range(0.7f,1.8f);
            curentBullet.transform.localScale = new Vector2(r,r);
            curentBullet.transform.position = new Vector2(transform.position.x,transform.position.y);
            Spore s = curentBullet.GetComponent<Spore>();
            s.speed = Random.Range(0.5f,1f);
            s.target = new Vector2(transform.position.x + (Random.Range(-5,5) * 100),transform.position.y + (Random.Range(-5,5) * 100));
            curentBullet.transform.parent = null;      
        }
    }
    public void SoundEffect(){GetComponent<AudioSource>().PlayOneShot(sound);}

    private void Destroy() => Destroy(gameObject);
}
