#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using CalculatingSystem;

[CustomPropertyDrawer(typeof(FormulaNode), true)]
public class FormulaNodeDrawer : PropertyDrawer
{
    private const float ButtonWidth = 20f;
    private const float Spacing = 2f;
    private const float OperationWidth = 50f;
    private const float FixedLabelWidth = 120f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var node = property.managedReferenceValue;

        Rect buttonRect = new Rect(
            position.xMax - ButtonWidth,
            position.y,
            ButtonWidth,
            EditorGUIUtility.singleLineHeight
        );

        Rect mainRect = new Rect(
            position.x,
            position.y,
            position.width - ButtonWidth - Spacing,
            EditorGUIUtility.singleLineHeight
        );

        if (node == null)
        {
            if (GUI.Button(mainRect, "Set Formula"))
                ShowTypeMenu(property);

            EditorGUI.EndProperty();
            return;
        }

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = FixedLabelWidth;
        Rect contentRect = EditorGUI.PrefixLabel(mainRect, label);
        EditorGUIUtility.labelWidth = originalLabelWidth;

        if (node is ConstantNode)
        {
            var valueProp = property.FindPropertyRelative("Value");

            EditorGUI.PropertyField(contentRect, valueProp, GUIContent.none);
        }
        else if (node is SinNode || node is CosNode || node is AbsoluteNode)
        {
            var nodeProp = property.FindPropertyRelative("Node");
            EditorGUI.PropertyField(contentRect, nodeProp, GUIContent.none, true);
        }
        else if (node is VariableNode)
        {
            var varProp = property.FindPropertyRelative("Variable");
            EditorGUI.PropertyField(contentRect, varProp, GUIContent.none);
        }
        else if (node is Expression)
        {
            float nodeWidth = (contentRect.width - OperationWidth - Spacing * 2) / 2f;

            var leftProp = property.FindPropertyRelative("Left");
            var opProp = property.FindPropertyRelative("Operation");
            var rightProp = property.FindPropertyRelative("Right");

            Rect leftRect = new Rect(contentRect.x, contentRect.y, nodeWidth, contentRect.height);
            Rect opRect = new Rect(leftRect.xMax + Spacing, contentRect.y, OperationWidth, contentRect.height);
            Rect rightRect = new Rect(opRect.xMax + Spacing, contentRect.y, nodeWidth, contentRect.height);

            EditorGUI.PropertyField(leftRect, leftProp, GUIContent.none, true);
            EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
            EditorGUI.PropertyField(rightRect, rightProp, GUIContent.none, true);
        }
        else
        {
            EditorGUI.LabelField(contentRect, $"Unsupported node: {node.GetType().Name}");
        }

        if (GUI.Button(buttonRect, "☰"))
            ShowTypeMenu(property);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Constant"), false, () => SetNodeType(property, new ConstantNode(0)));
        menu.AddItem(new GUIContent("Absolute"), false, () => SetNodeType(property, new AbsoluteNode()));
        menu.AddItem(new GUIContent("Sin"), false, () => SetNodeType(property, new SinNode()));
        menu.AddItem(new GUIContent("Cos"), false, () => SetNodeType(property, new CosNode()));
        menu.AddItem(new GUIContent("Variable"), false, () => SetNodeType(property, new VariableNode()));
        menu.AddItem(new GUIContent("Expression"), false, () => SetNodeType(property, new Expression()));
        menu.ShowAsContext();
    }

    private void SetNodeType(SerializedProperty property, FormulaNode node)
    {
        property.managedReferenceValue = node;
        property.serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(ConditionNode), true)]
public class ConditionNodeDrawer : PropertyDrawer
{
    private const float ButtonWidth = 20f;
    private const float Spacing = 2f;
    private const float OperationWidth = 80f;
    private const float FixedLabelWidth = 120f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var node = property.managedReferenceValue;

        Rect buttonRect = new Rect(
            position.xMax - ButtonWidth,
            position.y,
            ButtonWidth,
            EditorGUIUtility.singleLineHeight
        );

        Rect mainRect = new Rect(
            position.x,
            position.y,
            position.width - ButtonWidth - Spacing,
            EditorGUIUtility.singleLineHeight
        );

        if (node == null)
        {
            if (GUI.Button(mainRect, "Set Condition"))
                ShowTypeMenu(property);

            EditorGUI.EndProperty();
            return;
        }

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = FixedLabelWidth;
        Rect contentRect = EditorGUI.PrefixLabel(mainRect, label);
        EditorGUIUtility.labelWidth = originalLabelWidth;

        // --- Specific node types ---
        if (node is ConditionVariableNode)
        {
            var varProp = property.FindPropertyRelative("Variable");
            EditorGUI.PropertyField(contentRect, varProp, GUIContent.none);
        }
        else if (node is ComparisonNode)
        {
            float nodeWidth = (contentRect.width - OperationWidth - Spacing * 2) / 2f;

            var leftProp = property.FindPropertyRelative("Left");
            var opProp = property.FindPropertyRelative("Operator");
            var rightProp = property.FindPropertyRelative("Right");

            Rect leftRect = new Rect(contentRect.x, contentRect.y, nodeWidth, contentRect.height);
            Rect opRect = new Rect(leftRect.xMax + Spacing, contentRect.y, OperationWidth, contentRect.height);
            Rect rightRect = new Rect(opRect.xMax + Spacing, contentRect.y, nodeWidth, contentRect.height);

            EditorGUI.PropertyField(leftRect, leftProp, GUIContent.none, true);
            EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
            EditorGUI.PropertyField(rightRect, rightProp, GUIContent.none, true);
        }
        else if (node is LogicNode)
        {
            float nodeWidth = (contentRect.width - OperationWidth - Spacing * 2) / 2f;

            var leftProp = property.FindPropertyRelative("Left");
            var opProp = property.FindPropertyRelative("Operator");
            var rightProp = property.FindPropertyRelative("Right");

            Rect leftRect = new Rect(contentRect.x, contentRect.y, nodeWidth, contentRect.height);
            Rect opRect = new Rect(leftRect.xMax + Spacing, contentRect.y, OperationWidth, contentRect.height);
            Rect rightRect = new Rect(opRect.xMax + Spacing, contentRect.y, nodeWidth, contentRect.height);

            EditorGUI.PropertyField(leftRect, leftProp, GUIContent.none, true);
            EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
            EditorGUI.PropertyField(rightRect, rightProp, GUIContent.none, true);
        }
        else
        {
            EditorGUI.LabelField(contentRect, $"Unsupported condition: {node.GetType().Name}");
        }

        if (GUI.Button(buttonRect, "☰"))
            ShowTypeMenu(property);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Variable Condition"), false, () => SetNodeType(property, new ConditionVariableNode()));
        menu.AddItem(new GUIContent("Comparison"), false, () => SetNodeType(property, new ComparisonNode(new ConstantNode(0), ComparisonOperator.Equal, new ConstantNode(0))));
        menu.AddItem(new GUIContent("Logic"), false, () => SetNodeType(property, new LogicNode()));
        menu.ShowAsContext();
    }

    private void SetNodeType(SerializedProperty property, ConditionNode node)
    {
        property.managedReferenceValue = node;
        property.serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(ActionNode), true)]
public class ActionNodeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
        {
            // Dropdown для створення нового ActionNode
            if (GUI.Button(position, "Select ActionNode Type"))
            {
                var menu = new GenericMenu();
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>().Where(t => !t.IsAbstract);

                foreach (var type in types)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }
        }
        else
        {
            // Показуємо всі серіалізовані поля цього ActionNode
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}

#endif