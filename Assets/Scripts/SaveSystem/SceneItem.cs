[System.Serializable]
/// <summary>
/// This class is used to store the data of an item in the scene.
/// </summary>
public class SceneItem
{
    public int itemCode;
    public Vector3Serializable position;
    public string itemName;

    public SceneItem()
    {
        position = new Vector3Serializable();
    }
}
