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
    [SerializeField] float hoverScale = 1.08f;
    [SerializeField] float hoverAnimSpeed = 12f;
    [SerializeField] Color normalLabelColor = Color.white;
    [SerializeField] Color hoverLabelColor = new Color(1f, 1f, 1f, 1f);

    bool isStarting;
    Vector3 startGameBaseScale = Vector3.one;
    bool hasStartGameBaseScale;

    void Awake()
    {
        AutoFindReferences();
        CacheStartGameBaseScale();
        BindStartGameButton();
    }

    void Update()
    {
        if (isStarting || startGameHitArea == null)
        {
            return;
        }

        bool isHovered = RectTransformUtility.RectangleContainsScreenPoint(
            startGameHitArea,
            Input.mousePosition,
            null);

        UpdateStartGameHoverVisual(isHovered);

        if (isHovered && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    void CacheStartGameBaseScale()
    {
        if (startGameHitArea == null)
        {
            return;
        }

        startGameBaseScale = startGameHitArea.localScale;
        hasStartGameBaseScale = true;
    }

    void UpdateStartGameHoverVisual(bool isHovered)
    {
        if (!hasStartGameBaseScale)
        {
            CacheStartGameBaseScale();
        }

        float targetScaleFactor = isHovered ? hoverScale : 1f;
        Vector3 targetScale = startGameBaseScale * targetScaleFactor;
        float t = 1f - Mathf.Exp(-hoverAnimSpeed * Time.unscaledDeltaTime);
        startGameHitArea.localScale = Vector3.Lerp(startGameHitArea.localScale, targetScale, t);

        if (startGameLabel != null)
        {
            Color targetColor = isHovered ? hoverLabelColor : normalLabelColor;
            startGameLabel.color = Color.Lerp(startGameLabel.color, targetColor, t);
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

        if (startGameLabel != null)
        {
            normalLabelColor = startGameLabel.color;
            // Soft warm tint reads better than brightening already-white text.
            hoverLabelColor = new Color(1f, 0.92f, 0.72f, normalLabelColor.a);
        }
    }

    public void BindStartGameButton()
    {
        // Click and hover are handled in Update via startGameHitArea.
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
