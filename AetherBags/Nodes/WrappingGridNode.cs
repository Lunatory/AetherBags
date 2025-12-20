using System;
using System.Collections.Generic;
using KamiToolKit;
using KamiToolKit. Nodes;

namespace AetherBags.Nodes
{
    public class WrappingGridNode<T> : LayoutListNode where T : NodeBase
    {
        public float HorizontalSpacing { get; set; } = 10;
        public float VerticalSpacing { get; set; } = 10;

        private List<List<NodeBase>> _rows = new();

        protected override void InternalRecalculateLayout()
        {
            if (NodeList. Count == 0)
                return;

            _rows.Clear();

            float availableWidth = Width;
            float currentX = 0f;
            float currentY = 0f;
            float rowHeight = 0f;
            List<NodeBase> currentRow = new();

            foreach (var node in NodeList)
            {
                float nodeWidth = node.Width;
                float nodeHeight = node.Height;

                if (currentX + nodeWidth > availableWidth && currentRow.Count > 0)
                {
                    _rows.Add(currentRow);
                    currentRow = new();
                    currentY += rowHeight + VerticalSpacing;
                    currentX = 0f;
                    rowHeight = 0f;
                }

                node.X = currentX;
                node. Y = currentY;
                AdjustNode(node);

                currentX += nodeWidth + HorizontalSpacing;
                rowHeight = Math.Max(rowHeight, nodeHeight);
                currentRow.Add(node);
            }

            if (currentRow.Count > 0)
            {
                _rows.Add(currentRow);
            }
        }

        public float GetRequiredHeight()
        {
            if (NodeList.Count == 0)
                return 0f;

            float maxY = 0f;
            foreach (var node in NodeList)
            {
                float nodeBottom = node.Y + node.Height;
                maxY = Math. Max(maxY, nodeBottom);
            }

            return maxY;
        }
    }
}