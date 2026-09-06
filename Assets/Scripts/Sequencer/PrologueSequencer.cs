using System.Collections;
using UnityEngine;

public class PrologueSequencer : MonoBehaviour
{
    const float EnterKeyPulseInterval = 0.6f;
    const float SpotLightHeightOffset = 3f;
    const float OpeningWaitingTime = 1f;
    const float LinesIntervalTime = 0.6f;

    PrologueUiImpl prologueUi;
    Light spotLight;
    Transform desk;
    GameObject iconTip;

    string meAvatar = "Me";
    string pieAvatar = "Pie";
    string rossAvatar = "Ross";

    Coroutine prologueCoroutine;
    Coroutine enterKeyPulseCoroutine;

    void Start()
    {
        prologueUi = FindObjectOfType<PrologueUiImpl>();
        if (prologueUi == null)
        {
            Debug.LogError("PrologueUiImpl not found in scene.");
            return;
        }

        GameObject spotLightObject = GameObject.Find("SpotLight");
        if (spotLightObject != null)
        {
            spotLight = spotLightObject.GetComponent<Light>();
            if (spotLight != null)
            {
                spotLight.enabled = false;
            }
        }
        else
        {
            Debug.LogError("Spot Light not found in scene.");
        }

        GameObject deskObject = GameObject.Find("Desk");
        if (deskObject != null)
        {
            desk = deskObject.transform;
        }
        else
        {
            Debug.LogError("Desk not found in scene.");
        }

        CacheIconTip();
        if (iconTip != null)
        {
            iconTip.SetActive(false);
        }
        else
        {
            Debug.LogError("IconTip not found under Desk.");
        }

        prologueCoroutine = StartCoroutine(PorologuePlot());
    }

    public IEnumerator PorologuePlot()
    {
        yield return new WaitForSeconds(OpeningWaitingTime);
        prologueUi.PlayLines(meAvatar, "What... What's happening? Where am I?");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        ShineSpotLightOnDesk();
        EnableIconTip();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(meAvatar, "What's there? Some drawings?");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(pieAvatar, "Don't know what it is? Of courese, man like you never cares.");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(rossAvatar, "Yeah, he only cares about himself.");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(meAvatar, "You guys? Pie and Ross? Wait! You can talk!?");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(pieAvatar, "Wanna know what's happening? Go to the table and have a look by yourself.");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(rossAvatar, "Listen carefully, press A, W, S, D to move.");
        yield return WaitForContinue();
    }

    void ShineSpotLightOnDesk()
    {
        if (spotLight == null || desk == null)
        {
            return;
        }

        spotLight.transform.position = desk.position + Vector3.up * SpotLightHeightOffset;
        spotLight.transform.LookAt(desk.position);
        spotLight.enabled = true;
    }

    void EnableIconTip()
    {
        CacheIconTip();
        if (iconTip == null)
        {
            Debug.LogError("EnableIconTip failed: IconTip not found.");
            return;
        }

        iconTip.SetActive(true);
    }

    void CacheIconTip()
    {
        if (iconTip != null)
        {
            return;
        }

        if (desk != null)
        {
            Transform found = FindChildRecursive(desk, "IconTip");
            if (found != null)
            {
                iconTip = found.gameObject;
                return;
            }

            IconUiImpl deskTip = desk.GetComponentInChildren<IconUiImpl>(true);
            if (deskTip != null)
            {
                iconTip = deskTip.gameObject;
                return;
            }
        }

        IconUiImpl[] tips = FindObjectsOfType<IconUiImpl>(true);
        for (int i = 0; i < tips.Length; i++)
        {
            if (tips[i] != null && tips[i].gameObject.name == "IconTip")
            {
                iconTip = tips[i].gameObject;
                return;
            }
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

        while (!prologueUi.IsEnterPressed())
        {
            yield return null;
        }

        if (enterKeyPulseCoroutine != null)
        {
            StopCoroutine(enterKeyPulseCoroutine);
            enterKeyPulseCoroutine = null;
        }

        prologueUi.HideLines();
    }

    IEnumerator EnterkeyReminder()
    {
        while (true)
        {
            prologueUi.PulseEnterKey();
            yield return new WaitForSeconds(EnterKeyPulseInterval);
            prologueUi.ResetEnterKeyScale();
            yield return new WaitForSeconds(EnterKeyPulseInterval);
        }
    }
}
