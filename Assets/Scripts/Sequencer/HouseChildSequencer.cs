using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseChildSequencer : MonoBehaviour
{
    const float EnterKeyPulseInterval = 0.6f;
    const float OpeningWaitingTime = 1f;
    const float LinesIntervalTime = 0.6f;

    PlotUiImpl plotUiImpl;

    string meAvatar = "Me";
    string pieAvatar = "Pie";
    string rossAvatar = "Ross";

    Coroutine houseCoroutine;
    Coroutine enterKeyPulseCoroutine;

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
