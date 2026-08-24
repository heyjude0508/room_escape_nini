using System;
using System.Collections.Generic;

[Serializable]
public class PrologueCommand
{
    public string id;
    public float value;
    public string target;

    public PrologueCommand()
    {
    }

    public PrologueCommand(string id, float value = 0f, string target = null)
    {
        this.id = id;
        this.value = value;
        this.target = target;
    }
}
