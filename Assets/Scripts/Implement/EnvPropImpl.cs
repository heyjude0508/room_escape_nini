//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvPropImpl : MonoBehaviour, IEnvProp
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] EnvProp envProp;

    void Awake()
    {
        if (envProp == null)
        {
            return;
        }

        if (envProp.animationSource == null)
        {
            envProp.animationSource = GetComponent<Animation>();
        }

        if (envProp.audioSource == null)
        {
            envProp.audioSource = GetComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EventAimStart()
    {
        //if (gameEventAimStart != null)
        //{
        //    gameEventAimStart.Raise();
        //}

        //if (dtAnim != null)
        //{
        //    dtAnim.DOPlay();
        //}
    }

    public void EventAimEnd()
    {
        //gameEventAimEnd.Raise();
    }

    public void EventInteract()
    {
        //gameEventInteract.Raise();
    }

    public string GetDescription()
    {
        if (envProp == null)
        {
            return string.Empty;
        }

        return envProp.propStatus == EnvPropStatusEnum.ENV_PROP_OPEN
            ? envProp.propOpenStatusDesc
            : envProp.propCloseStatusDesc;
    }

    public void Interact()
    {
        ChangeStatus();
    }

    public void ChangeStatus()
    {
        if (envProp == null)
        {
            return;
        }

        if (envProp.propStatus == EnvPropStatusEnum.ENV_PROP_OPEN)
        {
            envProp.propStatus = EnvPropStatusEnum.ENV_PROP_CLOSE;
            PlayAnimation(envProp.closeAnimationName);
            PlaySound(envProp.closeSound);
            return;
        }

        envProp.propStatus = EnvPropStatusEnum.ENV_PROP_OPEN;
        PlayAnimation(envProp.openAnimationName);
        PlaySound(envProp.openSound);
    }

    public void PlayAnimation(string animationName)
    {
        if (envProp.animationSource == null)
        {
            Debug.LogWarning("Animation is missing");
            return;
        }

        if (!string.IsNullOrEmpty(animationName))
        {
            envProp.animationSource.Play(animationName);
            return;
        }

        envProp.animationSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            return;
        }

        if (envProp.audioSource != null)
        {
            envProp.audioSource.PlayOneShot(sound);
            return;
        }

        AudioSource.PlayClipAtPoint(sound, transform.position);
    }
}
