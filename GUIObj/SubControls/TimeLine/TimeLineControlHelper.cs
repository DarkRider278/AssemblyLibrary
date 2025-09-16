using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GUIObj.Enums;

namespace GUIObj.SubControls.TimeLine
{
    internal class TimeLineItemChangedEventArgs : EventArgs
    {
        public TimeLineManipulationMode Mode { get; set; }
        public TimeLineAction Action { get; set; }
        public TimeSpan DeltaTime { get; set; }
        public double DeltaX { get; set; }
    }

    internal class TimeLineDragAdorner : Adorner
    {
        private readonly ContentPresenter _adorningContentPresenter;
        //internal ITimeLineControlData Data { get; set; }
        //internal DataTemplate Template { get; set; }
        Point _mousePosition;
        public Point MousePosition
        {
            get
            {
                return _mousePosition;
            }
            set
            {
                if (_mousePosition != value)
                {
                    _mousePosition = value;
                    _layer.Update(AdornedElement);
                }

            }
        }
        readonly AdornerLayer _layer;

        public TimeLineDragAdorner(TimeLineControlItem uiElement, DataTemplate template)
            : base(uiElement)
        {
            _adorningContentPresenter = new ContentPresenter
            {
                Content = uiElement.DataContext,
                ContentTemplate = template,
                Opacity = 0.5
            };
            _layer = AdornerLayer.GetAdornerLayer(uiElement);
            if (_layer != null) _layer.Add(this);
            IsHitTestVisible = false;
        }

        public void Detach()
        {
            _layer.Remove(this);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adorningContentPresenter;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //_adorningContentPresenter.Measure(constraint);
            return new Size(((TimeLineControlItem)AdornedElement).Width, ((TimeLineControlItem)AdornedElement).DesiredSize.Height);//(_adorningContentPresenter.Width,_adorningContentPresenter.Height);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adorningContentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform) ?? throw new InvalidOperationException());
            result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));
            return result;
        }
    }

}