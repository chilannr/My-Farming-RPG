using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 将返回的属性高度加倍，以适应我们将绘制的额外项目代码描述
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 在父属性上使用BeginProperty / EndProperty，以便预设覆盖逻辑适用于整个属性

        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck(); // 开始检查更改的值

            // 绘制项目代码
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            // 绘制项目描述
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description", GetItemDescription(property.intValue));

            // 如果项目代码值已更改，则将值设置为新值
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }
    ///
    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_itemList;

        // 加载包含项目列表的ScriptableObject
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Item/so_ItemList.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = so_itemList.itemDetails;

        // 查找具有匹配项目代码的项目详情
        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode);

        if (itemDetail != null)
        {
            return itemDetail.itemDescription;
        }
        else
        {
            return "";
        }
    }
}