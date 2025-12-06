using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;

public static class AddressablesLoader
{
    public static async Task<T> Load<T>(string key) where T : Object
    {
        return await Addressables.LoadAssetAsync<T>(key).Task;
    }
}
