using System;

[Serializable]
public class PrologueUi
{
    public PrologueLines prologueLines;

    public PrologueUi(PrologueLines prologueLines)
    {
        this.prologueLines = prologueLines;
    }

    public PrologueUi()
    {
        prologueLines = new PrologueLines();
    }
}
