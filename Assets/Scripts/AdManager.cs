using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    public delegate void ShowAdCallback(ShowResult result);
    
    protected ShowAdCallback callbackFunction;

    void Awake()
    {
        instance = this;
    }
	
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowAd("rewardedVideo");
        }
    }
#endif

    public void ShowAd(string zone = "", ShowAdCallback adCallback = null)
    {
        callbackFunction = adCallback;

//#if UNITY_EDITOR
//        StartCoroutine(WaitForAd());
//#endif

        if(string.Equals(zone, ""))
           zone = null;

        ShowOptions options = new ShowOptions();
        options.resultCallback = AdCallbackhandler;

        if (Advertisement.IsReady(zone))
            Advertisement.Show(zone, options);
        else
            AdCallbackhandler(ShowResult.Failed);
    }

    public bool IsAdReady(string zone = "")
    {
        return(Advertisement.IsReady(zone));
    }

//    IEnumerator WaitForAd()
//    {
//        float currentTimeScale = Time.timeScale;
//
//        Time.timeScale = 0.0f;
//        yield return null;
//
//        while (Advertisement.isShowing)
//            yield return null;
//
//        Time.timeScale = currentTimeScale;
//    }

    void AdCallbackhandler(ShowResult result)
    {
        if(callbackFunction != null)
            callbackFunction(result);
    }
}

