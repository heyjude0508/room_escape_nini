using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseChildSequencer : MonoBehaviour
{
    const float OpeningWaitingTime = 1f;
    const float ShowTipTime = 5f;
    
    const float LinesIntervalTime = 0.6f;
    const float EnterKeyPulseInterval = 0.6f;

    const float FragmentPlotTime = 0.6f;
    const string FragmentTlItemId = "F001";

    PlotUiImpl plotUiImpl;

    string meAvatar = "Me";
    string pieAvatar = "Pie";
    string rossAvatar = "Ross";

    Coroutine houseCoroutine;
    Coroutine fragmentPlotCoroutine;
    Coroutine enterKeyPulseCoroutine;
    bool fragmentTlPlotPlayed;

    void Start()
    {
        plotUiImpl = FindObjectOfType<PlotUiImpl>();
        if (plotUiImpl == null)
        {
            Debug.LogError("PlotUiImpl not found in scene.");
            return;
        }

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

    public IEnumerator PorologuePlot()
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

    public IEnumerator FragmentPlot()
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

        fragmentPlotCoroutine = null;
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
