using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningUiImpl : MonoBehaviour, IOpeningUi
{
    const string BackgroundName = "Background";
    const string TitleTextName = "TitleText";
    const string StartGameButtonName = "StartGameButton";
    const string StartGameLabelName = "StartGameLabel";
    const string CopyrightTextName = "CopyrightText";

    [SerializeField] Image backgroundImage;
    [SerializeField] TMP_Text titleText;
    [SerializeField] RectTransform startGameHitArea;
    [SerializeField] TMP_Text startGameLabel;
    [SerializeField] TMP_Text copyrightText;
    [SerializeField] string nextSceneName = "HouseChild";

    bool isStarting;

    void Awake()
    {
        AutoFindReferences();
        BindStartGameButton();
    }

    void Update()
    {
        if (isStarting || startGameHitArea == null)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(startGameHitArea, Input.mousePosition, null))
        {
            StartGame();
        }
    }

    public void AutoFindReferences()
    {
        if (backgroundImage == null)
        {
            Transform background = transform.Find(BackgroundName);
            if (background != null)
            {
                backgroundImage = background.GetComponent<Image>();
            }
        }

        if (titleText == null)
        {
            Transform title = transform.Find(TitleTextName);
            if (title != null)
            {
                titleText = title.GetComponent<TMP_Text>();
            }
        }

        if (startGameHitArea == null)
        {
            Transform startButton = transform.Find(StartGameButtonName);
            if (startButton != null)
            {
                startGameHitArea = startButton as RectTransform;
            }
        }

        if (startGameLabel == null)
        {
            Transform startLabel = startGameHitArea != null
                ? startGameHitArea.Find(StartGameLabelName)
                : transform.Find(StartGameButtonName + "/" + StartGameLabelName);
            if (startLabel != null)
            {
                startGameLabel = startLabel.GetComponent<TMP_Text>();
            }
        }

        if (copyrightText == null)
        {
            Transform copyright = transform.Find(CopyrightTextName);
            if (copyright != null)
            {
                copyrightText = copyright.GetComponent<TMP_Text>();
            }
        }
    }

    public void BindStartGameButton()
    {
        // Click is handled in Update via startGameHitArea; no Unity Button required.
    }

    public void StartGame()
    {
        if (isStarting)
        {
            return;
        }

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next scene name is empty.");
            return;
        }

        isStarting = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
