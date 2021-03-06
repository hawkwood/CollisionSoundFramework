using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

// Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
// Altered by Brecht Lecluyse http://www.brechtos.com
// Further altered by Justin Hawkwood https://hawkwood.com

namespace StringyDropdown
{

    [CustomPropertyDrawer(typeof(StringyDropdownAttribute))]
    public class StringyDropdownPropertyDrawer : PropertyDrawer
    {
        private StringyDropdownStringsModel dropdownStrings = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                var attrib = this.attribute as StringyDropdownAttribute;

                if (attrib.UseDefaultFieldDrawer)
                {
                    property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                }
                else
                {
                    List<string> stringList = new List<string>();

                    stringList.Add("-"); // empty placeholder

                    if (dropdownStrings == null)
                    {
                        // Debug.Log("StringyDropdown: Reloading dropdownStrings");
                        do
                        {
                            string[] results = AssetDatabase.FindAssets(attrib.FileName);
                            if (results.Length == 0)
                            {
                                Debug.LogError("StringyDropdown: No asset found named " + attrib.FileName);
                                break;
                            }

                            if (results.Length > 1)
                            {
                                Debug.LogWarning("StringyDropdown: Multiple assets found named " + attrib.FileName);
                            }

                            foreach (var result in results)
                            {
                                string path = AssetDatabase.GUIDToAssetPath(result);

                                Object possDSObj = AssetDatabase.LoadAssetAtPath(path, typeof(StringyDropdownStringsModel));
                                if (possDSObj == null)
                                {
                                    // Debug.LogError("StringyDropdown: Found asset is not of type StringyDropdownStringsModel");
                                    continue;
                                }

                                dropdownStrings = possDSObj as StringyDropdownStringsModel;
                            }
                        } while (false);
                    }
                    else
                    {
                        // Debug.Log("StringyDropdown: dropdownStrings already loaded");
                    }

                    if (dropdownStrings != null && dropdownStrings.strings.Length > 0)
                    {
                        foreach (var str in dropdownStrings.strings)
                        {
                            if (str == null || str.Length == 0) continue;

                            stringList.Add(str);
                        }
                        // tagList.AddRange(stringyStrings.strings);
                    }
                    else
                    {
                        Debug.LogError("StringyDropdown: Failed to load or is empty, assets named " + attrib.FileName);
                    }

                    string propertyString = property.stringValue;
                    int index = -1;
                    if (propertyString == "")
                    {
                        //The tag is empty
                        index = 0; //first index is the special <notag> entry
                    }
                    else
                    {
                        //check if there is an entry that matches the entry and get the index
                        //we skip index 0 as that is a special custom case
                        for (int i = 1; i < stringList.Count; i++)
                        {
                            if (stringList[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    //Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, stringList.ToArray());

                    //Adjust the actual string value of the property based on the selection
                    if (index == 0)
                    {
                        property.stringValue = "";
                    }
                    else if (index >= 1)
                    {
                        property.stringValue = stringList[index];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
