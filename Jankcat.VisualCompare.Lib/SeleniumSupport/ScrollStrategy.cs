using System;
using Jankcat.VisualCompare.Lib.Models;

namespace Jankcat.VisualCompare.Lib.SeleniumSupport
{
    public class ScrollStrategy
    {
        private Dimensions _dimensions;
        private int _indexX;
        private int _indexY;
        private Area _area;
        public ScrollStrategy(Dimensions dimensions)
        {
            _dimensions = dimensions;
            _indexX = 0;
            _indexY = 0;
            SetScrollArea(0, 0, _dimensions.PageWidth, dimensions.PageHeight);
        }

        public void SetScrollArea(int startX, int startY, int endX, int endY)
        {
            var documentWidth = _dimensions.PageWidth;
            var documentHeight = _dimensions.PageHeight;

            if (startX >= documentWidth)
            {
                throw new Exception("startX is out of range");
            }
            else if (startY >= documentHeight)
            {
                throw new Exception("startY is out of range");
            }
            else if (endX > documentWidth)
            {
                throw new Exception("endX is out of range");
            }
            else if (endY > documentHeight)
            {
                throw new Exception("endY is out of range");
            }

            _area = new Area
            {
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY
            };
        }

        public ScrollPosition GetScrollPosition() {
            var scrollPosition = new ScrollPosition();
            scrollPosition.X = _area.StartX + (_indexX * _dimensions.ViewportWidth);
            scrollPosition.Y = _area.StartY + (_indexY * _dimensions.ViewportHeight);
            scrollPosition.IndexX = _indexX;
            scrollPosition.IndexY = _indexY;
            return scrollPosition;
        }

        public CropDimension GetCropDimensions() {
            // Get Width
            var wantedWidth = _area.EndX - _area.StartX - _indexX * _dimensions.ViewportWidth;
            var width = wantedWidth > _dimensions.ViewportWidth ? _dimensions.ViewportWidth : wantedWidth;
            // Get Height
            var wantedHeight = _area.EndY - _area.StartY - _indexY * _dimensions.ViewportHeight;
            var height = wantedHeight > _dimensions.ViewportHeight ? _dimensions.ViewportHeight : wantedHeight;

            var crop = new CropDimension();
            crop.Width = width;
            crop.Height = height;

            return crop;
        }

        public bool HasNextScrollPosition()
        {
            return (HasNextVerticalScrollPosition() || HasNextHorizontalScrollPosition());
        }

        private bool HasNextHorizontalScrollPosition()
        {
            var width = _area.EndX - _area.StartX;
            return width > (_indexX * _dimensions.ViewportWidth + _dimensions.ViewportWidth);
        }

        private bool HasNextVerticalScrollPosition()
        {
            var height = _area.EndY - _area.StartY;
            return height > (_indexY * _dimensions.ViewportHeight + _dimensions.ViewportHeight);
        }

        public void MoveToNextScrollPosition()
        {
            if (HasNextHorizontalScrollPosition())
            {
                _indexX++;
            }
            else if (HasNextVerticalScrollPosition())
            {
                _indexX = 0;
                _indexY++;
            }
        }
    }
}
