using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ProjectEnums;

public class SceneLoader : MonoBehaviour {

    public static SceneLoader instance;
    public bool sceneLoadInProgress;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void LoadScene(int sceneBuildIndex, object parameters = null)
    {
        StartCoroutine(LoadSceneInternal(SceneManager.GetSceneAt(sceneBuildIndex).name, parameters));
    }

    public void LoadScene(string sceneName, object parameters = null)
    {
        StartCoroutine(LoadSceneInternal(sceneName, parameters));
    }

    private IEnumerator LoadSceneInternal(string sceneName, object parameters)
    {
        WindowManager.instance.ShowWindow(WindowPanel.LoadingScreen);
        sceneLoadInProgress = true;
        yield return new WaitForSecondsRealtime(Gval.panelAnimationDuration);

        WindowManager.instance.CloseWindowsOnSceneLoad();
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(Gval.mininumLoadingScreenDisplayTime - Gval.panelAnimationDuration);
        while (!asyncOp.isDone)
        {
            if (asyncOp.progress >= 0.9F)
            {
                sceneLoadInProgress = false;
                asyncOp.allowSceneActivation = true;
                yield return null;
            }
        }
        WindowManager.instance.escapeableWindowStack.Clear();
        if (sceneName != "Gameplay")
        {
            WindowManager.instance.CloseWindow(WindowPanel.LoadingScreen);
        }
    }

}