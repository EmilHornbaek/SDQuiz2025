using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Nullzone.Unity.UIElements
{
    public enum DataType
    {
        Float,      // absolute pixel width
        Percentage, // relative to parent width
        Int         // absolute pixel width, rounded
    }

    [UxmlElement, Preserve]
    public partial class AspectRatioElement : VisualElement
    {
        private float aspectRatioX = 1f;
        private float aspectRatioY = 1f;
        private float width = 100f;

        [UxmlAttribute]
        public DataType SizeType { get; set; } = DataType.Percentage;

        [UxmlAttribute("x")]
        public float AspectRatioX
        {
            get => aspectRatioX;
            set
            {
                aspectRatioX = Mathf.Max(0.01f, value);
                UpdateSize();
            }
        }

        [UxmlAttribute("y")]
        public float AspectRatioY
        {
            get => aspectRatioY;
            set
            {
                aspectRatioY = Mathf.Max(0.01f, value);
                UpdateSize();
            }
        }

        [UxmlAttribute("width")]
        public float Width
        {
            get => width;
            set
            {
                width = Mathf.Max(0, value);
                UpdateSize();
            }
        }

        public AspectRatioElement()
        {
            RegisterCallback<GeometryChangedEvent>(_ => UpdateSize());
        }

        private void UpdateSize()
        {
            if (parent == null || parent.resolvedStyle.width <= 0)
                return;

            float parentWidth = parent.resolvedStyle.width;
            float ratio = aspectRatioX / aspectRatioY;

            // Decide how to interpret Width based on SizeType
            float targetWidth = width;
            switch (SizeType)
            {
                case DataType.Percentage:
                    targetWidth = parentWidth * (width / 100f);
                    break;

                case DataType.Int:
                    targetWidth = Mathf.RoundToInt(width);
                    break;

                case DataType.Float:
                default:
                    // already in pixels
                    break;
            }

            float targetHeight = targetWidth / ratio;

            style.width = targetWidth;
            style.height = targetHeight;
        }
    }
}
