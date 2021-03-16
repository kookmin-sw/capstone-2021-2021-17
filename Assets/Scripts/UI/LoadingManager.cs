using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour{
    
    public static string nextScene;
    [SerializeField]
    Image progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

//로딩씬을 다음씬과 연결
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }

//로딩씬 가져오기
    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);

                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}

