using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class LoadAB : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // LoadFromFileExample();

        //StartCoroutine(LoadFromMemoryAsync());

        // StartCoroutine(InstantiateObject());

        StartCoroutine(LoadManifest());
    }


    //AssetBundle.LoadFromFile加载方式 
    void LoadFromFileExample() {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath+ "/cube.unity3d");
        if (ab!=null) {
            GameObject cube = ab.LoadAsset<GameObject>("Cube");
            Instantiate(cube);
        }
    }
    //AssetBundle.LoadFromMemoryAsync加载方式
    IEnumerator LoadFromMemoryAsync() {
        AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(Application.streamingAssetsPath + "/cube.unity3d"));
        yield return createRequest;
        AssetBundle ab = createRequest.assetBundle;
        if (ab != null)
        {
            GameObject cube = ab.LoadAsset<GameObject>("Cube");
            Instantiate(cube);
        }
    }
    //UnityWebRequest加载方式 
    IEnumerator InstantiateObject() {
        string url = "file:///"+Application.streamingAssetsPath + "/cube.unity3d";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url); ;
        yield return request.SendWebRequest();
        AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
        if (ab != null)
        {
            GameObject cube = ab.LoadAsset<GameObject>("Cube");
            Instantiate(cube);
        }

    }


    //使用加载manifest文件方法 加载方式 
    IEnumerator LoadManifest() {
        UnityWebRequest requestManifest = UnityWebRequestAssetBundle.GetAssetBundle("file:///" + Application.streamingAssetsPath + "/StandaloneWindows");
        yield return requestManifest.SendWebRequest();
        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(requestManifest);
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] dep = manifest.GetAllDependencies("cube.unity3d");
        AssetBundle[] abs = new AssetBundle[dep.Length];
        for (int i=0;i<dep.Length;i++) {
            Debug.Log(dep[i]);
            UnityWebRequest requestDepends = UnityWebRequestAssetBundle.GetAssetBundle("file:///" + Application.streamingAssetsPath + "/"+dep[i]);
            yield return requestDepends.SendWebRequest();
            AssetBundle assetBundleDepends = DownloadHandlerAssetBundle.GetContent(requestDepends);
            if (assetBundleDepends != null)
            {
                assetBundleDepends.LoadAsset(dep[i]);
            }                    
        }
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle("file:///" + Application.streamingAssetsPath+ "/cube.unity3d");
        yield return request.SendWebRequest();
        AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
        if (ab != null)
        {
            GameObject cube = ab.LoadAsset<GameObject>("Cube");
            Instantiate(cube);
        }
    }

}
