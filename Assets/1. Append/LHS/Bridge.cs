using System.Runtime.InteropServices;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    // 유니티 씬에 이 컴포넌트를 지니는 오브젝트가 존재해야 작동한다.

    #region Unity -> JavaScript
    [DllImport("__Internal")]
    private static extern void ExecuteJavaScriptMethod(string method);

    public static void OpenLeaderBoard()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ExecuteJavaScriptMethod("openLeaderBoard()");
#else
        Debug.Log("리더보드 오픈");
#endif
    }

    public static void SubmitScore(int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ExecuteJavaScriptMethod($"submitScore({score})");
#else
        Debug.Log("점수 제출");
#endif
    }
    #endregion

    #region JavaScript -> Unity
    public void OnVisibilityChanged(string state)
    {
        if (state == "hidden")
        {
            AudioListener.pause = true;
            Time.timeScale = 0f;
        }
        else
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
        }
    }
    #endregion
}
