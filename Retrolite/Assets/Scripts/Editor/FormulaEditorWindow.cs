#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using CalculatingSystem;
using System.Collections.Generic;

public class FormulaEditorWindow : EditorWindow
{
    private List<NodeView> nodes = new();
    private Vector2 scrollPos;

    [MenuItem("Tools/Formula Editor")]
    public static void OpenWindow()
    {
        GetWindow<FormulaEditorWindow>("Formula Editor");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Formula Node Editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Constant"))
            nodes.Add(new ConstantNodeView(new Vector2(50, 100)));

        if (GUILayout.Button("Add Variable"))
            nodes.Add(new VariableNodeView(new Vector2(50, 200)));

        if (GUILayout.Button("Add Expression"))
            nodes.Add(new ExpressionNodeView(new Vector2(50, 300)));

        GUILayout.Space(10);
        if (GUILayout.Button("Build Formula"))
            BuildFormula();

        foreach (var node in nodes)
            node.Draw();

        EditorGUILayout.EndScrollView();
    }

    private void BuildFormula()
    {
        // Пошук кореневого ExpressionNodeView
        foreach (var node in nodes)
        {
            if (node is ExpressionNodeView expr)
            {
                var left = new ConstantNode(5);
                var right = new VariableNode(StatVariable.PlayerHP);
                var result = new Expression(left, expr.operation, right);
                Debug.Log("Formula: " + result.ToReadableString());
                break;
            }
        }
    }

}
#endif

public abstract class NodeView
{
    public Vector2 Position;
    public Rect Rect;

    protected const float Width = 150f;
    protected const float Height = 60f;

    public NodeView(Vector2 position)
    {
        Position = position;
        Rect = new Rect(position.x, position.y, Width, Height);
    }

    public virtual void Draw()
    {
        GUI.Box(Rect, GetTitle());
    }

    protected abstract string GetTitle();
}

public class ConstantNodeView : NodeView
{
    public float Value = 1f;

    public ConstantNodeView(Vector2 pos) : base(pos) { }

    public override void Draw()
    {
        base.Draw();
        Rect inputRect = new Rect(Rect.x + 10, Rect.y + 25, Rect.width - 20, 20);
        Value = EditorGUI.FloatField(inputRect, Value);
    }

    protected override string GetTitle() => "Constant";
}

public class VariableNodeView : NodeView
{
    public StatVariable Value;

    public VariableNodeView(Vector2 pos) : base(pos) { }

    public override void Draw()
    {
        base.Draw();
        Rect inputRect = new Rect(Rect.x + 10, Rect.y + 25, Rect.width - 20, 20);
        Value = (StatVariable)EditorGUI.EnumPopup(inputRect, Value);
    }

    protected override string GetTitle() => "Variable";
}

public class ExpressionNodeView : NodeView
{
    public FormulaNode LeftNode;
    public FormulaNode RightNode;

    public Operator operation = Operator.Add;

    public ExpressionNodeView(Vector2 pos) : base(pos) { }

    public override void Draw()
    {
        base.Draw();

        float y = Rect.y + 25;
        float x = Rect.x + 10;

        Rect dropdownRect = new Rect(x, y, Rect.width - 20, 20);
        operation = (Operator)EditorGUI.EnumPopup(dropdownRect, operation);

        GUI.Label(new Rect(x, y + 22, Rect.width - 20, 20), $"Left: {(LeftNode == null ? "null" : LeftNode.ToReadableString())}");
        GUI.Label(new Rect(x, y + 42, Rect.width - 20, 20), $"Right: {(RightNode == null ? "null" : RightNode.ToReadableString())}");
    }

    protected override string GetTitle() => "Expression";
}
