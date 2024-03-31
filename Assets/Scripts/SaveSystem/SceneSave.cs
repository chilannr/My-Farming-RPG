using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // 字典,存储场景中的int类型数据,键为字符串标识符
    public Dictionary<string, int> intDictionary;

    // 字典,存储场景中的bool类型数据,键为字符串标识符
    public Dictionary<string, bool> boolDictionary;

    // 字典,存储场景中的字符串类型数据,键为字符串标识符
    public Dictionary<string, string> stringDictionary;

    // 字典,存储场景中的Vector3类型数据,键为字符串标识符
    public Dictionary<string, Vector3Serializable> vector3Dictionary;

    // 字典,存储场景中的int数组类型数据,键为字符串标识符
    public Dictionary<string, int[]> intArrayDictionary;

    // 列表,存储场景中的SceneItem对象
    public List<SceneItem> listSceneItem;

    // 字典,存储场景中的GridPropertyDetails对象,键为字符串标识符
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;

    // 数组,存储场景中的InventoryItem列表
    public List<InventoryItem>[] listInvItemArray;
}