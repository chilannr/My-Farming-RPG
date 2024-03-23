public interface ISaveable
{
    string ISaveableUniqueID { get; set; } // 可保存对象的唯一标识符

    GameObjectSave GameObjectSave { get; set; } // 保存对象的数据

    void ISaveableRegister(); // 注册可保存对象

    void ISaveableDeregister(); // 注销可保存对象

    GameObjectSave ISaveableSave(); // 将可保存对象的数据保存到GameObjectSave对象中，并返回该对象

    void ISaveableLoad(GameSave gameSave); // 从GameSave对象中加载可保存对象的数据

    void ISaveableStoreScene(string sceneName); // 将可保存对象的数据存储到指定场景中

    void ISaveableRestoreScene(string sceneName); // 从指定场景中恢复可保存对象的数据
}