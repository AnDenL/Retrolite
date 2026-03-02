namespace CalculatingSystem
{
    using System;
    using System.Linq.Expressions;
    using UnityEngine;

    [Serializable]
    public struct Formula : ISerializationCallbackReceiver
    {
        [SerializeReference] private FormulaNode rootNode;
        private Func<Context, float> _cachedFunc;

        public Formula(FormulaNode node)
        {
            rootNode = node;
            _cachedFunc = null;
        }

        public void OnBeforeSerialize() { }
    
        public void OnAfterDeserialize()
        {
            // Як тільки ти щось міняєш в інспекторі, кеш скидається
            _cachedFunc = null; 
        }

        public void Compile()
        {
            if (rootNode == null)
            {
                _cachedFunc = context => 0f;
                return;
            }

            var contextParam = Expression.Parameter(typeof(Context), "context");
            
            var body = rootNode.BuildExpression(contextParam);
            
            var lambda = Expression.Lambda<Func<Context, float>>(body, contextParam);
            _cachedFunc = lambda.Compile();
        }

        public float Evaluate(Context context)
        {
            if (_cachedFunc == null) Compile();
            return _cachedFunc(context);
        }

        public readonly int GetTotalNodeCount() => rootNode != null ? rootNode.GetNodeCount() : 0;
        
        public readonly string ToReadableString() => rootNode != null ? rootNode.ToReadableString() : "none";
        public readonly bool IsConstant() => rootNode != null ? rootNode.IsConstant() : true;
    }
}