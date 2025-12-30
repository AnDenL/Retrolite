using System.Collections;
using CalculatingSystem;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Spores", menuName = "CreatureAI/Skills/Spores")]
    public class Spores : PassiveSkill
    {
        public float Cooldown;
        public int MinCount, MaxCount;
        public GameObject Prefab;
        public BulletData Data;
        public Sprite[] Sprites; 
        public GameObject MushroomPrefab;

        private BulletPool pool;
        private Transform clip;
        private float lastTime;

        public override SkillType Type => SkillType.Attack;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            Context context = new()
            {
                Owner = owner,
            };

            clip = owner.transform.Find("clip");
            if (!clip)
            {
                var obj = new GameObject("clip");
                obj.transform.parent = owner.transform;
                clip = obj.transform;
                clip.transform.position = owner.transform.position;
            }

            pool = new BulletPool(Prefab, clip, Data, context);

            var sr = owner.transform.Find("Sprite").GetComponent<SpriteRenderer>();

            if (sr)
            {
                for (int i = 0; i < 4; i++)
                {
                    var obj = Instantiate(MushroomPrefab, owner.transform.Find("Sprite"));
                    obj.GetComponent<SpriteRenderer>().sprite = Sprites[Random.Range(0,Sprites.Length-1)];
                    Vector2 size = sr.sprite.bounds.size;
                    obj.transform.position = owner.transform.position + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 3, size.y / 3) + 0.1f);
                }
            }
            
            owner.OnCast += Spawn;
        }

        public void Spawn(IEnumerator enumerator)
        {
            if (Time.time < lastTime + Cooldown) return;

            int r = Random.Range(MinCount, MaxCount);

            for (int i = 0; i < r; i++)
            {
                clip.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                pool.Get().Fire(Random.Range(0, 1));
            }
            lastTime = Time.time;
        }
    }
}