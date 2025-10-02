using System;
using System.Collections.Generic;
using CreatureAI;
using MoonSharp.Interpreter;
using UnityEngine;

public static class LuaApi
{
    public static bool UseLua = true;

    public static void Register(Script lua)
    {
        UserData.RegisterType<Creature>();
        UserData.RegisterType<HealthBase>();
        UserData.RegisterType<Skill>();
        UserData.RegisterType<PassiveSkill>();
        UserData.RegisterType<AIController>();
        UserData.RegisterType<Alignment>();
        UserData.RegisterType<PositionSkill>();
        UserData.RegisterType<TargetedSkill>();
        UserData.RegisterType<SelfSkill>();

        lua.Globals["FindCreatureByName"] = (Func<string, Creature>)FindCreatureByName;
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
            var lua = new Script();
            Register(lua);
            lua.DoFile(path);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error executing Lua file {path}: {ex.Message}");
        }
    }
}