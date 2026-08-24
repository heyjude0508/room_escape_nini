using System;
using System.Collections.Generic;

[Serializable]
public class PrologueLines
{
    public const string CommandWait = "Wait";
    public const string CommandSpotlightOn = "SpotlightOn";

    public List<PrologueLine> lines;

    public PrologueLines()
    {
        lines = new List<PrologueLine>
        {
            new PrologueLine(
                "Wha... What happened?",
                new List<PrologueCommand>
                {
                    new PrologueCommand(CommandSpotlightOn),
                    new PrologueCommand(CommandWait, 1f)
                }),
            new PrologueLine("What's that there?")
        };
    }
}
