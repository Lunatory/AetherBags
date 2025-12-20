using System;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace AetherBags.Nodes
{
    public class HybridDirectionalStackNode<T> : LayoutListNode where T : NodeBase
    {
        public FlexGrowDirection GrowDirection
        {
            get;
            set
            {
                field = value;
                RecalculateLayout();
            }
        } = FlexGrowDirection.DownRight;

        public bool Vertical
        {
            get;
            set
            {
                field = value;
                RecalculateLayout();
            }
        } = true;

        public float Spacing
        {
            get;
            set
            {
                field = value;
                RecalculateLayout();
            }
        } = 1f;

        public bool StretchCrossAxis
        {
            get;
            set
            {
                field = value;
                RecalculateLayout();
            }
        } = true;

        protected override void InternalRecalculateLayout()
        {
            if (NodeList.Count == 0)
                return;

            bool alignRight = GrowDirection is FlexGrowDirection.DownLeft or FlexGrowDirection.UpLeft;
            bool alignBottom = GrowDirection is FlexGrowDirection.UpRight or FlexGrowDirection.UpLeft;

            float startX = alignRight ? Width : 0f;
            float startY = alignBottom ? Height : 0f;

            float cursor = 0f;

            for (int i = 0; i < NodeList.Count; i++)
            {
                var node = NodeList[i];

                if (StretchCrossAxis)
                {
                    if (Vertical)
                        node.Width = Width;
                    else
                        node.Height = Height;
                }

                float x, y;
                if (Vertical)
                {
                    x = alignRight ? startX - node.Width : startX;
                    y = alignBottom ? startY - node.Height - cursor : startY + cursor;
                    cursor += node.Height + Spacing;
                }
                else
                {
                    x = alignRight ? startX - node.Width - cursor : startX + cursor;
                    y = alignBottom ? startY - node.Height : startY;
                    cursor += node.Width + Spacing;
                }

                node.X = x;
                node.Y = y;
                AdjustNode(node);
            }
        }
    }
}