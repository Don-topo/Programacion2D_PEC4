
using UnityEngine;

public static class FileManager
{

    public static Collaborators LoadCollaborators(TextAsset textAsset)
    {
        Collaborators collaborators = JsonUtility.FromJson<Collaborators>(textAsset.text);
        return collaborators;
    }

}
