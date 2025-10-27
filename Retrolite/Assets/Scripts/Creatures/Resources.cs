namespace Creatures
{
using System;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class Resource
    {
        public int Count { get; private set; }

        public event Action<int> OnChanged;

        public Resource(int initialCount)
        {
            Count = initialCount;
        }

        public void Add(int amount)
        {
            if (amount == 0) return;
            Count = Mathf.Max(Count + amount, 0);
            OnChanged?.Invoke(Count);
        }

        public bool TrySpend(int amount)
        {
            if (Count < amount) return false;
            Count -= amount;
            OnChanged?.Invoke(Count);
            return true;
        }

        public bool CanSpend(int amount) => Count >= amount;
    }

    [Serializable]
    public class ResourceContainer
    {
        private readonly Dictionary<ResourceType, Resource> resources = new();
        private readonly Creature owner;

        public IReadOnlyDictionary<ResourceType, Resource> Resources => resources;

        public event Action<ResourceType, int> OnAnyResourceChanged;

        public ResourceContainer(Creature owner)
        {
            this.owner = owner;
        }

        public Resource Get(ResourceType type)
        {
            if (!resources.TryGetValue(type, out var res))
            {
                res = new Resource(0);
                res.OnChanged += (value) => OnAnyResourceChanged?.Invoke(type, value);
                resources[type] = res;
            }
            return res;
        }

        public void Add(ResourceType type, int amount) => Get(type).Add(amount);
        public bool TrySpend(ResourceType type, int amount) => Get(type).TrySpend(amount);
        public bool CanSpend(ResourceType type, int amount) => Get(type).CanSpend(amount);
    }

    public enum ResourceType
    {
        Bits,
        Money,
        Stamina,
        Energy
    }
}