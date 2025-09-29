
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    internal static class GridUIBuilder
    {
        public static RectTransform BuildRootPanel(Transform parent)
        {
            var panelGo = new GameObject("Grid (Overlay Panel)", typeof(RectTransform), typeof(Image));
            var panelRectTransform = (RectTransform)panelGo.transform;
            panelRectTransform.SetParent(parent, false);
            panelRectTransform.anchorMin = new Vector2(1, 1);
            panelRectTransform.anchorMax = new Vector2(1, 1);
            panelRectTransform.pivot = new Vector2(1, 1);
            panelRectTransform.anchoredPosition = new Vector2(-16, -16);
            panelRectTransform.sizeDelta = new Vector2(400, 400);

            if (panelRectTransform.TryGetComponent(out Image background))
            {
                background.color = new Color(0f, 0f, 0f, 0.5f);
            }

            return panelRectTransform;
        }

        public static GridLayoutGroup BuildGridContainer(Transform parent, int cellSize, Vector2 cellSpacing)
        {
            var gridGo = new GameObject("GridContainer", typeof(RectTransform), typeof(GridLayoutGroup));
            var gridRectTransform = (RectTransform)gridGo.transform;
            gridRectTransform.SetParent(parent, false);
            gridRectTransform.anchorMin = gridRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            gridRectTransform.pivot = new Vector2(0.5f, 0.5f);
            gridRectTransform.sizeDelta = new Vector2(cellSize * 20, cellSize * 12);

            var gridLayoutGroup = gridGo.GetComponent<GridLayoutGroup>();
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
            gridLayoutGroup.spacing = cellSpacing;
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;

            return gridLayoutGroup;
        }

        public static CellElements BuildCell(Transform parent, int index, int cellSize, Color backgroundColor, Color textColor)
        {
            var cellGo = new GameObject($"Cell ({index})", typeof(RectTransform), typeof(Image));
            var cellRectTransform = (RectTransform)cellGo.transform;
            cellRectTransform.SetParent(parent, false);

            var backgroundImage = cellGo.GetComponent<Image>();
            backgroundImage.color = backgroundColor;

            var textGo = new GameObject("Label (TMP)", typeof(RectTransform), typeof(TextMeshProUGUI));
            var textRectTransform = (RectTransform)textGo.transform;
            textRectTransform.SetParent(cellGo.transform, false);
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            var labelText = textGo.GetComponent<TextMeshProUGUI>();
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.fontSize = Mathf.RoundToInt(cellSize * 0.5f);
            labelText.color = textColor;

            return new CellElements { backgroundImage = backgroundImage, labelText = labelText };
        }

        internal struct CellElements
        {
            public Image backgroundImage;
            public TextMeshProUGUI labelText;
        }
    }
}