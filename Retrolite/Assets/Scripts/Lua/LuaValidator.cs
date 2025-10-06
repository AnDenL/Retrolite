using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

public static class LuaValidator
{
    public static bool Validate(string luaCode, out string error)
    {
        try
        {
            var script = new Script();
            script.Options.ScriptLoader = new FileSystemScriptLoader();
            script.LoadString(luaCode);
            error = null;
            return true;
        }
        catch (SyntaxErrorException ex)
        {
            error = $"Syntax error: {ex.DecoratedMessage}";
            return false;
        }
        catch (InterpreterException ex)
        {
            error = $"Interpreter error: {ex.DecoratedMessage}";
            return false;
        }
        catch (System.Exception ex)
        {
            error = $"General error: {ex.Message}";
            return false;
        }
    }
}
