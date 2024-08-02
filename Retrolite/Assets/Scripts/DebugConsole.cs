using TMPro;
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _consoleHistory;
    [SerializeField] private TextMeshProUGUI _tips;
    [SerializeField] private GameObject _field;
    [SerializeField] private Scroll _scroll;

    private static Dictionary<string, object> variables = new Dictionary<string, object>();
    private static List<Bind> binds = new List<Bind>();
    private static List<string> spawnObjects = new List<string>();
    private static TextMeshProUGUI _consoleHistoryStatic;
    private EventSystem _eventSystem;
    private TMP_InputField _fieldText;
    private static List<string> _history = new List<string>();
    private List<string> tips = new List<string>();
    private int currentCommand;
    private const string _errorColor = "#ff5454", _white = "#FFFFFF", _yellow = "#ffd754";

    private AudioSource _audio;
    private GameObject _player;

    private void Start()
    {
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        _audio = GameObject.Find("MusicController").GetComponent<AudioSource>();
        _consoleHistoryStatic = _consoleHistory;
        _fieldText = _field.GetComponent<TMP_InputField>();
        if(_history.Count == 0) _history.Insert(0,"");

        IEnumerable<string> prefabNames = Resources.LoadAll<GameObject>("").Select(prefab => prefab.name);
        foreach (string name in prefabNames)
        {
            spawnObjects.Add(name);
        }
        _player = GameObject.Find("Player");
    }

    private void Update()
    {
        EventSystem.current.SetSelectedGameObject(_field);
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentCommand++;
            currentCommand = Math.Clamp(currentCommand,0,_history.Count - 1);
            _fieldText.text = _history[currentCommand];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentCommand--;
            currentCommand = Math.Clamp(currentCommand,0,_history.Count - 1);
            _fieldText.text = _history[currentCommand];
        }

        if(Time.timeScale == 0) return;

        foreach (var bind in binds)
        {
            if (bind.KeyDown)
            {
                if (Input.GetKeyDown(bind.Key))
                {
                    Command(bind.Command);
                }
            }
            else 
            {
                if (Input.GetKey(bind.Key))
                {
                    Command(bind.Command);
                }
            }
        }
    }

    public void Tips(string command)
    {
        if(!String.IsNullOrEmpty(command) && command[0] == '/')
        {
            command = command.Remove(0,1);
            string action = GetCommand(ref command);
            action = action.ToLower();
            switch(action)
            {
                default:
                    _tips.text = "- Clear: - Restart: - LoadScene: - Time: - Spawn: - Destroy: - Bind: - Unbind: - Music: - Immortal: - Color: - Scale: - Roll: - Freeze: - Unfreeze: - Var: - Dvar: - Method: - RunInBackground:";
                    break;
                case "print": case "message": case "write": case "p":
                    _tips.text = "Enter any text";
                    break;
                case "clear": case "clean": case "c":
                    _tips.text = "clean the console?";
                    break;
                case "restart": case "reset":  case "r":
                    _tips.text = "restart current scene?";
                    break;
                case "loadscene":  case "scene": case "l":
                    _tips.text = "Load scene by number";
                    break;
                case "timescale":  case "time": case "t":
                    _tips.text = "Enter any number you want(float number must be with ,)";
                    break;
                case "immortal": case "i": 
                    _tips.text = "false - you can die \ntrue - you can't die";
                    break;
                case "spawn": case "s":
                    tips = new List<string>();
                    _tips.text = "Enter name of object and count(Optional)";

                    foreach (string s in spawnObjects)
                    {
                        if(s.StartsWith(command, StringComparison.InvariantCultureIgnoreCase)) _tips.text += " - " + s;
                    }
                    break;
                case "destroy": case "d":
                    tips = new List<string>();
                    _tips.text = "Enter name of object";
                    GameObject[] obj = UnityEngine.Object.FindObjectsOfType<GameObject>();

                    foreach (GameObject objName in obj)
                    {
                        tips.Add(objName.name);
                    }

                    foreach (string s in tips)
                    {
                        if(s.StartsWith(command)) _tips.text += " - " + s;
                    }

                    break;
                case "music": case "m":
                    _tips.text = "Type a path to music you want to play";
                    break;
                case "color": case "colour": case "cl":
                    _tips.text = "Type any color in HEX for example #ff5454 or just color like red or blue";
                    break;
                case "scale":
                    tips = new List<string>();
                    _tips.text = "Changes size of an object \n" +  "example: /scale Player 2 2";

                    GameObject[] objToScale = UnityEngine.Object.FindObjectsOfType<GameObject>();

                    foreach (GameObject objName in objToScale)
                    {
                        tips.Add(objName.name);
                    }

                    foreach (string s in tips)
                    {
                        if(s.StartsWith(command)) _tips.text += " - " + s;
                    }
                    break;
                case "roll":
                    _tips.text = "Type minimal and maximal number";
                    break;
                case "freeze": case "f":
                    _tips.text = "Freezes the object on your cursor";
                    break;
                case "unfreeze": case "uf":
                    _tips.text = "Unfreezes the object on your cursor";
                    break;
                case "bind": case "key":
                    _tips.text = "Activates command when you press a key \n" +
                        $"example: /{action} G /grab";
                    break;
                case "unbind":
                    _tips.text = "Deletes bind by key or command \n" +
                        "examples:\n/unbind key G \n" + 
                        "/unbind command /grab \n" +
                        "/unbind all\n";
                    switch (GetCommand(ref command))
                    {
                        case "key":
                            foreach(Bind bind in binds)
                            {
                                string strKey = bind.Key.ToString();
                                if(strKey.StartsWith(command)) _tips.text += " - " + strKey;
                            }
                            break;
                        case "command":
                            foreach(Bind bind in binds)
                            {
                                if(bind.Command.StartsWith(command)) _tips.text += " - " + bind.Command;
                            }
                            break;
                    }
                    break;
                case "deletevar": case "dvar":
                    _tips.text = "Deletes variable";

                    List<string> variablesNames = new List<string>();
                    foreach (var varName in variables)
                    {
                        variablesNames.Add(varName.Key);
                    }

                    foreach (string s in variablesNames)
                    {
                        if(s.StartsWith(command)) _tips.text += " - " + s;
                    }
                    break;
                case "var":
                    _tips.text = "Creates a variable with value\n" +
                        $"example: /var P obj Player\n" +
                        "Variable types: str(text), int, float, img, bool, obj\n" +
                        "Also you can change variable value with = and +";
                    break;
                case "method": 
                    _tips.text = "Invokes a method from some component on the object\n" +
                        "Example: /method string Weapon0 Gun SetDamage 999\n" +
                        "/method (variable or void) (object) (component) (method name) (parameter)\n";
                    GetCommand(ref command);

                    if(String.IsNullOrEmpty(command)) break;

                    tips = new List<string>();
                    GameObject[] gobj = UnityEngine.Object.FindObjectsOfType<GameObject>();

                    foreach (GameObject objName in gobj)
                    {
                        tips.Add(objName.name);
                    }

                    foreach (string s in tips)
                    {
                        if(s.StartsWith(command)) _tips.text += " - " + s;
                    }
                    break;
                case "runinbackground":
                    _tips.text = "True or false";
                    break;
            }
        }
        else _tips.text = "Every command starts with /";
    }

    public void Command(string command)
    {
        if(!String.IsNullOrEmpty(command) && command[0] == '/')
        {
            command = command.Remove(0,1);
            string action = GetCommand(ref command);
            action = action.ToLower();
            
            switch(action)
            {
                default:
                    WriteLine("Idk what "+ action +" means lol");
                    break;
                case "deletevar": case "dvar":
                    string variableToDelete = GetCommand(ref command);
                    if(variableToDelete[0] != '$') variableToDelete = "$" + variableToDelete;
                    if (variables.ContainsKey(variableToDelete))
                    {
                        variables.Remove(variableToDelete);
                        WriteLine("Variable " + variableToDelete + " deleted");
                    }
                    else
                    {
                        WriteLine("Variable " + variableToDelete + " not found", _errorColor);
                    }
                    break;
                case "var":
                    string title = GetCommand(ref command);
                    if (!variables.ContainsKey("$" + title) || command[0] == '=' || command[0] == '+')
                    {
                        string type = GetCommand(ref command);
                        switch(type)
                        {   
                            case "obj":
                                GameObject objec = null;
                                if(command == "mouse") objec = GetMouseObject();
                                else objec = GameObject.Find(command);

                                if(objec != null)
                                {
                                    variables.Add("$" + title, objec);
                                    WriteLine("Variable " + title + " created with value: " + command);
                                }
                                else  WriteLine("Object "+ command +" not found", _errorColor);
                                break;
                            case "int":
                                int i = 0;
                                if(int.TryParse(command, out i)) variables.Add("$" + title, i);
                                WriteLine("Variable " + title + " created with value: " + command);
                                break;
                            case "img":
                                StartCoroutine(LoadImage(command, title));
                                break;
                            case "float":
                                float f = 0;
                                if(float.TryParse(command, out f)) variables.Add("$" + title, f);
                                WriteLine("Variable " + title + " created with value: " + command);
                                break;
                            case "=":
                                Type variableType1 = variables["$" + title].GetType();
                                if(variableType1 == typeof(int))
                                {
                                    int integerrrrrrrrrrrrrrrrrr = 0;
                                    if(int.TryParse(command, out integerrrrrrrrrrrrrrrrrr))
                                    {
                                        variables["$" + title] = integerrrrrrrrrrrrrrrrrr;
                                        WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                    }
                                    else 
                                        WriteLine($"Parse error, {command} can`t be parsed to integer number", _errorColor);
                                }
                                else if(variableType1 == typeof(float))
                                {
                                    float floaett = 0;
                                    if(float.TryParse(command, out floaett))
                                    {
                                        variables["$" + title] = floaett;
                                        WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                    }
                                    else 
                                        WriteLine($"Parse error, {command} can`t be parsed to float number", _errorColor);
                                }
                                else if(variableType1 == typeof(string))
                                {
                                    variables["$" + title] = command;
                                    WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                }
                                else if(variableType1 == typeof(bool))
                                {
                                    bool booleann = false;
                                    if(bool.TryParse(command, out booleann))
                                    {
                                        variables["$" + title] = booleann;
                                        WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                    }
                                    else 
                                        WriteLine($"Parse error, {command} can`t be parsed to bool", _errorColor);
                                }
                                else if(variableType1 == typeof(GameObject))
                                {
                                    GameObject objectr = null;
                                    if(command == "mouse") objectr = GetMouseObject();
                                    else objectr = GameObject.Find(command);

                                    if(objectr != null)
                                    {
                                        variables["$" + title] = objectr;
                                        WriteLine("Variable " + title + " created with value: " + command);
                                    }
                                    else  WriteLine("Object "+ command +" not found", _errorColor);
                                    }
                                    else 
                                    {
                                        WriteLine($"'=' does not work with this type of variable", _errorColor);
                                    }
                                break;
                            case "bool":
                                bool b = false;
                                if(bool.TryParse(command, out b)) variables.Add("$" + title, b);
                                WriteLine("Variable " + title + " created with value: " + command);
                                break;
                            case "+":
                                Type variableType = variables["$" + title].GetType();
                                if(variableType == typeof(int))
                                {
                                    int integerrrrrrrrrrrrrrrrrrr = 0;
                                    if(int.TryParse(command, out integerrrrrrrrrrrrrrrrrrr))
                                    {
                                        variables["$" + title] = (int)variables["$" + title] + integerrrrrrrrrrrrrrrrrrr;
                                        WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                    }
                                    else 
                                        WriteLine($"Parse error, {command} can`t be parsed to integer number", _errorColor);
                                }
                                else if(variableType == typeof(float))
                                {
                                    float floaet = 0;
                                    if(float.TryParse(command, out floaet))
                                    {
                                        variables["$" + title] = (float)variables["$" + title] + floaet;
                                        WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                    }
                                    else 
                                        WriteLine($"Parse error, {command} can`t be parsed to float number", _errorColor);
                                }
                                else if(variableType == typeof(string))
                                {
                                    variables["$" + title] = variables["$" + title] + command;
                                    WriteLine("Value of variable " + title + " changed to " + variables["$" + title]);
                                }
                                else 
                                {
                                    WriteLine($"Can't add this type of variable", _errorColor);
                                }
                                break;
                            case "text": case "str":
                                variables.Add("$" + title,command);
                                WriteLine("Variable " + title + " created with value: " + type + command);
                                break;
                            default:
                                variables.Add("$" + title,type + command);
                                WriteLine("Variable " + title + " created with value: " + type + command);
                                break;
                        }        
                    }
                    else
                    {
                        WriteLine("Variable " + title + " already exists", _errorColor);
                    }
                    break;
                case "print": case "message": case "write": case "p":
                    if(command[0] == '$') WriteLine(variables[command].ToString());
                    else WriteLine(command);
                    break;
                case "clear": case "clean": case "c":
                    _consoleHistory.text = null;
                    break;
                case "restart": case "reset": case "r":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    Time.timeScale = MainManu._timeScale;
                    break;
                case "loadscene":  case "scene": case "l":
                    int sceneNumber = 0;
                    if(command[0] == '$') 
                    {
                        sceneNumber = (int)variables[command];
                        sceneNumber = Mathf.Clamp(sceneNumber, 0, 5);
                        SceneManager.LoadScene(sceneNumber);
                    }
                    else if(int.TryParse(command, out sceneNumber))
                    {
                        sceneNumber = Mathf.Clamp(sceneNumber, 0, 5);
                        SceneManager.LoadScene(sceneNumber);
                    }
                    else 
                        WriteLine($"Parse error, {command} can`t be parsed to integer number", _errorColor);
                    break;
                case "timescale":  case "time": case "t":
                    float timeScale = 1;
                    if(command[0] == '$') 
                    {
                        timeScale = (float)variables[command];
                        timeScale = Mathf.Clamp(timeScale, 0, 20); 
                        MainManu._timeScale = timeScale;
                        Time.timeScale = timeScale;
                    }
                    else if(float.TryParse(command, out timeScale))
                    {
                        timeScale = Mathf.Clamp(timeScale, 0, 20); 
                        MainManu._timeScale = timeScale;
                        WriteLine("Time scale changed to " + timeScale);
                    }
                    else 
                        WriteLine($"Parse error, {command} can`t be parsed to float number", _errorColor);
                    break;
                case "immortal": case "i": 
                    bool flag;
                    if(command[0] == '$') 
                    {
                        flag = (bool)variables[command];
                        _player.GetComponent<Health>().isInvincible = flag;
                        WriteLine("Immortality " + flag);
                    }
                    else if (Boolean.TryParse(GetCommand(ref command), out flag))
                    {
                        _player.GetComponent<Health>().isInvincible = flag;
                        WriteLine("Immortality " + flag);
                    }
                    else 
                        WriteLine($"Parse error, {command} can`t be parsed to boolean", _errorColor);
                    break;
                case "spawn": case "s":
                    string name = GetCommand(ref command);
                    int repeats = 1;
                    GameObject prefab = null;
                    if(name[0] == '$') 
                        prefab = variables[name] as GameObject;
                    else 
                        prefab = Resources.Load<GameObject>(name);
                    if (prefab != null)
                    {
                        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        Instantiate(prefab, cursorPosition, Quaternion.identity);   
                        WriteLine("Object " + name + " created");
                        if(int.TryParse(command, out repeats))
                        {
                            for(int i = 1; i < repeats; i++)
                            {
                                GameObject spawnedObject = Instantiate(prefab, cursorPosition, Quaternion.identity);   
                                GUIUtility.systemCopyBuffer = spawnedObject.name;
                                WriteLine("Object " + name + " created");   
                            }
                        }
                    }
                    else WriteLine("Prefab "+ name +" not found", _errorColor); 
                    break;
                case "destroy": case "d":
                    GameObject obj = null;
                    if(command[0] == '$') 
                        obj = variables[command] as GameObject;
                    else 
                        if(command != "mouse") obj = GameObject.Find(command);
                        else obj = GetMouseObject();
                    if(obj != null)
                    {
                        Destroy(obj);
                        WriteLine(command +" destroyed");
                    }
                    else WriteLine("Object "+ command +" not found", _errorColor);
                    break;
                case "music": case "m":
                    string path;
                    
                    if(command[0] == '$') 
                        path = variables[command].ToString();
                    else 
                        path = command;

                    WriteLine(path);

                    if (!string.IsNullOrEmpty(path))
                    {
                        StartCoroutine(LoadAudio(path));
                    }
                    break;
                case "color": case "colour": case "cl":
                    _consoleHistoryStatic.text += "</color>";
                    if(command[0] == '#') _consoleHistoryStatic.text += $"<color={command}>";
                    else _consoleHistoryStatic.text += $"<color=\"{command}\">";
                    break;
                case "scale":
                    string tName = GetCommand(ref command);
                    Transform objtransform = null;

                    if(tName != "mouse") objtransform = GameObject.Find(tName).transform;
                    else objtransform = GetMouseObject().transform;

                    if(objtransform != null)
                    {
                        float x = 0;
                        float y = 1;
                        string strX = GetCommand(ref command);
                        string strY = GetCommand(ref command);
                        if(!float.TryParse(strX, out x))
                        {
                            WriteLine($"Parse error, {strX} can`t be parsed to float number");
                            break;
                        }
                        if(!float.TryParse(strY, out y))
                        {
                            WriteLine($"Parse error, {strY} can`t be parsed to float number");
                            break;
                        }

                        objtransform.localScale = new Vector2(x, y);
                    }
                    else 
                        WriteLine("Object "+ command +" not found", _errorColor);
                    break;
                case "roll":
                    float min = 0;
                    float max = 100;
                    string m = GetCommand(ref command);
                    if(!float.TryParse(m, out min))
                    {
                        WriteLine($"Parse error, {m} can`t be parsed to float number");
                        break;
                    }
                    if(float.TryParse(command, out max))
                    {
                        WriteLine($"Min:{min}\nMax:{max}\nResult:{UnityEngine.Random.Range(min, max)}");
                    }
                    else
                    {
                        WriteLine($"Parse error, {command} can`t be parsed to float number");
                        break;
                    }
                    break;
                case "freeze": case "f":
                    GameObject fobj = GetMouseObject();

                    if(fobj != null)
                    {
                        Rigidbody2D rb = fobj.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        }
                        else
                            WriteLine("Object not found", _errorColor);
                    }
                    break;
                case "unfreeze": case "uf":
                    GameObject ufobj = GetMouseObject();

                    if (ufobj != null)
                    {
                        Rigidbody2D rb = ufobj.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                        }
                        else
                            WriteLine("Object has no physics", _errorColor);
                    }
                    else
                        WriteLine("Object not found", _errorColor);
                    break;
                case "sprite":
                    SpriteRenderer sprite = null;
                    GameObject gobj;
                    string objN = GetCommand(ref command);

                    if(objN != "mouse")
                        gobj = GameObject.Find(objN);
                    else 
                        gobj = GetMouseObject();

                    if(gobj != null)
                    {
                        sprite = gobj.GetComponent<SpriteRenderer>();
                        if(sprite != null)
                        {
                            if(command[0] == '$') 
                                sprite.sprite = variables[command] as Sprite;
                            else
                                WriteLine("You did not specify a new sprite", _errorColor);
                        }
                        else WriteLine("The object is without a sprite", _errorColor);
                    }
                    else 
                        WriteLine("Object not found", _errorColor);
                    break;
                case "bind": case "key":
                    KeyCode keydown;
                    string stringKeyDown = GetCommand(ref command);
                    bool onKeyDown = true;

                    if(Boolean.TryParse(stringKeyDown, out onKeyDown))
                    {
                        stringKeyDown = GetCommand(ref command);
                    }
                    if (System.Enum.TryParse(stringKeyDown, true, out keydown))
                    {
                        binds.Add(new Bind(keydown, command, onKeyDown));
                        WriteLine($"Binding added: {stringKeyDown} -> {command}");
                    }
                    else WriteLine("Invalid KeyCode!", _errorColor);
                    break;
                case "unbind":
                    string typeIdentification = GetCommand(ref command);
                    string bindIdentification = GetCommand(ref command);
                    List<Bind> bindsToRemove = new List<Bind>();
                    switch(typeIdentification)
                    {
                        case "key":
                            foreach(Bind bind in binds)
                            {
                                if(bind.Key.ToString() == bindIdentification) 
                                {
                                    bindsToRemove.Add(bind);
                                    WriteLine($"Unbinded {bind.Key.ToString()} {bind.Command}");
                                }
                            }
                            break;
                        case "command":
                            foreach(Bind bind in binds)
                            {
                                if(bind.Command == bindIdentification) 
                                {
                                    bindsToRemove.Add(bind);
                                    WriteLine($"Unbinded {bind.Key.ToString()} {bind.Command}");
                                }
                            }
                            break;
                        case "all":
                            binds = new List<Bind>();
                            WriteLine("All binds removed");
                            break;
                    }
                    binds.Except(bindsToRemove);
                    break;
                case "method": 
                    string variable = GetCommand(ref command);

                    GameObject gameobj = null;
                    string objName = GetCommand(ref command);
                    Debug.Log(objName);

                    if(objName != "mouse")
                        gameobj = GameObject.Find(objName);
                    else 
                        gameobj = GetMouseObject();

                    if(gameobj == null) 
                    {
                        WriteLine("Object not found");
                        break;
                    }
                        
                    
                    var component = gameobj.GetComponent(GetCommand(ref command));

                    if(component == null)
                    {
                        WriteLine("Component not found");
                        break;
                    }
                    var methodInfo = component.GetType().GetMethod(GetCommand(ref command), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if(methodInfo != null)
                    {
                        switch (variable)
                        {
                            case "void":
                                methodInfo.Invoke(component, null);
                                break;
                            case "obj":
                                GameObject objec = null;
                                if(command == "mouse") objec = GetMouseObject();
                                else objec = GameObject.Find(command);

                                if(objec != null)
                                {
                                    methodInfo.Invoke(component, new object[] { objec });
                                }
                                break;
                            case "str": case "string":
                                methodInfo.Invoke(component, new object[] { command });
                                break;
                            case "int": case "integer":
                                int i = 0;
                                if(int.TryParse(command, out i))
                                    methodInfo.Invoke(component, new object[] { i });
                                else 
                                    WriteLine("Only int numbers are valid");
                                break;
                            case "f": case "float":
                                float f = 0;
                                if(float.TryParse(command, out f))
                                    methodInfo.Invoke(component, new object[] { f });
                                else 
                                    WriteLine("Only numbers are valid");
                                break;
                        }
                    
                    } 
                    else WriteLine("Method not found");
                    break;
                case "runinbackground":
                    bool runinbackground = false;
                    if(command[0] == '$') 
                    {
                        runinbackground = (bool)variables[command];
                        Application.runInBackground = runinbackground;
                        WriteLine("Immortality " + runinbackground);
                    }
                    else if (Boolean.TryParse(GetCommand(ref command), out runinbackground))
                    {
                        Application.runInBackground = runinbackground;
                        WriteLine("Immortality " + runinbackground);
                    }
                    else 
                        WriteLine($"Parse error, {command} can`t be parsed to boolean", _errorColor);
                    break;
            }
        }
        else WriteLine(command);
        currentCommand = 0;
        _history.Insert(1,_fieldText.text);
        _fieldText.text = null;
        int lines = _consoleHistoryStatic.textInfo.lineCount;
        if(lines > 16) _scroll.modify = (lines - 15) / 26f;
    }
    IEnumerator LoadAudio(string path)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            WriteLine(www.error, _errorColor);
        }
        else
        {
            DownloadHandlerAudioClip audioHandler = (DownloadHandlerAudioClip)www.downloadHandler;
            AudioClip audioClip = audioHandler.audioClip;

            WriteLine("Music changed");

            _audio.Stop();

            _audio.clip = audioClip;
            _audio.Play();
        }

        www.Dispose();
    }
    IEnumerator LoadImage(string path, string name)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            WriteLine(www.error, _errorColor);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (texture != null)
            {
                WriteLine("Image loaded successfully");
                WriteLine("Texture dimensions: " + texture.width + "x" + texture.height);

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                variables.Add("$" + name, sprite);
            }
            else
            {
                WriteLine("Failed to load texture. Texture is null.", _errorColor);
            }
        }

        www.Dispose();
    }
    public static void WriteLine(string text, string color = null)
    {
        if(color != null) _consoleHistoryStatic.text += $"<color={color}>{text}</color>\n";   
        else _consoleHistoryStatic.text += text + "\n";   
    }

    private string GetCommand(ref string text)
    {
        string command = "";
        bool wordEnd = false;
        foreach(char letter in text)
        {
            if(letter != ' ') 
                command += letter;
            else 
            {
                wordEnd = true;
                break;
            }
        }
        text = text.Remove(0, command.Length + Convert.ToInt32(wordEnd));
        return command;
    }
    private GameObject GetMouseObject()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] hitEmenies = Physics2D.OverlapCircleAll(cursorPosition, 0.1f);
        if (hitEmenies != null && hitEmenies[0] != null) return hitEmenies[0].gameObject;
        else return null;
    }
}

public class Bind
{
    public KeyCode Key;
    public string Command;
    public bool KeyDown;

    public Bind(KeyCode key, string command, bool down)
    {
        Key = key;
        Command = command;
        KeyDown = down;
    }
}