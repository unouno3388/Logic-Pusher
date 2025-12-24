using Cysharp.Threading.Tasks; // 支援你代碼中的 UniTask
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameAssetLoader
{
    // 加載普通資源 (SO, Prefab, etc.)
    public async UniTask<T> LoadAsset<T>(string address) where T : Object
    {
        var handle = Addressables.LoadAssetAsync<T>(address);
        return await handle.ToUniTask();
    }

    // 加載場景
    public async UniTask LoadScene(string address, UnityEngine.SceneManagement.LoadSceneMode mode, System.Action<bool> callback)
    {
        var handle = Addressables.LoadSceneAsync(address, mode, false); // 先不自動激活
        var instance = await handle.ToUniTask();

        // 將加載好的場景實例傳回 GameManager
        //GameManager.Instance.LoadedScene = instance;
        callback?.Invoke(true);
    }
}