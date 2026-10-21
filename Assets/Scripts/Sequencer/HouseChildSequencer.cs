using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseChildSequencer : MonoBehaviour
{
    const float OpeningWaitingTime = 1f;
    const float ShowTipTime = 5f;
    
    const float LinesIntervalTime = 0.6f;
    const float EnterKeyPulseInterval = 0.6f;

    const float FragmentPlotTime = 0.6f;
    const float RollingCartBlackoutSeconds = 0.8f;
    const string FragmentTlItemId = "F001";
    const string MainDoorPuzzleId = "MainLock";
    const string KitchenTriggerId = "Tri001";
    const string RollingCartObjectName = "RollingCart_01";

    const string ActPanelName = "ActPanel";
    const string BlackoutCanvasName = "RollingCartBlackoutCanvas";

    PlotUiImpl plotUiImpl;
    GameObject actPanel;
    GameObject blackoutRoot;
    Image blackoutImage;

    string meAvatar = "Me";
    string pieAvatar = "Pie";
    string rossAvatar = "Ross";

    Coroutine houseCoroutine;
    Coroutine fragmentPlotCoroutine;
    Coroutine mainDoorPlotCoroutine;
    Coroutine kitchenPlotCoroutine;
    Coroutine chefAssigCoroutine;
    Coroutine chefReactPlotCoroutine;
    Coroutine enterKeyPulseCoroutine;
    Coroutine rollingCartSolvedCoroutine;
    bool fragmentTlPlotPlayed;
    bool mainDoorPlotPlayed;
    bool kitchenPlotPlayed;
    bool chefReactPlotPlayed;
    bool rollingCartSolvedPlayed;
    bool plotBlockingInteractionUi;

    public bool IsBlockingInteractionUi => plotBlockingInteractionUi;

    void Start()
    {
        plotUiImpl = FindObjectOfType<PlotUiImpl>();
        if (plotUiImpl == null)
        {
            Debug.LogError("PlotUiImpl not found in scene.");
            return;
        }

        CacheActPanel();
        houseCoroutine = StartCoroutine(PorologuePlot());
    }

    public void NotifyItemPicked(string itemId)
    {
        if (fragmentTlPlotPlayed || string.IsNullOrEmpty(itemId))
        {
            return;
        }

        if (!string.Equals(itemId, FragmentTlItemId, System.StringComparison.Ordinal))
        {
            return;
        }

        if (plotUiImpl == null)
        {
            plotUiImpl = FindObjectOfType<PlotUiImpl>();
            if (plotUiImpl == null)
            {
                Debug.LogError("PlotUiImpl not found in scene.");
                return;
            }
        }

        fragmentTlPlotPlayed = true;
        if (fragmentPlotCoroutine != null)
        {
            StopCoroutine(fragmentPlotCoroutine);
        }

        fragmentPlotCoroutine = StartCoroutine(FragmentPlot());
    }

    public void NotifyLockInteracted(string puzzleId, bool canSolve)
    {
        if (canSolve || mainDoorPlotPlayed || string.IsNullOrEmpty(puzzleId))
        {
            return;
        }

        if (!string.Equals(puzzleId, MainDoorPuzzleId, System.StringComparison.Ordinal))
        {
            return;
        }

        if (plotUiImpl == null)
        {
            plotUiImpl = FindObjectOfType<PlotUiImpl>();
            if (plotUiImpl == null)
            {
                Debug.LogError("PlotUiImpl not found in scene.");
                return;
            }
        }

        mainDoorPlotPlayed = true;
        if (mainDoorPlotCoroutine != null)
        {
            StopCoroutine(mainDoorPlotCoroutine);
        }

        mainDoorPlotCoroutine = StartCoroutine(MainDoorPlot());
    }

    public void NotifyTriggerEntered(string triggerId)
    {
        if (kitchenPlotPlayed || string.IsNullOrEmpty(triggerId))
        {
            return;
        }

        if (!string.Equals(triggerId, KitchenTriggerId, System.StringComparison.Ordinal))
        {
            return;
        }

        if (plotUiImpl == null)
        {
            plotUiImpl = FindObjectOfType<PlotUiImpl>();
            if (plotUiImpl == null)
            {
                Debug.LogError("PlotUiImpl not found in scene.");
                return;
            }
        }

        kitchenPlotPlayed = true;
        if (kitchenPlotCoroutine != null)
        {
            StopCoroutine(kitchenPlotCoroutine);
        }

        kitchenPlotCoroutine = StartCoroutine(KitchenPlot());
    }

    public void NotifyNpcInteracted(Npc npc)
    {
        if (npc == null || string.IsNullOrEmpty(npc.npcAvatarName))
        {
            return;
        }

        if (plotBlockingInteractionUi)
        {
            return;
        }

        if (plotUiImpl == null)
        {
            plotUiImpl = FindObjectOfType<PlotUiImpl>();
            if (plotUiImpl == null)
            {
                Debug.LogError("PlotUiImpl not found in scene.");
                return;
            }
        }

        if (chefAssigCoroutine != null)
        {
            StopCoroutine(chefAssigCoroutine);
        }

        chefAssigCoroutine = StartCoroutine(ChefAssigThenReact(npc.npcAvatarName));
    }

    public void NotifyRollingCartSolved()
    {
        if (rollingCartSolvedPlayed)
        {
            return;
        }

        rollingCartSolvedPlayed = true;
        if (rollingCartSolvedCoroutine != null)
        {
            StopCoroutine(rollingCartSolvedCoroutine);
        }

        rollingCartSolvedCoroutine = StartCoroutine(RollingCartSolvedSequence());
    }

    IEnumerator RollingCartSolvedSequence()
    {
        BeginPlotBlocking();
        try
        {
            SetBlackoutVisible(true);
            yield return new WaitForSeconds(RollingCartBlackoutSeconds);
            HideRollingCart01();
            SetBlackoutVisible(false);
        }
        finally
        {
            rollingCartSolvedCoroutine = null;
            EndPlotBlocking();
        }
    }

    void HideRollingCart01()
    {
        GameObject cart = GameObject.Find(RollingCartObjectName);
        if (cart == null)
        {
            // Include inactive in case something already disabled the root.
            Transform found = FindInSceneIncludingInactive(RollingCartObjectName);
            if (found != null)
            {
                cart = found.gameObject;
            }
        }

        if (cart == null)
        {
            Debug.LogWarning(RollingCartObjectName + " not found to hide after solve.");
            return;
        }

        // Detached IconTips are no longer children of the cart — disable by ownership.
        DisableIconTipsOwnedBy(cart.transform);
        cart.SetActive(false);
    }

    static void DisableIconTipsOwnedBy(Transform root)
    {
        if (root == null)
        {
            return;
        }

        IconUiImpl[] tips = FindObjectsOfType<IconUiImpl>(true);
        for (int i = 0; i < tips.Length; i++)
        {
            IconUiImpl tip = tips[i];
            if (tip == null || !tip.IsOwnedBy(root))
            {
                continue;
            }

            tip.enabled = false;
            tip.gameObject.SetActive(false);
        }
    }

    void SetBlackoutVisible(bool visible)
    {
        EnsureBlackoutUi();
        if (blackoutRoot == null)
        {
            return;
        }

        blackoutRoot.SetActive(visible);
        if (blackoutImage != null)
        {
            blackoutImage.color = Color.black;
        }
    }

    void EnsureBlackoutUi()
    {
        if (blackoutRoot != null && blackoutImage != null)
        {
            return;
        }

        blackoutRoot = GameObject.Find(BlackoutCanvasName);
        if (blackoutRoot != null)
        {
            blackoutImage = blackoutRoot.GetComponentInChildren<Image>(true);
            if (blackoutImage != null)
            {
                return;
            }
        }

        blackoutRoot = new GameObject(BlackoutCanvasName);
        Canvas canvas = blackoutRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32000;
        blackoutRoot.AddComponent<CanvasScaler>();
        blackoutRoot.AddComponent<GraphicRaycaster>();

        GameObject imageGo = new GameObject("Blackout", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageGo.transform.SetParent(blackoutRoot.transform, false);
        RectTransform rect = imageGo.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        blackoutImage = imageGo.GetComponent<Image>();
        blackoutImage.color = Color.black;
        blackoutImage.raycastTarget = true;
        blackoutRoot.SetActive(false);
    }

    static Transform FindInSceneIncludingInactive(string objectName)
    {
        Transform[] all = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || t.name != objectName)
            {
                continue;
            }

            // Skip assets / prefabs not in a loaded scene.
            if (t.gameObject.scene.IsValid() && t.gameObject.scene.isLoaded)
            {
                return t;
            }
        }

        return null;
    }

    IEnumerator ChefAssigThenReact(string npcAvatarName)
    {
        try
        {
            yield return ChefAssig(npcAvatarName);

            if (chefReactPlotPlayed)
            {
                yield break;
            }

            chefReactPlotPlayed = true;
            if (chefReactPlotCoroutine != null)
            {
                StopCoroutine(chefReactPlotCoroutine);
            }

            chefReactPlotCoroutine = StartCoroutine(ChefReactPlot());
            yield return chefReactPlotCoroutine;
        }
        finally
        {
            chefAssigCoroutine = null;
        }
    }

    public IEnumerator PorologuePlot()
    {
        BeginPlotBlocking();
        try
        {
            yield return new WaitForSeconds(OpeningWaitingTime);
            plotUiImpl.PlayLines(meAvatar, "Is this... my childhood living room?!");
            yield return WaitForContinue();

            yield return new WaitForSeconds(LinesIntervalTime);
            plotUiImpl.PlayLines(meAvatar, "It's crazy!!! I gotta escape from here!");
            yield return WaitForContinue();

            yield return new WaitForSeconds(LinesIntervalTime);
            plotUiImpl.PlayLines(pieAvatar, "This room's not crazy, and you're the crazy one.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(LinesIntervalTime);
            plotUiImpl.PlayLines(rossAvatar, "Find out the key of living room and one fragment of torn drawings, then you can leave from this room.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(LinesIntervalTime);
            plotUiImpl.PlayLines(rossAvatar, "Listen carefully, hold Shift to crouch.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(LinesIntervalTime);
            plotUiImpl.PlayLines(rossAvatar, "Press I to open and close your bag.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(LinesIntervalTime);
            plotUiImpl.PlayTip();
            yield return new WaitForSeconds(ShowTipTime);
            plotUiImpl.HideTip();
        }
        finally
        {
            EndPlotBlocking();
        }
    }

    public IEnumerator FragmentPlot()
    {
        BeginPlotBlocking();
        try
        {
            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "This is a fragment of the drawing I tore up this morning. I didn't even notice what my son drew...");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "I was so stressed out about losing money in the stock market.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "Looking back, I was way too impulsive, taking my anger out on my family like that.");
            yield return WaitForContinue();
        }
        finally
        {
            fragmentPlotCoroutine = null;
            EndPlotBlocking();
        }
    }

    public IEnumerator MainDoorPlot()
    {
        BeginPlotBlocking();
        try
        {
            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "There are three doors to get out of this house.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "But this's the only door with a working lock and the other two are broken.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "I need to find out the key.");
            yield return WaitForContinue();
        }
        finally
        {
            mainDoorPlotCoroutine = null;
            EndPlotBlocking();
        }
    }

    public IEnumerator KitchenPlot()
    {
        BeginPlotBlocking();
        try
        {
            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "Um... the kitchen is straight out of my childhood, but the dining room is from the present.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "It's weird.");
            yield return WaitForContinue();
        }
        finally
        {
            kitchenPlotCoroutine = null;
            EndPlotBlocking();
        }
    }

    public IEnumerator ChefAssig(string npcAvatarName)
    {
        BeginPlotBlocking();
        try
        {
            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(npcAvatarName, "I'm Chef Cat. I've been waiting a long time for you! Come help me out!");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(npcAvatarName, "I need to prepare food for your parents, but I don't know what their favorites are.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(npcAvatarName, "You're their son, so you must know.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(npcAvatarName, "Help me gather the ingredients for their favorite dish and put them on the cart. I'll cook for them.");
            yield return WaitForContinue();
        }
        finally
        {
            EndPlotBlocking();
        }
    }

    public IEnumerator ChefReactPlot()
    {
        BeginPlotBlocking();
        try
        {
            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "This black cat was my childhood plushie. I used to always make it play chef or waiter.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "First Pie and Ross, now this. Oh well, I'm used to it.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(pieAvatar, "It's a plushie, but our friend.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "Look, I regret what happened this morning, but I have a family to feed! Can you just let me go back?");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "Now I have to figure out what my parents' favorite food is? How could I not know that? I don't have time for this!");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "......");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(meAvatar, "Wait......");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(pieAvatar, "Looks like you noticed.");
            yield return WaitForContinue();

            yield return new WaitForSeconds(FragmentPlotTime);
            plotUiImpl.PlayLines(rossAvatar, "You have absolutely no idea what they like. So be a good boy and stay right here.");
            yield return WaitForContinue();
        }
        finally
        {
            chefReactPlotCoroutine = null;
            EndPlotBlocking();
        }
    }

    void BeginPlotBlocking()
    {
        plotBlockingInteractionUi = true;
        SetActPanelActive(false);
    }

    void EndPlotBlocking()
    {
        plotBlockingInteractionUi = false;
        // Leave ActPanel off; PlayerActionImpl will show it again when aiming an interactable.
        SetActPanelActive(false);
    }

    void CacheActPanel()
    {
        if (actPanel != null)
        {
            return;
        }

        if (plotUiImpl != null)
        {
            Transform found = FindChildRecursive(plotUiImpl.transform.root, ActPanelName);
            if (found == null)
            {
                found = FindChildRecursive(plotUiImpl.transform, ActPanelName);
            }

            if (found != null)
            {
                actPanel = found.gameObject;
                return;
            }
        }

        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Transform found = FindChildRecursive(canvases[i].transform, ActPanelName);
            if (found != null)
            {
                actPanel = found.gameObject;
                return;
            }
        }
    }

    void SetActPanelActive(bool active)
    {
        CacheActPanel();
        if (actPanel != null)
        {
            actPanel.SetActive(active);
        }
    }

    static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }

            Transform nested = FindChildRecursive(child, childName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    IEnumerator WaitForContinue()
    {
        enterKeyPulseCoroutine = StartCoroutine(EnterkeyReminder());

        while (!plotUiImpl.IsEnterPressed())
        {
            yield return null;
        }

        if (enterKeyPulseCoroutine != null)
        {
            StopCoroutine(enterKeyPulseCoroutine);
            enterKeyPulseCoroutine = null;
        }

        plotUiImpl.HideLines();
    }

    IEnumerator EnterkeyReminder()
    {
        while (true)
        {
            plotUiImpl.PulseEnterKey();
            yield return new WaitForSeconds(EnterKeyPulseInterval);
            plotUiImpl.ResetEnterKeyScale();
            yield return new WaitForSeconds(EnterKeyPulseInterval);
        }
    }
}
