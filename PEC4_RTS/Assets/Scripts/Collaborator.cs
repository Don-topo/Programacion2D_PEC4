

using System;

[Serializable]
public class Collaborator
{
    public string name;
    public string aportation;

    public Collaborator(Collaborator collaborator)
    {
        name = collaborator.name;
        aportation = collaborator.aportation;
    }
}
