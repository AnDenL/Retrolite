using UnityEngine;
using System;
using CalculatingSystem;

[Serializable]
public class Rule
{
    [SerializeReference] public ConditionNode[] conditions;
    [SerializeReference] public ActionNode[] actions;

    public void Check(Context context)
    {
        foreach (ConditionNode condition in conditions)
            if (condition.Evaluate(context))
            {
                ExecuteAll(context);
                break;
            }
                
    }

    public void ExecuteAll(Context context)
    {
        foreach (ActionNode action in actions)
            action.Execute(context);
    }
}