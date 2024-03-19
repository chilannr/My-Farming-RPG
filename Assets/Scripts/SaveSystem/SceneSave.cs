using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, bool> boolDictionary;    // string key is an identifier name we choose for this list
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, string> vector3Dictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public List<SceneItem> listSceneItem;
    public Dictionary<string, string> gridPropertyDetailsDictionary;
    public List<InventoryItem>[] listInvItemArray;
}
