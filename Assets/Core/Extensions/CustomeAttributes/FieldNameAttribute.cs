using UnityEngine;

namespace Nullzone.Unity.Attributes
{
    public class FieldNameAttribute : PropertyAttribute
    {
        public string Label;

        public FieldNameAttribute(string label)
        {
            Label = label;
        }
    }
}