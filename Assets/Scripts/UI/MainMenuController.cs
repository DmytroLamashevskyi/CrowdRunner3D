using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField, Tooltip("Regular game scene name.")]
    private string _gameScene = "Game";
    [SerializeField, Tooltip("Endless/infinity mode scene name.")]
    private string _infinityScene = "InfinityGame";
    [SerializeField, Tooltip("Shop scene name (optional).")]
    private string _shopScene = "";

    [Header("UI Roots")]
    [SerializeField, Tooltip("Canvas that contains the menu.")]
    private Transform _mainMenuCanvas;
    [SerializeField, Tooltip("Root where buttons live (can be a nested child like 'MainMenu').")]
    private Transform _buttonsRoot;
    [SerializeField, Tooltip("Settings panel GameObject (hidden by default).")]
    private GameObject _settingsPanel;
    [SerializeField, Tooltip("About panel GameObject (hidden by default).")]
    private GameObject _aboutPanel;

    [Header("Systems")]
    [SerializeField, Tooltip("Scene loader for fade transitions (optional).")]
    private SceneLoader _loader;

    [Header("Buttons (Optional manual assign)")]
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _infinityBtn;
    [SerializeField] private Button _shopBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _aboutBtn;
    [SerializeField, Tooltip("Back button in Settings panel (optional)")]
    private Button _settingsBackBtn;
    [SerializeField, Tooltip("Back button in About panel (optional)")]
    private Button _aboutBackBtn;

    private void Reset()
    {
        if(!_mainMenuCanvas)
        {
            var t = transform.Find("MainMenuCanvas");
            if(t) _mainMenuCanvas = t;
        }
        if(!_buttonsRoot && _mainMenuCanvas)
        {
            var m = _mainMenuCanvas.Find("MainMenu");
            _buttonsRoot = m ? m : _mainMenuCanvas;
        }
    }

    private void Awake()
    {
        if(!_mainMenuCanvas) _mainMenuCanvas = transform;
        if(!_buttonsRoot) _buttonsRoot = _mainMenuCanvas;

        // Автопоиск, если поля не проставлены вручную
        _startBtn = _startBtn ? _startBtn : FindBtnInChildren("Start Button");
        _infinityBtn = _infinityBtn ? _infinityBtn : FindBtnInChildren("Infinity Game");
        _shopBtn = _shopBtn ? _shopBtn : FindBtnInChildren("Shop");
        _settingsBtn = _settingsBtn ? _settingsBtn : FindBtnInChildren("Settings");
        _aboutBtn = _aboutBtn ? _aboutBtn : FindBtnInChildren("About");

        // Навешиваем обработчики
        _startBtn?.onClick.AddListener(PlayRegular);
        _infinityBtn?.onClick.AddListener(PlayInfinity);
        _shopBtn?.onClick.AddListener(OpenShop);
        _settingsBtn?.onClick.AddListener(ShowSettings);
        _aboutBtn?.onClick.AddListener(ShowAbout);

        _settingsBackBtn?.onClick.AddListener(ClosePanels);
        _aboutBackBtn?.onClick.AddListener(ClosePanels);

        // Начальное состояние
        _settingsPanel?.SetActive(false);
        _aboutPanel?.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    private Button FindBtnInChildren(string name)
    {
        var t = FindDeep(_buttonsRoot, name);
        var b = t ? t.GetComponent<Button>() : null;
        if(!b) Debug.LogWarning($"[MainMenu] Button '{name}' not found under '{_buttonsRoot?.name}'.");
        return b;
    }

    // Рекурсивный поиск по имени
    private static Transform FindDeep(Transform root, string name)
    {
        if(!root) return null;
        if(root.name == name) return root;
        for(int i = 0; i < root.childCount; i++)
        {
            var r = FindDeep(root.GetChild(i), name);
            if(r) return r;
        }
        return null;
    }

    public void PlayRegular() => LoadSceneSafe(_gameScene);
    public void PlayInfinity() => LoadSceneSafe(_infinityScene);

    public void OpenShop()
    {
        if(string.IsNullOrEmpty(_shopScene))
        {
            Debug.Log("[MainMenu] Shop scene not set.");
            return;
        }
        LoadSceneSafe(_shopScene);
    }

    public void ShowSettings()
    {
        _settingsPanel?.SetActive(true);
        _aboutPanel?.SetActive(false);
    }

    public void ShowAbout()
    {
        _settingsPanel?.SetActive(false);
        _aboutPanel?.SetActive(true);
    }

    public void ClosePanels()
    {
        _settingsPanel?.SetActive(false);
        _aboutPanel?.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadSceneSafe(string scene)
    {
        if(string.IsNullOrEmpty(scene))
        {
            Debug.LogWarning("[MainMenu] Scene name is empty.");
            return;
        }
        if(_loader) _loader.LoadScene(scene);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
