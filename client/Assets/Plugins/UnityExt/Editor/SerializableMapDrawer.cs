using System;

using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;
using System.Reflection;

namespace UnityExt.Editor
{
    [CustomPropertyDrawer(typeof(SerializableMap), true)]
    public class SerializableMapDrawer : PropertyDrawer
    {
        private const string KEY_HEADER = "__UnityExt.Editor.SerializableMapDrawer.Header__";

        private const float HEIGHT_HEADER = 20f;
        private const float HEIGHT_ITEMS = 18f;
        private const float WIDTH_KEY_PERCENT = 0.25f;
        private const float WIDTH_VALUE_PERCENT = 0.75f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool state = EditorPrefs.GetBool(KEY_HEADER, true);
            SerializedProperty spKeys = property.FindPropertyRelative("keys");

            float height = 0f;
            // header的高度
            height += HEIGHT_HEADER;
            // 所有item的高度(记得加上size的高度)
            if (state)
                height += (spKeys.arraySize + 1) * HEIGHT_ITEMS;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 绘制Header
            if (DrawHeader(new Rect(position.x, position.y, position.width, HEIGHT_HEADER), property.displayName, KEY_HEADER))
            {
                EditorGUI.indentLevel += 1;
                // 偏移Header的高度
                position.y += HEIGHT_HEADER;

                SerializedProperty spKeys = property.FindPropertyRelative("keys");
                SerializedProperty spValues = property.FindPropertyRelative("values");
                // 显示条目数量
                int newSize = EditorGUI.DelayedIntField(new Rect(position.x, position.y, position.width, HEIGHT_ITEMS), "Size:", spKeys.arraySize);
                if (newSize != spKeys.arraySize && newSize >= 0)
                {
                    if (newSize > spKeys.arraySize)
                    {
                        // 顺序追加
                        for (int i = spKeys.arraySize; i < newSize; i++)
                        {
                            spKeys.InsertArrayElementAtIndex(i);
                            spKeys.GetArrayElementAtIndex(i).stringValue = "Item" + i;
                            spValues.InsertArrayElementAtIndex(i);
                        }
                    }
                    else
                    {
                        // 倒序删减
                        for (int i = spKeys.arraySize - 1; i >= newSize; i--)
                        {
                            spKeys.DeleteArrayElementAtIndex(i);
                            spValues.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
                position.y += HEIGHT_ITEMS;


                // 由于当前Assembly是UnityEditor，所以需要手动导入UnityEngine
                Assembly ass = Assembly.Load("UnityEngine");
                Type valueType = ass.GetType(property.FindPropertyRelative("valueTypeName").stringValue, true, true);

                // 遍历显示元素集合
                for (int i = 0, len = spKeys.arraySize; i < len; i++)
                {
                    //Rect rect_title = new Rect(position.x, position.y + i * HEIGHT_ITEMS, position.width * WIDTH_KEY_PERCENT, HEIGHT_ITEMS - 1);
                    //EditorGUI.LabelField(rect_title, spKeys.GetArrayElementAtIndex(i).stringValue + ":");
                    //Rect rect_object = new Rect(position.x + position.width * WIDTH_KEY_PERCENT, position.y + i * HEIGHT_ITEMS, position.width * WIDTH_VALUE_PERCENT, HEIGHT_ITEMS - 1);
                    Rect rect_object = new Rect(position.x, position.y + i * HEIGHT_ITEMS, position.width, HEIGHT_ITEMS - 1);
                    //EditorGUI.ObjectField(rect_object, spValues.GetArrayElementAtIndex(i), valueType, new GUIContent(spKeys.GetArrayElementAtIndex(i).stringValue));
                    Object tmpValue = EditorGUI.ObjectField(rect_object, new GUIContent(spKeys.GetArrayElementAtIndex(i).stringValue), spValues.GetArrayElementAtIndex(i).objectReferenceValue, valueType, true);
                    if (tmpValue != spValues.GetArrayElementAtIndex(i).objectReferenceValue)
                    {
                        spValues.GetArrayElementAtIndex(i).objectReferenceValue = tmpValue;
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.EndProperty();
        }


        private static bool DrawHeader(Rect position, string title, string key)
        {
            EditorGUI.indentLevel -= 1;

            //Color ori_backgroundColor = GUI.backgroundColor;
            //Color org_contentColor = GUI.contentColor;

            bool state = EditorPrefs.GetBool(key, true);
            //if (!state)
            //    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUI.changed = false;
            if (state)
                title = "\u25BC" + (char)0x200a + title;
            else
                title = "\u25BA" + (char)0x200a + title;
            //GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            // "dragtab"
            if (!EditorGUI.Toggle(position, true, EditorStyles.miniLabel))
                state = !state;
            EditorGUI.LabelField(position, title);
            if (GUI.changed)
                EditorPrefs.SetBool(key, state);

            //GUI.contentColor = org_contentColor;
            //GUI.backgroundColor = ori_backgroundColor;

            EditorGUI.indentLevel += 1;

            return state;
        }
    }
}
