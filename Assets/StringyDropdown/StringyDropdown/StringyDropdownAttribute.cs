using UnityEngine;

namespace StringyDropdown
{
    public class StringyDropdownAttribute : PropertyAttribute
    {
        public bool UseDefaultFieldDrawer = false;
        public string FileName = "DefaultStringyStrings";
    }
}