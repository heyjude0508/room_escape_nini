using System.Collections;
using UnityEngine;

public class PrologueSequencer : MonoBehaviour
{
    const float EnterKeyPulseInterval = 0.6f;
    const float SpotLightHeightOffset = 3f;

    PrologueUiImpl prologueUi;
    Light spotLight;
    Transform desk;

    string meAvatar = "Me";
    string daBingAvatar = "DaBing";
    string rouSongAvatar = "RouSong";

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

        prologueCoroutine = StartCoroutine(PorologuePlot());
    }

    public IEnumerator PorologuePlot()
    {
        yield return new WaitForSeconds(2f);
        prologueUi.PlayLines("What... What happened? Where am I?", meAvatar);
        yield return WaitForContinue();
        yield return new WaitForSeconds(1f);
        ShineSpotLightOnDesk();

        yield return new WaitForSeconds(0.5f);
        prologueUi.PlayLines("What's there? Some drawings?", meAvatar);
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
