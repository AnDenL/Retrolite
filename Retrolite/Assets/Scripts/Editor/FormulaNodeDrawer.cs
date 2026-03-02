#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CalculatingSystem;

[CustomPropertyDrawer(typeof(ActionNode), true)]
[CustomPropertyDrawer(typeof(FormulaNode), true)]
[CustomPropertyDrawer(typeof(ConditionNode), true)]
public class UniversalNodeDrawer : PropertyDrawer
{
    private const float TypeButtonWidth = 90f;
    private const float Spacing = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null) 
            return EditorGUIUtility.singleLineHeight;

        var children = GetChildren(property);
        
        if (children.Count <= 1) 
            return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            foreach (var child in children)
            {
                height += EditorGUI.GetPropertyHeight(child, true) + Spacing;
            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect headerRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect buttonRect = new(headerRect.xMax - TypeButtonWidth, headerRect.y, TypeButtonWidth, headerRect.height);
        
        string typeName = property.managedReferenceValue != null 
            ? property.managedReferenceValue.GetType().Name.Replace("Node", "") 
            : "None";
            
        if (GUI.Button(buttonRect, typeName, EditorStyles.miniButton))
        {
            ShowTypeMenu(property);
        }

        if (property.managedReferenceValue == null)
        {
            Rect labelRect = headerRect;
            labelRect.width -= TypeButtonWidth + Spacing;
            EditorGUI.PrefixLabel(labelRect, label);
            EditorGUI.EndProperty();
            return;
        }

        var children = GetChildren(property);
        Rect contentRect = headerRect;
        contentRect.width -= TypeButtonWidth + Spacing;

        if (children.Count == 0)
        {
            EditorGUI.LabelField(contentRect, label);
        }
        else if (children.Count == 1)
        {
            var child = children[0];
            Rect prefixRect = EditorGUI.PrefixLabel(contentRect, label);
            EditorGUI.PropertyField(prefixRect, child, GUIContent.none, true);
        }
        else
        {
            property.isExpanded = EditorGUI.Foldout(contentRect, property.isExpanded, label, true);
            
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float y = position.y + EditorGUIUtility.singleLineHeight + Spacing;
                foreach (var child in children)
                {
                    float h = EditorGUI.GetPropertyHeight(child, true);
                    Rect childRect = new Rect(position.x, y, position.width, h);
                    EditorGUI.PropertyField(childRect, child, true);
                    y += h + Spacing;
                }
                EditorGUI.indentLevel--;
            }
        }

        EditorGUI.EndProperty();
    }

    private List<SerializedProperty> GetChildren(SerializedProperty property)
    {
        List<SerializedProperty> children = new List<SerializedProperty>();
        var iterator = property.Copy();
        var end = iterator.GetEndProperty();
        bool enterChildren = true;
        
        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
        {
            children.Add(iterator.Copy());
            enterChildren = false; 
        }
        return children;
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        Type baseType = GetTargetType();
        
        var types = TypeCache.GetTypesDerivedFrom(baseType).Where(t => !t.IsAbstract && !t.IsGenericType);

        menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
        {
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        });

        menu.AddSeparator("");

        foreach (var type in types.OrderBy(t => t.Name))
        {
            string menuPath = type.Name.Replace("Node", ""); 
            bool isCurrent = property.managedReferenceValue != null && property.managedReferenceValue.GetType() == type;

            menu.AddItem(new GUIContent(menuPath), isCurrent, () =>
            {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    private Type GetTargetType()
    {
        Type type = fieldInfo.FieldType;
        if (type.IsArray) return type.GetElementType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) return type.GetGenericArguments()[0];
        return type;
    }
}

[CustomPropertyDrawer(typeof(Formula))]
[CustomPropertyDrawer(typeof(Condition))]
[CustomPropertyDrawer(typeof(GameAction))]
public class WrapperDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var rootProp = property.FindPropertyRelative("rootNode");
        return rootProp != null ? EditorGUI.GetPropertyHeight(rootProp, label, true) : EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rootProp = property.FindPropertyRelative("rootNode");
        if (rootProp != null)
        {
            EditorGUI.PropertyField(position, rootProp, label, true);
        }
    }
}
#endif