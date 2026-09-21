using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueSequencer : MonoBehaviour
{
    const float EnterKeyPulseInterval = 0.6f;
    const float SpotLightHeightOffset = 3f;
    const float OpeningWaitingTime = 1f;
    const float LinesIntervalTime = 0.6f;
    const float ShowTipTime = 5f;

    PrologueUiImpl prologueUi;
    Light spotLight;
    Transform desk;
    GameObject iconTip;

    string meAvatar = "Me";
    string pieAvatar = "Pie";
    string rossAvatar = "Ross";

    List<string> storyTextList = new List<string>
    {
        "I'm Pie. And I am a Silver Shaded British Shorthair bought by this family.",
        "This's my bro, ross. He am a stray Ragdoll cat found by the same family in the yard and got adopted by them.",
        "Here, we encountered loved ones who were kind, diligent, and responsible. We thought we would have a happy family.",
        "But, in this family, I don't know why there's always a man never knew how to appreciate what he had.",
        "This man is arrogant, irritable, lazy, and greedy. And he hurts others over and over again though he's a husband, a dad and our owner.",
        "Today, his poor kid merely drew a crayon pic of the family to show this scumbag, and it disturbed his day trading.",
        "He completely lost it, tore up the drawing, slapped the kid, had a huge fight with his wife, and even shattered our food bowls with a kick!!",
        "We couldn't take it anymore!! Couldn't!! So, we brought this bastard here. The cage of home. I know that's what it's always been to you. Yeah, this bastard is YOU!",
        "If you wanna go back to reality, back to your family... then piece crayon drawing back together. Take a look at the stupid things you've done. That is the only way to escape here!"

    };

    public IReadOnlyList<string> StoryTextList => storyTextList;

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

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(rossAvatar, "Press E to interact with interactable objects.");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(rossAvatar, "Hold Shift to crouch.");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayLines(rossAvatar, "Move the Mouse to look around.");
        yield return WaitForContinue();

        yield return new WaitForSeconds(LinesIntervalTime);
        prologueUi.PlayTip();
        yield return new WaitForSeconds(ShowTipTime);
        prologueUi.HideTip();
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
