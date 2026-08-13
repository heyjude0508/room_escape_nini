using UnityEngine;

[System.Serializable]
public class EnvProp
{
    public string id;
    public string propName;
    public string propCloseStatusDesc;
    public string propOpenStatusDesc;
    public EnvPropStatusEnum propStatus;

    public Animation animationSource;
    public string openAnimationName;
    public string closeAnimationName;

    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    public EnvProp(
        string id,
        string propName,
        string propCloseStatusDesc,
        string propOpenStatusDesc,
        EnvPropStatusEnum propStatus,
        Animation animationSource,
        string openAnimationName,
        string closeAnimationName,
        AudioSource audioSource,
        AudioClip openSound,
        AudioClip closeSound)
    {
        this.id = id;
        this.propName = propName;
        this.propCloseStatusDesc = propCloseStatusDesc;
        this.propOpenStatusDesc = propOpenStatusDesc;
        this.propStatus = propStatus;
        this.animationSource = animationSource;
        this.openAnimationName = openAnimationName;
        this.closeAnimationName = closeAnimationName;
        this.audioSource = audioSource;
        this.openSound = openSound;
        this.closeSound = closeSound;
    }

    public EnvProp()
    {
        id = "Default Prop";
        propName = "Default Prop";
        propCloseStatusDesc = "Press E to open the door";
        propOpenStatusDesc = "Press E to close the door";
        propStatus = EnvPropStatusEnum.ENV_PROP_CLOSE;

        animationSource = null;
        openAnimationName = null;
        closeAnimationName = null;

        audioSource = null;
        openSound = null;
        closeSound = null;
    }
}
