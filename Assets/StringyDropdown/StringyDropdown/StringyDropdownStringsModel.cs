using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringyDropdown
{
    [CreateAssetMenu(fileName = "DefaultStringyStrings", menuName = "StringyDropdown/New  StringyDropdown Strings file", order = 1)]
    public class StringyDropdownStringsModel : ScriptableObject
    {
        public string[] strings;

        private void OnValidate()
        {
            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = strings[i].Trim();
            }
        }
    }
}
