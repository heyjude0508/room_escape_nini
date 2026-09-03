using System.Collections;
using UnityEngine;

public class PrologueSequencer : MonoBehaviour
{
    const float EnterKeyPulseInterval = 0.6f;

    PrologueUiImpl prologueUi;
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

        prologueCoroutine = StartCoroutine(PorologuePlot());
    }

    public IEnumerator PorologuePlot()
    {
        yield return new WaitForSeconds(2f);
        prologueUi.PlayLines("What... What happened?", meAvatar);
        enterKeyPulseCoroutine = StartCoroutine(EnterkeyReminder());
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
