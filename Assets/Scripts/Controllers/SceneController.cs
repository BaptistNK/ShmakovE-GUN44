using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneController : MonoBehaviour
{
    private ZenjectSceneLoader _sceneLoader;
    [Inject]
    public void Construct(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }
    public void OpenMainScene()
    {
        _sceneLoader.LoadScene(0, LoadSceneMode.Single);
    }
    public void OpenGameScene()
    {        
        Scene gameScene = SceneManager.GetSceneByBuildIndex(1);

        if (gameScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(1).completed += (action) =>
            {               
                _sceneLoader.LoadScene(1, LoadSceneMode.Additive);
            };
        }
        else
        {
            _sceneLoader.LoadScene(1, LoadSceneMode.Additive);
        }
    }
}
