using UnityEngine;
using System;
using CalculatingSystem;

[Serializable]
public class Rule
{
    public Condition[] conditions;
    public GameAction[] actions;

    public void Check(Context context)
    {
        foreach (Condition condition in conditions)
            if (condition.Evaluate(context))
            {
                ExecuteAll(context);
                break;
            }
    }

    public void ExecuteAll(Context context)
    {
        foreach (GameAction action in actions)
            action.Execute(context);
    }
}