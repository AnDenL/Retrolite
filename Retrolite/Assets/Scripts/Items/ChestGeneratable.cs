using System;
using Creatures;
using UnityEngine;

public class ChestGeneratable : MonoBehaviour, IGenerationStruct
{
    [SerializeField] private Chest chest;

    [Header("Resources")]
    public GenerationResource[] resources;
    public GameObject[] prefabs;

    public int count;

    public void Generate(GameRandom random)
    {
        var reward = Reward.Empty();

        for (int i = 0; i < count; i++)
        {
            int j = random.Range(0,5);

            switch (j)
            {
                case 0:
                case 1:
                    reward.Resources.Add(resources[random.Range(0,resources.Length)].Generate(random));
                    break;
                case 2:
                    var obj = prefabs[random.Range(0, prefabs.Length)];
                    obj = Instantiate(obj, chest.transform);

                    if (obj.TryGetComponent(out IGenerationStruct gen)) gen.Generate(random);
                    
                    reward.Items.Add(obj);
                    obj.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        chest.SetReward(reward);
    }

    public void Clear()
    {
        chest.SetReward(new Reward());
    }
}

[Serializable]
public struct GenerationResource
{
    public ResourceType type;
    public int min;
    public int max;

    public readonly ResourceReward Generate(GameRandom random)
        => new() {Type = type, Amount = random.Range(min, max)};
}