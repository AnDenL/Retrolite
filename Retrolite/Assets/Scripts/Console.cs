using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Console : MonoBehaviour
{
    [SerializeField] private InputField _field;
    [SerializeField] private Text _console;
    [SerializeField] private Text _consoleHistory;
    [SerializeField] private Scroll _scroll;
    [SerializeField] private Volume _processing;
    private string _lastCommand;
    private int lines;
    private GameObject obj;
    private const int _maxScene = 5;
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            EnterCommand();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _field.text = _lastCommand;
    }

    private void EnterCommand(string text = null)
    {
        string com = null, arg = null;
        if(text == null) text = _console.text;

        foreach(char letter in text)
        {
            if(letter != ' ') 
                arg += letter;
            else if(com == null)
            {
                com = arg;
                arg = null;
            } 
            else arg += letter;
        }
        if(com == null) com = "print";
        
        DoCommand(com,arg);
    }

    private void DoCommand(string com, string arg = "Void")
    {
        com = com.ToLower();
        switch (com)
        {
            default:
                WriteLine("Unknown command: " + com);
                break;
            case "clear":
                lines = 0;
                _consoleHistory.text = null;
                break;
            case "help":
                WriteLine("- Clear: clean the console");
                WriteLine("- Print (text): type a text in console");
                WriteLine("- Restart: restarts current scene");
                WriteLine("- LoadScene (scene number): load scene");
                WriteLine("- Time (number): slows down or speeds up time");
                WriteLine("- Spawn (Help or Prefab name): spawns object");
                WriteLine("- Destroy (help or Object name): derstroy object");
                WriteLine("- Volume (help or profile name): changes effects");
                WriteLine("- Repeat (number): repeat last command");
                WriteLine("- Immortal/mortal: makes you immortal/mortal");
                WriteLine("- Hit (number): Damages all targets in mouse position");
                break;
            case "message": case "print":
                WriteLine(arg);
                break;
            case "restart":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case "loadscene":  case "scene":
                int sceneNumber = 0;
                if(int.TryParse(arg, out sceneNumber))
                {
                    sceneNumber = Mathf.Clamp(sceneNumber, 0, _maxScene);
                    SceneManager.LoadScene(sceneNumber);
                }
                else 
                    WriteLine("Only numbers are accepted as arguments to the LoadScene");  
                break;
            case "timescale":  case "time":
                float timeScale = 1;
                if(float.TryParse(arg, out timeScale))
                {
                    timeScale = Mathf.Clamp(timeScale, 0, 100); 
                    MainManu._timeScale = timeScale;
                    WriteLine("Time scale changed to " + timeScale);
                }
                else 
                    WriteLine("Only numbers are accepted as arguments to the TimeScale");  
                break;
            case "spawn":
                if(arg == "help")
                {
                    IEnumerable<string> prefabNames = Resources.LoadAll<GameObject>("").Select(prefab => prefab.name);
                    foreach (string name in prefabNames)
                    {
                        WriteLine("- "+name);
                    }
                }
                else 
                {
                    GameObject prefab = Resources.Load<GameObject>(arg);
                    if (prefab != null)
                    {
                        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Instantiate(prefab, cursorPosition, Quaternion.identity);   
                        WriteLine("Object " + arg + " created");
                    }
                    else
                        WriteLine("Prefab "+ arg +" not found");
                }
                break;
            case "destroy":
                if(arg == "help")
                {
                    GameObject[] obj = UnityEngine.Object.FindObjectsOfType<GameObject>();

                    foreach (GameObject objName in obj)
                    {
                        WriteLine("- "+objName.name);
                    }
                }
                else 
                {
                    obj = GameObject.Find(arg);
                    if(obj != null)
                    {
                        Destroy(obj);
                        WriteLine(arg +" destroyed");
                    }
                    else WriteLine("Object "+ arg +" not found");
                }
                break;
            case "postprocessing" : case "volume":
                if(arg == "help")
                    {
                        IEnumerable<string> profileNames = Resources.LoadAll<VolumeProfile>("").Select(prefab => prefab.name);
                        foreach (string name in profileNames)
                        {
                            WriteLine("- "+name);
                        }
                    }
                else
                {
                    VolumeProfile volume = Resources.Load<VolumeProfile>(arg);
                    if (volume != null)
                    {
                        _processing.profile = volume;
                        WriteLine("PostProcessing changed");
                    }
                    else
                    {
                        WriteLine("Profile "+ arg +" not found");
                    }
                } 
                break;
            case "repeat":
                _field.text = _lastCommand;
                int repeats = 0;
                string command = _lastCommand;

                if(int.TryParse(arg, out repeats)) 
                {
                    repeats = Mathf.Clamp(repeats, 0, 999);
                    for(int i = 0; i < repeats; i++)EnterCommand(command);
                }

                else  WriteLine("Only numbers are accepted as arguments to the Repeat");

                _field.text = command;
                break;
            case "immortal":
                GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().isInvincible = true;
                break;
            case "mortal":
                GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().isInvincible = false;
                break;
            case "hit":
                float damage;
                if (float.TryParse(arg, out damage))
                {
                    Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D[] hitEmenies = Physics2D.OverlapCircleAll(cursorPosition, 0.1f);
                    foreach (Collider2D enemy in hitEmenies) 
                    {
                        Health hp = enemy.GetComponent<Health>();
                        if(hp != null) hp.SetHealth(damage);
                    } 
                } 
                else WriteLine("Only numbers are accepted as arguments to the Hit");
                break;
        }
        _lastCommand = _console.text;
        _field.text = null;
    }
    private void WriteLine(string text)
    {
        lines++;
        _consoleHistory.text += text + "\n";
        if(lines > 20)_scroll._moddify = (lines - 19) / 32f;
    }
}
