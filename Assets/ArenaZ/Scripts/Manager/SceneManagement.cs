using RedApple;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneManagement : Singleton<SceneManagement>
{
    [SerializeField]
    private Image loaderImage;
    [SerializeField]
    private GameObject splashObj;
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private CanvasGroup mainCanvasGroup;
    string loadingString;

    private void Start()
    {
        StartCoroutine(LoadSceneAtStart());
    }
    IEnumerator LoadSceneAtStart()
    {
        yield return new WaitForSeconds(1f);
        splashObj.SetActive(false);
        LoadScene("LevelSelection");
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnOpenScene;
    }

    void OnDisable()
	{
		SceneManager.sceneLoaded -= OnOpenScene;
	}

	private void OnOpenScene(Scene scene, LoadSceneMode mode)
	{        
        if (mode == LoadSceneMode.Single)
        {
            mainCanvasGroup.gameObject.SetActive(true);
            mainCanvasGroup.alpha = 1;
            mainCanvasGroup.DOFade(0, 0.5f).OnComplete(() => mainCanvasGroup.gameObject.SetActive(false)).SetDelay(0.5f);
        }
	}

	#region LoadScene
	public void LoadScene(string sceneName)
	{
        loaderImage.fillAmount = 0;
        loadingText.text = loadingString +"0%...";
        mainCanvasGroup.gameObject.SetActive(true);
        mainCanvasGroup.alpha = 0;
        mainCanvasGroup.DOFade(1, 0.5f).OnComplete(()=> StartCoroutine(StartLoadScene(sceneName)));      
	}

	IEnumerator StartLoadScene(string sceneName)
	{
       
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		while(!async.isDone)
		{           
			loaderImage.fillAmount = Mathf.Lerp(loaderImage.fillAmount, async.progress,0.3f);            
            loadingText.text = loadingString + (int)(loaderImage.fillAmount * 100) + "%...";
			yield return null;
		}
        System.GC.Collect();
        loaderImage.DOFillAmount(1, 0.5f).OnUpdate(() => loadingText.text = loadingString+ (int)(loaderImage.fillAmount * 100) + "%...");	
	}	
	#endregion
}