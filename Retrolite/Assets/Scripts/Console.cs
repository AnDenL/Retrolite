using TMPro;
using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Console : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleOutput;
    [SerializeField] private TMP_InputField inputField;

    private static List<string> commandHistory = new List<string>();
    private EventSystem eventSystem;
    private int historyIndex = 0;
    private Script lua;

    private const string errorColor = "#f23c24";
    private const string infoColor = "#FFFF00";
    private const string successColor = "#00FF00";

    private void Start()
    {
        lua = new Script();

        if (commandHistory.Count == 0) commandHistory.Add("");

        eventSystem = EventSystem.current;
        lua.Options.DebugPrint = s => AppendOutput(s);

        LuaApi.Register(lua);
    }

    private void AppendOutput(string message)
    {
        consoleOutput.text += message + "\n";
    }

    private void AppendOutput(string message, string color)
    {
        consoleOutput.text += $"<color={color}>{message}</color>\n";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RunCommand();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            historyIndex++;
            if (historyIndex >= commandHistory.Count)
                historyIndex = commandHistory.Count - 1;
            inputField.text = commandHistory[historyIndex];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            historyIndex--;
            if (historyIndex < 0)
                historyIndex = 0;

            inputField.text = commandHistory[historyIndex];
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }

    public void RunCommand()
    {
        if (string.IsNullOrEmpty(inputField.text))
            return;
        string code = inputField.text;
        inputField.text = "";

        try
        {
            DynValue result = lua.DoString(code);

            if (result != null && result.Type != DataType.Void && result.Type != DataType.Nil)
            {
                AppendOutput(">" + result.ToString());
            }
        }
        catch (SyntaxErrorException ex)
        {
            AppendOutput("Syntax Error: " + ex.DecoratedMessage, errorColor);
        }
        catch (ScriptRuntimeException ex)
        {
            AppendOutput("Runtime Error: " + ex.DecoratedMessage, errorColor);
        }
        catch (Exception ex)
        {
            AppendOutput("C# Error: " + ex.Message, errorColor);
        }

        commandHistory.Insert(1, code);
        historyIndex = 0;
        eventSystem.SetSelectedGameObject(inputField.gameObject);
    }
    
    public bool CheckSyntax(string code, out string error)
    {
        error = null;
        try
        {
            lua.LoadString(code);
            return true;
        }
        catch (SyntaxErrorException ex)
        {
            error = "Syntax Error: " + ex.DecoratedMessage;
            return false;
        }
        catch (Exception ex)
        {
            error = "C# Error: " + ex.Message;
            return false;
        }
    }
}
