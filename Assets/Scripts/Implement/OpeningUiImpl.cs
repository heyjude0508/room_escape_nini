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
    [SerializeField] Color normalLabelColor = new Color(1f, 0.894f, 0.71f, 1f);
    [SerializeField] Color hoverLabelColor = new Color(1f, 0.78f, 0.35f, 1f);
    [SerializeField] Material readableTextMaterial;

    bool isStarting;
    Vector3 startGameBaseScale = Vector3.one;
    bool hasStartGameBaseScale;

    void Awake()
    {
        AutoFindReferences();
        ApplyReadableTextStyle();
        CacheStartGameBaseScale();
        BindStartGameButton();
    }

    void ApplyReadableTextStyle()
    {
        Material material = readableTextMaterial;
        if (material == null)
        {
            material = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Drop Shadow");
        }

        normalLabelColor = new Color(1f, 0.894f, 0.71f, 1f);
        hoverLabelColor = new Color(1f, 0.78f, 0.35f, 1f);

        ApplyTextStyle(titleText, new Color(1f, 0.965f, 0.91f, 1f), material);
        ApplyTextStyle(startGameLabel, normalLabelColor, material);
        ApplyTextStyle(copyrightText, new Color(1f, 0.965f, 0.91f, 0.9f), material);
    }

    static void ApplyTextStyle(TMP_Text text, Color color, Material material)
    {
        if (text == null)
        {
            return;
        }

        text.color = color;
        if (material != null)
        {
            text.fontSharedMaterial = material;
        }
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
