using TMPro;
using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleOutput;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI autoComplition;
    [SerializeField] private TextMeshProUGUI warnings;
    [SerializeField] private Image warningImage;
    [SerializeField] private Sprite okIcon, warnIcon;

    private static List<string> commandHistory = new();
    private List<string> autoComplitions = new();
    private EventSystem eventSystem;
    private int historyIndex = 0;
    private int complitionSelected;
    private Script lua;
    
    private const string errorColor = "#f75640ff";
    private const string infoColor = "#dfc63cff";
    private const string successColor = "#24ff24ff";

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
            if (inputField.text.Length < 1)
            {
                historyIndex++;
                if (historyIndex >= commandHistory.Count)
                    historyIndex = commandHistory.Count - 1;
                inputField.text = commandHistory[historyIndex];
            }
            else
            {
                if (autoComplitions.Count == 0) return;

                complitionSelected++;
                if (complitionSelected >= autoComplitions.Count) complitionSelected = 0;

                autoComplition.text = inputField.text + autoComplitions[complitionSelected];
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (inputField.text.Length < 1)
            {
                historyIndex--;
                if (historyIndex < 0)
                    historyIndex = 0;

                inputField.text = commandHistory[historyIndex];
            }
            else
            {
                if (autoComplitions.Count == 0) return;

                complitionSelected--;
                if (complitionSelected < 0) complitionSelected = autoComplitions.Count - 1;

                autoComplition.text = inputField.text + autoComplitions[complitionSelected];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (autoComplitions.Count == 0) return;
            int Count = autoComplitions[complitionSelected].Length;
            inputField.text += autoComplitions[complitionSelected];
            inputField.caretPosition += Count;
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
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
        inputField.ActivateInputField();
    }

    public void OnTextChange(string text)
    {
        if (historyIndex == 0) commandHistory[0] = inputField.text;

        if (string.IsNullOrWhiteSpace(text))
        {
            warnings.text = "";
            autoComplition.text = "";
            return;
        }

        var word = text.Split(" ")[^1];

        var globalKeys = lua.Globals.Keys
            .Where(k => k.Type == DataType.String)
            .Select(k => k.String)
            .ToArray();

        autoComplitions = new();
        complitionSelected = 0;

        foreach (var key in globalKeys)
        {
            if (key.StartsWith(word, StringComparison.OrdinalIgnoreCase))
            {
                string completion = key.Substring(word.Length);
                autoComplitions.Add(completion);
            }
        }

        if (autoComplitions.Count != 0)
            autoComplition.text = inputField.text + autoComplitions[complitionSelected];

        if (!LuaValidator.Validate(text, out string error))
        {
            warnings.text = $"<color={infoColor}>{error}</color>\n";
            warningImage.sprite = warnIcon;
        }
        else
        {
            warnings.text = "";
            warningImage.sprite = okIcon;
        }
    }
}
