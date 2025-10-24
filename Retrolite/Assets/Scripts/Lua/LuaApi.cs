using System;
using System.Collections.Generic;
using CalculatingSystem;
using CreatureAI;
using MoonSharp.Interpreter;
using UnityEngine;

public static class LuaApi
{
    public static bool UseLua = true;
    private static Script _sharedLua;

    public static void Init()
    {
        _sharedLua = new Script();
        Register(_sharedLua);
    }

    public static void Register(Script lua)
    {
        UserData.RegisterAssembly();
        UserData.RegisterType<PlayerHealth>();
        UserData.RegisterType<Creature>();
        UserData.RegisterType<FormulaNode>();
        UserData.RegisterType<HealthBase>();
        UserData.RegisterType<Skill>();
        UserData.RegisterType<AIController>();
        UserData.RegisterType<Transform>();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Quaternion>();
        UserData.RegisterType<GameObject>();

        lua.Globals["FindCreatureByName"] = (Func<string, Creature>)FindCreatureByName;
        lua.Globals["FindObject"] = (Func<string, GameObject>)GameObject.Find;
        lua.Globals["LoadObject"] = (Func<string, UnityEngine.Object>)(path => Resources.Load(path));
        lua.Globals["Instantiate"] = (Func<GameObject, Vector3, Quaternion, GameObject>)UnityEngine.Object.Instantiate;
        lua.Globals["Instantiate"] = (Func<GameObject, Transform, GameObject>)UnityEngine.Object.Instantiate;
        lua.Globals["Destroy"] = (Action<UnityEngine.Object>)UnityEngine.Object.Destroy;

        lua.Globals["GetTime"] = (Func<float>)(() => Time.time);
        lua.Globals["WaitForSeconds"] = (Func<float, DynValue>)((seconds) =>
        {
            return DynValue.Nil;
        });

        lua.Globals["RandomRange"] = (Func<float, float, float>)UnityEngine.Random.Range;
        lua.Globals["CreateEmpty"] = (Func<string, GameObject>)((name) => new GameObject(name));

        lua.Globals["Player"] = PlayerController.Player;
    }

    public static Creature FindCreatureByName(string name)
    {
        var creatureObj = GameObject.Find(name);
        if (creatureObj == null) return null;
        return creatureObj.GetComponent<Creature>();
    }

    public static void ExecuteFile(string path)
    {
        try
        {
            if (_sharedLua == null)
                Init();

            _sharedLua.DoFile(path);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Lua] Error executing file {path}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void ExecuteString(string code)
    {
        try
        {
            if (_sharedLua == null)
                Init();

            _sharedLua.DoString(code);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Lua] Error executing string: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
