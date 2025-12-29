using System.Collections;
using CalculatingSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Spores", menuName = "CreatureAI/Skills/Spores")]
    public class Spores : PassiveSkill
    {
        public float cooldown;
        public int minCount, maxCount;
        public GameObject prefab;
        public BulletData data;        

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
            pool = new BulletPool(prefab, clip, data, context);
            owner.OnCast += Spawn;
        }

        public void Spawn(IEnumerator enumerator)
        {
            if (Time.time < lastTime + cooldown) return;

            int r = Random.Range(minCount, maxCount);

            for (int i = 0; i < r; i++)
            {
                clip.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                pool.Get().Fire(Random.Range(0, 1));
            }
            lastTime = Time.time;
        }
    }
}