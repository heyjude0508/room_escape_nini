using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningUiImpl : MonoBehaviour, IOpeningUi
{
    [SerializeField] OpeningUi openingUi;

    Material readableTextMaterial;
    bool isStarting;
    Vector3 startGameBaseScale = Vector3.one;
    bool hasStartGameBaseScale;

    void Awake()
    {
        if (openingUi == null)
        {
            openingUi = new OpeningUi();
        }

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

        openingUi.normalLabelColor = new Color(1f, 0.894f, 0.71f, 1f);
        openingUi.hoverLabelColor = new Color(1f, 0.78f, 0.35f, 1f);

        ApplyTextStyle(openingUi.titleText, new Color(1f, 0.965f, 0.91f, 1f), material);
        ApplyTextStyle(openingUi.startGameLabel, openingUi.normalLabelColor, material);
        ApplyTextStyle(openingUi.copyrightText, new Color(1f, 0.965f, 0.91f, 0.9f), material);
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
        if (isStarting || openingUi == null || openingUi.startGameHitArea == null)
        {
            return;
        }

        bool isHovered = RectTransformUtility.RectangleContainsScreenPoint(
            openingUi.startGameHitArea,
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
        if (openingUi == null || openingUi.startGameHitArea == null)
        {
            return;
        }

        startGameBaseScale = openingUi.startGameHitArea.localScale;
        hasStartGameBaseScale = true;
    }

    void UpdateStartGameHoverVisual(bool isHovered)
    {
        if (!hasStartGameBaseScale)
        {
            CacheStartGameBaseScale();
        }

        if (openingUi.startGameHitArea == null)
        {
            return;
        }

        float targetScaleFactor = isHovered ? openingUi.hoverScale : 1f;
        Vector3 targetScale = startGameBaseScale * targetScaleFactor;
        float t = 1f - Mathf.Exp(-openingUi.hoverAnimSpeed * Time.unscaledDeltaTime);
        openingUi.startGameHitArea.localScale = Vector3.Lerp(
            openingUi.startGameHitArea.localScale,
            targetScale,
            t);

        if (openingUi.startGameLabel != null)
        {
            Color targetColor = isHovered ? openingUi.hoverLabelColor : openingUi.normalLabelColor;
            openingUi.startGameLabel.color = Color.Lerp(openingUi.startGameLabel.color, targetColor, t);
        }
    }

    public void AutoFindReferences()
    {
        if (openingUi == null)
        {
            openingUi = new OpeningUi();
        }

        if (openingUi.backgroundImage == null)
        {
            Transform background = transform.Find(openingUi.backgroundName);
            if (background != null)
            {
                openingUi.backgroundImage = background.GetComponent<Image>();
            }
        }

        if (openingUi.titleText == null)
        {
            Transform title = transform.Find(openingUi.titleTextName);
            if (title != null)
            {
                openingUi.titleText = title.GetComponent<TMP_Text>();
            }
        }

        if (openingUi.startGameHitArea == null)
        {
            Transform startButton = transform.Find(openingUi.startGameButtonName);
            if (startButton != null)
            {
                openingUi.startGameHitArea = startButton as RectTransform;
            }
        }

        if (openingUi.startGameLabel == null)
        {
            Transform startLabel = openingUi.startGameHitArea != null
                ? openingUi.startGameHitArea.Find(openingUi.startGameLabelName)
                : transform.Find(openingUi.startGameButtonName + "/" + openingUi.startGameLabelName);
            if (startLabel != null)
            {
                openingUi.startGameLabel = startLabel.GetComponent<TMP_Text>();
            }
        }

        if (openingUi.copyrightText == null)
        {
            Transform copyright = transform.Find(openingUi.copyrightTextName);
            if (copyright != null)
            {
                openingUi.copyrightText = copyright.GetComponent<TMP_Text>();
            }
        }
    }

    public void BindStartGameButton()
    {
        // Click and hover are handled in Update via startGameHitArea.
    }

    public void StartGame()
    {
        if (isStarting || openingUi == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(openingUi.nextSceneName))
        {
            Debug.LogError("Next scene name is empty.");
            return;
        }

        isStarting = true;
        SceneManager.LoadScene(openingUi.nextSceneName);
    }
}
