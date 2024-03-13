using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private Image _fadingImage;
    [SerializeField] private float _fadingDuration;
    [SerializeField] private string _menuSceneName;
    [SerializeField] private string _battlefieldSceneName;
    [SerializeField] private Slider _progressBar;

    public void GoToBattlefield()
    {
        StartCoroutine(StartLevelChanging(_battlefieldSceneName));
    }

    public void GoToMenu()
    {
        StartCoroutine(StartLevelChanging(_menuSceneName));
    }

    private IEnumerator StartLevelChanging(string sceneName)
    {
        _fadingImage.DOColor(new Color(0, 0, 0, 1), _fadingDuration)
            .OnComplete(() => _progressBar.gameObject.SetActive(true))
            .Play();

        yield return new WaitForSeconds(_fadingDuration);

        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while(!asyncLoad.isDone) {
            _progressBar.value = asyncLoad.progress;
            yield return null;
        }

        _fadingImage.DOColor(new Color(0, 0, 0, 0), _fadingDuration)
            .OnStart(() => _progressBar.gameObject.SetActive(false))
            .Play();
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
