using System;
using System.Collections.Generic;

[Serializable]
public class PrologueLine
{
    public string text;
    public List<PrologueCommand> onComplete;

    public PrologueLine()
    {
        text = string.Empty;
        onComplete = new List<PrologueCommand>();
    }

    public PrologueLine(string text, List<PrologueCommand> onComplete = null)
    {
        this.text = text;
        this.onComplete = onComplete ?? new List<PrologueCommand>();
    }
}
