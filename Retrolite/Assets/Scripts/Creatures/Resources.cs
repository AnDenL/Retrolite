using System;

namespace CreatureAI
{
    [Serializable]
    public class Resource
    {
        public int count;

    }

    public enum ResourceType
    {
        Bits,
        Stamina,
        Energy,
        Health
    }
}