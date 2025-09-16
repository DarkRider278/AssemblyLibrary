using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using GUIObj.Enums;
using GUIObj.Structs;

namespace GUIObj.SubControls.TimeLine
{
    internal class TimeLineControlLine : Canvas
    {
        public static TimeSpan CalculateMinimumAllowedTimeSpan(double unitSize)
        {
            //minute = unitsize*pixels
            //desired minimum widh for these manipulations = 10 pixels
            int minPixels = 10;
            double hours = minPixels / unitSize;
            //convert to milliseconds
            long ticks = (long)(hours * 60 * 60000 * 10000);
            return new TimeSpan(ticks);
        }

        private double _bumpThreshold = 1.5;
        private ScrollViewer _scrollViewer;
        private readonly Canvas _gridCanvas;
        static TimeLineDragAdorner _dragAdorner;

        static TimeLineDragAdorner DragAdorner
        {
            get { return _dragAdorner; }
            set
            {
                if (_dragAdorner != null)
                    _dragAdorner.Detach();
                _dragAdorner = value;
            }
        }

        private bool _synchedWithSiblings = true;

        public bool SynchedWithSiblings
        {
            get { return _synchedWithSiblings; }
            set { _synchedWithSiblings = value; }
        }

        internal bool m_IsSynchInstigator;
        internal double m_SynchWidth;

        bool _itemsInitialized;

        bool _unitSizeInitialized;
        bool _startDateInitialized;

        #region dependency properties

        /*public ITimeLineControlData FocusOnItem
        {
            get { return (ITimeLineControlData)GetValue(FocusOnItemProperty); }
            set { SetValue(FocusOnItemProperty, value); }
        }*/

        // Using a DependencyProperty as the backing store for FocusOnItem.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty FocusOnItemProperty = DependencyProperty.Register("FocusOnItem", typeof(ITimeLineControlData), typeof(TimeLineControl), new UIPropertyMetadata(null, new PropertyChangedCallback(FocusItemChanged)));
        public static void FocusItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((e.NewValue != null) && (d is TimeLineControlLine tc))
            {
                tc.ScrollToItem(e.NewValue as ITimeLineControlData);
            }

        }

        private void ScrollToItem(ITimeLineControlData target)
        {
            double tgtNewWidth = 0;
            double maxUnitSize = 450; //28000;
            double minUnitSize = 1;
            if (_scrollViewer != null)
            {
                for (int i = 1; i < Children.Count; i++)
                {
                    var ctrl = Children[i] as TimeLineControlItem;
                    if (ctrl != null && ctrl.DataContext == target)
                    {
                        double curW = ctrl.Width;
                        if (curW < 5)
                        {
                            tgtNewWidth = 50;
                        }
                        else if (curW > _scrollViewer.ViewportWidth)
                        {
                            tgtNewWidth = _scrollViewer.ViewportWidth / 3;
                        }

                        if (tgtNewWidth != 0)
                        {
                            double newUnitSize = (UnitSize * tgtNewWidth) / curW;
                            if (newUnitSize > maxUnitSize)
                                newUnitSize = maxUnitSize;
                            else if (newUnitSize < minUnitSize)
                                newUnitSize = minUnitSize;
                            UnitSize = newUnitSize;
                            SynchronizeSiblings();
                        }

                        ctrl.BringIntoView();
                        return;
                    }
                }
            }
        }

        #region manager

        /*public ITimeLineManager Manager
        {
            get { return (ITimeLineManager)GetValue(ManagerProperty); }
            set { SetValue(ManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Manager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register("Manager", typeof(ITimeLineManager), typeof(TimeLineControlLine),
            new UIPropertyMetadata(null));*/

        #endregion

        #region minwidth

        public new double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinWidth.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(TimeLineControlLine),
                new UIPropertyMetadata(0.0));

        #endregion

        #region minheight

        public new double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinHeight.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty MinHeightProperty = DependencyProperty.Register("MinHeight",
            typeof(double), typeof(TimeLineControlLine), new UIPropertyMetadata(0.0));

        #endregion

        #region background and grid dependency properties

        #region minimum unit width

        public double MinimumUnitWidth
        {
            get { return (double)GetValue(MinimumUnitWidthProperty); }
            set { SetValue(MinimumUnitWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumUnitWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumUnitWidthProperty =
            DependencyProperty.Register("MinimumUnitWidth", typeof(double), typeof(TimeLineControlLine),
                new UIPropertyMetadata(10.0, OnBackgroundValueChanged));

        #endregion

        #region snap to grid

        public bool SnapToGrid
        {
            get { return (bool)GetValue(SnapToGridProperty); }
            set { SetValue(SnapToGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SnapToGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SnapToGridProperty =
            DependencyProperty.Register("SnapToGrid", typeof(bool), typeof(TimeLineControlLine),
                new UIPropertyMetadata(null));

        //new UIPropertyMetadata(false,
        //new PropertyChangedCallback(OnBackgroundValueChanged)));

        #endregion

        #region draw time grid

        public bool DrawTimeGrid
        {
            get { return (bool)GetValue(DrawTimeGridProperty); }
            set { SetValue(DrawTimeGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawTimeGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawTimeGridProperty = DependencyProperty.Register("DrawTimeGrid",
            typeof(bool), typeof(TimeLineControlLine), new UIPropertyMetadata(false, OnDrawTimeGridChanged));

        public bool DrawTimeGridText
        {
            get { return (bool)GetValue(DrawTimeGridTextProperty); }
            set { SetValue(DrawTimeGridTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawTimeGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawTimeGridTextProperty =
            DependencyProperty.Register("DrawTimeGridText", typeof(bool), typeof(TimeLineControlLine),
                new UIPropertyMetadata(false, OnDrawTimeGridChanged));


        #endregion

        #region minor unit thickness

        public int MinorUnitThickness
        {
            get { return (int)GetValue(MinorUnitThicknessProperty); }
            set { SetValue(MinorUnitThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorUnitThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorUnitThicknessProperty =
            DependencyProperty.Register("MinorUnitThickness", typeof(int), typeof(TimeLineControlLine),
                new UIPropertyMetadata(1, OnBackgroundValueChanged));

        #endregion

        #region major unit thickness

        public int MajorUnitThickness
        {
            get { return (int)GetValue(MajorUnitThicknessProperty); }
            set { SetValue(MajorUnitThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorUnitThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorUnitThicknessProperty =
            DependencyProperty.Register("MajorUnitThickness", typeof(int), typeof(TimeLineControlLine),
                new UIPropertyMetadata(3, OnBackgroundValueChanged));

        #endregion

        private static byte _defC = 80;

        #region day line brush

        public Brush DayLineBrush
        {
            get { return (Brush)GetValue(DayLineBrushProperty); }
            set { SetValue(DayLineBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayLineBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayLineBrushProperty = DependencyProperty.Register("DayLineBrush",
            typeof(Brush), typeof(TimeLineControlLine),
            new UIPropertyMetadata(new SolidColorBrush(new Color() { R = _defC, G = _defC, B = _defC, A = 255 }),
                OnBackgroundValueChanged));

        #endregion

        #region hour line brush

        public Brush LineBrush1M
        {
            get { return (Brush)GetValue(LineBrush1MProperty); }
            set { SetValue(LineBrush1MProperty, value); }
        }


        // Using a DependencyProperty as the backing store for LineBrush1m.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineBrush1MProperty =
            DependencyProperty.Register("LineBrush1M", typeof(Brush), typeof(TimeLineControlLine),
                new UIPropertyMetadata(new SolidColorBrush(new Color() { R = 0, G = 35, B = 255, A = 255 }),
                    OnBackgroundValueChanged));

        public Brush LineBrush10M
        {
            get { return (Brush)GetValue(LineBrush10MProperty); }
            set { SetValue(LineBrush10MProperty, value); }
        }


        // Using a DependencyProperty as the backing store for LineBrush1m.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineBrush10MProperty =
            DependencyProperty.Register("LineBrush10M", typeof(Brush), typeof(TimeLineControlLine),
                new UIPropertyMetadata(new SolidColorBrush(new Color() { R = 255, G = 0, B = 0, A = 255 }),
                    OnBackgroundValueChanged));

        #endregion

        #region minute line brush

        public Brush LineBrush30S
        {
            get { return (Brush)GetValue(LineBrush30SProperty); }
            set { SetValue(LineBrush30SProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ineBrush1M.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineBrush30SProperty = DependencyProperty.Register("LineBrush30S",
            typeof(Brush), typeof(TimeLineControlLine),
            new UIPropertyMetadata(new SolidColorBrush(new Color() { R = 80, G = 80, B = 80, A = 255 }),
                OnBackgroundValueChanged));

        public Brush LineBrush15S
        {
            get { return (Brush)GetValue(LineBrush15SProperty); }
            set { SetValue(LineBrush15SProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ineBrush1M.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineBrush15SProperty = DependencyProperty.Register("LineBrush15S",
            typeof(Brush), typeof(TimeLineControlLine),
            new UIPropertyMetadata(new SolidColorBrush(new Color() { R = 92, G = 250, B = 74, A = 255 }),
                OnBackgroundValueChanged));

        #endregion

        private static void OnDrawTimeGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc.DrawBackGround(true);
            }
        }

        private static void OnBackgroundValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc.DrawBackGround();
            }
        }

        #endregion

        #region item template

        private DataTemplate _template;

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate",
            typeof(DataTemplate), typeof(TimeLineControlLine), new UIPropertyMetadata(null, OnItemTemplateChanged));

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc.SetTemplate(e.NewValue as DataTemplate);
            }
        }

        #endregion

        #region Items

        public ObservableCollection<ITimeLineControlData> Items
        {
            get { return (ObservableCollection<ITimeLineControlData>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items",
            typeof(ObservableCollection<ITimeLineControlData>), typeof(TimeLineControlLine),
            new UIPropertyMetadata(null, OnItemsChanged));

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc.InitializeItems(e.NewValue as ObservableCollection<ITimeLineControlData>);
                tc.UpdateUnitSize(tc.UnitSize);
                tc._itemsInitialized = true;
                tc.DrawBackGround();
            }
        }

        #endregion

        #region ViewLevel

        public TimeLineViewLevel ViewLevel
        {
            get { return (TimeLineViewLevel)GetValue(ViewLevelProperty); }
            set { SetValue(ViewLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewLevelProperty = DependencyProperty.Register("ViewLevel",
            typeof(TimeLineViewLevel), typeof(TimeLineControlLine),
            new UIPropertyMetadata(TimeLineViewLevel.Minutes, OnViewLevelChanged));

        private static void OnViewLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc.UpdateViewLevel((TimeLineViewLevel)e.NewValue);

            }

        }

        #endregion

        #region unitsize


        public double UnitSize
        {
            get { return (double)GetValue(UnitSizeProperty); }
            set { SetValue(UnitSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitSizeProperty =
            DependencyProperty.Register("UnitSize", typeof(double), typeof(TimeLineControlLine),
                new UIPropertyMetadata(50.0,
                    OnUnitSizeChanged));

        private static void OnUnitSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc._unitSizeInitialized = true;
                tc.UpdateUnitSize((double)e.NewValue);

            }
        }

        #endregion

        #region start date

        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(TimeLineControlLine),
                new UIPropertyMetadata(DateTime.MinValue, OnStartDateChanged));

        public DateTime RunTime
        {
            get { return (DateTime)GetValue(RunTimeProperty); }
            set { SetValue(RunTimeProperty, value); }
        }

        public static readonly DependencyProperty RunTimeProperty =
            DependencyProperty.Register("RunTime", typeof(DateTime), typeof(TimeLineControlLine),
                new UIPropertyMetadata(DateTime.MinValue, OnRunTimeChange));

        private static void OnStartDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc._startDateInitialized = true;
                tc.ReDrawChildren();
            }
        }

        private static void OnRunTimeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControlLine tc = d as TimeLineControlLine;
            if (tc != null)
            {
                tc.UpdateRunMarker();
            }
        }

        private void UpdateRunMarker()
        {
            if (_gridCanvas.Children.Count >= 2)
            {
                Rectangle marker = _gridCanvas.Children[1] as Rectangle;

                if (marker != null)
                {
                    double y;
                    TimeSpan dur = RunTime.Subtract(StartDate);
                    if (RunTime == DateTime.MinValue)
                        y = -10;
                    else
                        y= ConvertTimeToDistance(dur);

                    Canvas.SetLeft(marker, y);
                }
            }
        }

        private Double ConvertTimeToDistance(TimeSpan span)
        {

            TimeLineViewLevel lvl = (TimeLineViewLevel)GetValue(ViewLevelProperty);
            Double unitSize = (Double)GetValue(UnitSizeProperty);
            Double value = unitSize;
            switch (lvl)
            {
                case TimeLineViewLevel.Minutes:
                    value = span.TotalMinutes * unitSize;
                    break;
                case TimeLineViewLevel.Hours:
                    value = span.TotalHours * unitSize;
                    break;
                case TimeLineViewLevel.Days:
                    value = span.TotalDays * unitSize;
                    break;
                case TimeLineViewLevel.Weeks:
                    value = (span.TotalDays / 7.0) * unitSize;
                    break;
                case TimeLineViewLevel.Months:
                    value = (span.TotalDays / 30.0) * unitSize;
                    break;
                case TimeLineViewLevel.Years:
                    value = (span.TotalDays / 365.0) * unitSize;
                    break;
                default:
                    break;
            }
            return value;
        }

        #endregion

        #region manipulation mode

        public TimeLineManipulationMode ManipulationMode
        {
            get { return (TimeLineManipulationMode)GetValue(ManipulationModeProperty); }
            set { SetValue(ManipulationModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManipulationMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManipulationModeProperty =
            DependencyProperty.Register("ManipulationMode", typeof(TimeLineManipulationMode),
                typeof(TimeLineControlLine), new UIPropertyMetadata(TimeLineManipulationMode.Free));

        #endregion

        #endregion

        public TimeLineControlLine()
        {
            _gridCanvas = new Canvas();
            Children.Add(_gridCanvas);
            Focusable = true;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            MouseEnter += TimeLineControl_MouseEnter;
            MouseLeave += TimeLineControl_MouseLeave;
            //Items = new ObservableCollection<ITimeLineControlData>();

            DragDrop.AddDragOverHandler(this, TimeLineControl_DragOver);
            DragDrop.AddDropHandler(this, TimeLineControl_Drop);
            DragDrop.AddDragEnterHandler(this, TimeLineControl_DragOver);
            DragDrop.AddDragLeaveHandler(this, TimeLineControL_DragLeave);

            AllowDrop = true;
            _scrollViewer = GetParentScrollViewer();
        }

        #region control life cycle events

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            _scrollViewer = GetParentScrollViewer();

        }

        /*
        /// <summary>
        /// I was unable to track down why this control was locking up when
        /// synchronise with siblings is checked and the parent element is closed etc.
        /// I was getting something with a contextswitchdeadblock that I was wracking my
        /// brain trying to figure out.  The problem only happened when a timeline control
        /// with a child timeline item was present.  I could have n empty timeline controls
        /// with no problem.  Adding one timeline item however caused that error when the parent element
        /// is closed etc.
        /// </summary>
        /// <param name="child"></param>
        protected override void ParentLayoutInvalidated(UIElement child)
        {
            //this event fires when something drags over this or when the control is trying to close
            if (child == _tmpDraggAdornerControl)
                return;
            if (!Children.Contains(child))
                return;
            base.ParentLayoutInvalidated(child);
            SynchedWithSiblings = false;
            //Because this layout invalidated became neccessary, I had to then put null checks on all attempts
            //to get a timeline item control.  There appears to be some UI threading going on so that just checking the children count
            //at the begining of the offending methods was not preventing me from crashing.  
            Children.Clear();
        }*/

        #endregion

        #region miscellaneous helpers

        private ScrollViewer GetParentScrollViewer()
        {
            DependencyObject item = VisualTreeHelper.GetParent(this);
            while (item != null)
            {
                if (item is ScrollViewer viewer)
                {
                    return viewer;
                }

                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        private void SetTemplate(DataTemplate dataTemplate)
        {
            _template = dataTemplate;
            for (int i = 0; i < Children.Count; i++)
            {
                TimeLineControlItem titem = Children[i] as TimeLineControlItem;
                if (titem != null)
                    titem.ContentTemplate = dataTemplate;
            }
        }

        private void InitializeItems(ObservableCollection<ITimeLineControlData> observableCollection)
        {
            if (observableCollection == null)
                return;
            this.Children.Clear();
            Children.Add(_gridCanvas);

            foreach (ITimeLineControlData data in observableCollection)
            {
                TimeLineControlItem adder = CreateTimeLineItemControl(data);

                Children.Add(adder);
            }

            Items.CollectionChanged -= Items_CollectionChanged;
            Items.CollectionChanged += Items_CollectionChanged;
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems[0] is ITimeLineControlData itm && itm.StartTime.HasValue &&
                    itm.RunId == 0) //  itm.StartTime.Value == DateTime.MinValue)
                {
                    //newly created item isn't a drop in so we need to instantiate and place its control.
                    TimeSpan duration = itm.EndTime.Value.Subtract(itm.StartTime.Value);
                    if (Items.Count == 1) //this is the first one added
                    {
                        itm.StartTime = StartDate;
                        itm.EndTime = StartDate.Add(duration);
                    }
                    else
                    {
                        var last = Items.OrderBy(i => i.StartTime.Value).LastOrDefault();
                        if (last != null)
                        {
                            //itm.StartTime = last.EndTime;
                            //itm.EndTime = itm.StartTime.Value.Add(duration);
                        }
                    }

                    itm.InitRunID();
                    var ctrl = CreateTimeLineItemControl(itm);
                    //The index if Items.Count-1 because of zero indexing.
                    //however our children is 1 indexed because 0 is our canvas grid.
                    Children.Insert(Items.Count, ctrl);
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var removeItem = e.OldItems[0];
                for (int i = 1; i < Children.Count; i++)
                {
                    TimeLineControlItem checker = Children[i] as TimeLineControlItem;
                    if (checker != null && checker.DataContext == removeItem)
                    {
                        Children.Remove(checker);
                        break;
                    }
                }
            }
        }

        private TimeLineControlItem CreateTimeLineItemControl(ITimeLineControlData data)
        {
            Binding startBinding = new Binding("StartTime");
            startBinding.Mode = BindingMode.TwoWay;
            startBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Binding endBinding = new Binding("EndTime");
            endBinding.Mode = BindingMode.TwoWay;
            endBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Binding durBinding = new Binding("DurationTime");
            durBinding.Mode = BindingMode.TwoWay;
            durBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Binding bgBinding = new Binding("BGColor");
            bgBinding.Mode = BindingMode.OneWay;
            bgBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;


            DateTime timelineStart = StartDate;

            Binding expandedBinding = new Binding("TimelineViewExpanded");
            expandedBinding.Mode = BindingMode.TwoWay;
            endBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            TimeLineControlItem adder = new TimeLineControlItem();
            adder.TimeLineStartTime = timelineStart;
            adder.DataContext = data;
            adder.Content = data;

            adder.SetBinding(TimeLineControlItem.StartTimeProperty, startBinding);
            adder.SetBinding(TimeLineControlItem.EndTimeProperty, endBinding);
            adder.SetBinding(TimeLineControlItem.DurationTimeProperty, durBinding);
            adder.SetBinding(TimeLineControlItem.IsExpandedProperty, expandedBinding);
            adder.SetBinding(TimeLineControlItem.BGColorProperty, bgBinding);

            if (_template != null)
            {
                adder.ContentTemplate = _template;
            }

            /*adder.PreviewMouseLeftButtonDown += item_PreviewEditButtonDown;
            adder.MouseMove += item_MouseMove;
            adder.PreviewMouseLeftButtonUp += item_PreviewEditButtonUp;*/
            adder.PreviewMouseRightButtonDown += item_PreviewEditButtonDown;
            adder.MouseMove += item_MouseMove;
            adder.PreviewMouseRightButtonUp += item_PreviewEditButtonUp;

            adder.PreviewMouseLeftButtonUp += item_PreviewDragButtonUp;
            adder.PreviewMouseLeftButtonDown += item_PreviewDragButtonDown;
            adder.UnitSize = UnitSize;
            adder.KeyUp += item_KeyUp;
            return adder;
        }

        

        #endregion

        #region updaters fired on dp changes

        private void UpdateUnitSize(double size)
        {
            if (Items == null)
                return;
            for (int i = 0; i < Items.Count; i++)
            {
                TimeLineControlItem titem = GetTimeLineItemControlAt(i);
                if (titem != null)
                    titem.UnitSize = size;
            }

            ReDrawChildren();
        }

        private void UpdateViewLevel(TimeLineViewLevel lvl)
        {
            if (Items == null)
                return;
            for (int i = 0; i < Items.Count; i++)
            {

                var templatedControl = GetTimeLineItemControlAt(i);
                if (templatedControl != null)
                    templatedControl.ViewLevel = lvl;

            }

            ReDrawChildren();
            //Now we go back and have to detect if things have been collapsed
        }

        //TODO: set up the timeline start date dependency property and do this margin check
        //for all including the first one.
        private void ReDrawChildren()
        {
            if (Items == null)
            {
                DrawBackGround();
                return;
            }

            DateTime start = (DateTime)GetValue(StartDateProperty);
            double w = 0;
            double s = 0;
            double e = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                var mover = GetTimeLineItemControlAt(i);
                if (mover != null)
                {
                    mover.TimeLineStartTime = start;
                    if (!mover.ReadyToDraw)
                        mover.ReadyToDraw = true;
                    mover.PlaceOnCanvas();
                    mover.GetPlacementInfo(ref s, ref w, ref e);
                }

            }

            //find our background rectangle and set its width;
            DrawBackGround();
        }

        #endregion

        #region background and grid methods

        private void DrawBackGround(bool isDrawGridUpdate = false)
        {
            Brush b = Background;
            double setWidth;
            if (_gridCanvas.Children.Count <= 0)
            {
                _gridCanvas.Children.Add(new Rectangle());
                _gridCanvas.Children.Add(new Rectangle());
            }

            Rectangle bg = _gridCanvas.Children[0] as Rectangle;
            Rectangle marker= _gridCanvas.Children[1] as Rectangle;
            if (!_startDateInitialized || !_unitSizeInitialized || !_itemsInitialized || Items == null)
            {

                setWidth = Math.Max(MinWidth, GetTimeLineWidth());
                setWidth = Math.Max(setWidth, m_SynchWidth);
                if (bg != null)
                {
                    bg.Width = setWidth;
                    bg.Height = Math.Max(DesiredSize.Height, Height);
                    if (double.IsNaN(bg.Height) || bg.Height < MinHeight)
                    {
                        bg.Height = MinHeight;
                    }

                    bg.Fill = b;
                    Width = bg.Width;
                    Height = bg.Height;
                }

                if (marker != null)
                {
                    marker.Width = 5;
                    marker.Height = Math.Max(DesiredSize.Height, Height);
                    if (DrawTimeGridText)
                    {
                        marker.Height -= 15;
                        Canvas.SetTop(marker,15);
                    }
                    if (double.IsNaN(marker.Height) || marker.Height < MinHeight)
                    {
                        marker.Height = MinHeight;
                    }
                    

                    marker.Fill = new SolidColorBrush(Colors.Red);
                    Canvas.SetLeft(marker,-10);
                }
            }
            else
            {
                var oldW = Width;
                var oldDrawTimeGrid = DrawTimeGrid;
                if (isDrawGridUpdate)
                    oldDrawTimeGrid = !oldDrawTimeGrid;
                //this is run every time we may need to update our siblings.
                SynchronizeSiblings();



                if (Items == null)
                    return;
                setWidth = Math.Max(MinWidth, GetTimeLineWidth());
                setWidth = Math.Max(setWidth, m_SynchWidth);
                if (bg != null)
                {
                    bg.Width = setWidth;
                    bg.Height = Math.Max(DesiredSize.Height, Height);
                    if (double.IsNaN(bg.Height) || bg.Height < MinHeight)
                    {
                        bg.Height = MinHeight;
                    }

                    bg.Fill = b;
                    Width = bg.Width;
                    Height = bg.Height;
                }

                /*if (marker != null)
                {
                    marker.Width = 5;
                    marker.Height = Math.Max(DesiredSize.Height, Height);
                    if (DrawTimeGridText)
                    {
                        marker.Height -= 10;
                        Canvas.SetTop(marker, 10);
                    }
                    if (double.IsNaN(marker.Height) || marker.Height < MinHeight)
                    {
                        marker.Height = MinHeight;
                    }

                    marker.Fill = new SolidColorBrush(Colors.Red);
                }*/

                if (DrawTimeGrid)
                {
                    if (Width != oldW || !oldDrawTimeGrid || (Width == MinWidth))
                        DrawTimeGridExecute();
                }
                else
                {
                    ClearTimeGridExecute();
                }

                if ((oldW != Width) && (_scrollViewer != null)) //if we are at min width then we need to redraw our time grid when unit sizes change
                {
                    var available = LayoutInformation.GetLayoutSlot(_scrollViewer);
                    Size s = new Size(available.Width, available.Height);
                    _scrollViewer.Measure(s);
                    _scrollViewer.Arrange(available);
                }
            }
        }

        internal double GetTimeLineWidth()
        {
            if (Items == null)
            {
                return MinWidth;
            }

            var lastItem = GetTimeLineItemControlAt(Items.Count - 1);

            if (lastItem == null)
                return MinWidth;
            double l = 0;
            double w = 0;
            double e = 0;
            lastItem.GetPlacementInfo(ref l, ref w, ref e);
            return Math.Max(MinWidth, e);
        }

        private void SynchronizeSiblings()
        {
            if (!SynchedWithSiblings)
                return;
            var current = VisualTreeHelper.GetParent(this) as FrameworkElement;

            while (current != null && !(current is ItemsControl))
            {
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
            }

            if (current != null)
            {
                //var pnl = current as ItemsControl;
                //this is called on updates for all siblings so it could easily
                //end up infinitely looping if each time tried to synch its siblings
                bool isSynchInProgress = false;
                //is there a synch instigator
                double maxWidth = GetTimeLineWidth();

                List<TimeLineControlLine> timeLineControls = FindAllTimeLineControlsInsidePanel(current).ToList();
                foreach (TimeLineControlLine tcSib in timeLineControls)
                {
                    if (tcSib.m_IsSynchInstigator)
                        isSynchInProgress = true;
                    double sibW = tcSib.GetTimeLineWidth();
                    if (sibW > maxWidth)
                    {
                        maxWidth = sibW;
                    }
                }

                m_SynchWidth = maxWidth;
                if (!isSynchInProgress)
                {
                    m_IsSynchInstigator = true;
                    foreach (TimeLineControlLine tcSib in timeLineControls)
                    {
                        if (tcSib != null && tcSib != this)
                        {
                            tcSib.m_SynchWidth = maxWidth;
                            //tcSib.UnitSize = UnitSize;
                            //tcSib.StartDate = StartDate;
                            tcSib.DrawBackGround();
                        }
                    }
                }

                m_IsSynchInstigator = false;
            }
        }

        //helper to let a panel find all children of a given type
        private static IEnumerable<TimeLineControlLine> FindAllTimeLineControlsInsidePanel(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is TimeLineControlLine control)
                    {
                        yield return control;
                    }

                    foreach (TimeLineControlLine childOfChild in FindAllTimeLineControlsInsidePanel(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void ClearTimeGridExecute()
        {
            if (_gridCanvas.Children.Count == 3)
                _gridCanvas.Children.RemoveAt(2);
        }

        private void DrawTimeGridExecute()
        {
            if (Items == null)
                return;
            if (StartDate == DateTime.MinValue)
                return;
            if (_gridCanvas.Children.Count < 3)
            {
                _gridCanvas.Children.Add(new Canvas());
            }

            if (_gridCanvas.Children[2] is Canvas grid)
            {
                grid.Children.Clear();

                //place our gridlines
                //DrawDayLines(grid);

                bool dt = DrawSecLines(grid);
                DrawMinLines(grid, dt);
            }
        }

        private bool DrawSecLines(Canvas grid)
        {
            double rater30S = UnitSize / 2;
            double raster15S = UnitSize / 4;
            bool drawTime = DrawTimeGridText;
            if (raster15S >= MinimumUnitWidth)
            {
                TimeSpan gap15S = new TimeSpan(0, 0, 15);
                DateTime next15S = StartDate.Add(gap15S);
                double dist15S = gap15S.TotalMinutes * UnitSize;
                DrawIncrementLines(grid, next15S, dist15S, new TimeSpan(0, 0, 15), raster15S, LineBrush15S, drawTime,
                    0);
                drawTime = false;
            }
            else if (rater30S >= MinimumUnitWidth)
            {
                TimeSpan gap30S = new TimeSpan(0, 0, 30);
                DateTime next30S = StartDate.Add(gap30S);
                double dist30S = gap30S.TotalMinutes * UnitSize;
                DrawIncrementLines(grid, next30S, dist30S, new TimeSpan(0, 0, 30), rater30S, LineBrush30S, drawTime, 0);
                drawTime = false;
            }

            return drawTime;
        }

        private bool DrawMinLines(Canvas grid, bool drawTime)
        {
            double raster1M = UnitSize;
            double raster10M = raster1M * 10;

            if (raster1M >= MinimumUnitWidth)
            {
                TimeSpan gab1M = new TimeSpan(0, 0, 0);
                DateTime next1M = StartDate.Add(gab1M);
                double dist1M = gab1M.TotalMinutes * raster1M;
                DrawIncrementLines(grid, next1M, dist1M, new TimeSpan(0, 01, 0), raster1M, LineBrush1M, drawTime, 10,
                    10);
                drawTime = false;
            }
            else if (raster10M >= MinimumUnitWidth)
            {
                TimeSpan gap10M = new TimeSpan(0, 10, 0);
                DateTime next10M = StartDate.Add(gap10M);
                double dist10M = gap10M.TotalMinutes * raster1M;
                DrawIncrementLines(grid, next10M, dist10M, new TimeSpan(0, 10, 0), raster10M, LineBrush10M, drawTime,
                    1);
                drawTime = false;
            }

            return drawTime;
        }

        private void DrawDayLines(Canvas grid)
        {
            double daySize = UnitSize * 24;


            if (daySize >= MinimumUnitWidth)
            {
                TimeSpan increment = new TimeSpan(24, 0, 0);
                int startHour = StartDate.Hour;
                int startMinute = StartDate.Minute;
                int remainingHours = 24 - startHour;
                if (startMinute > 0)
                    remainingHours--;
                int remainingMinutes = 60 - startMinute;
                if (startMinute == 0)
                    remainingMinutes = 0;
                int startSecond = StartDate.Second;
                int remainingSeconds = 60 - startSecond;
                if (startSecond != 0)
                    remainingMinutes--;
                else
                    remainingSeconds = 0;



                TimeSpan firstDayGap = new TimeSpan(remainingHours, remainingMinutes, remainingSeconds);
                double firstDayDistance = (firstDayGap.TotalHours * UnitSize);
                DateTime nextDay = StartDate.Add(new TimeSpan(remainingHours, remainingMinutes, 0));


                DrawIncrementLines(grid, nextDay, firstDayDistance,
                    increment, daySize, DayLineBrush, true, 7);
            }

        }

        private void DrawIncrementLines(Canvas grid, DateTime firstLineDate, double firstLineDistance,
            TimeSpan timeStep, double unitSize, Brush brush, bool printtime, int majorEvery, int majorEveryOffset = 0)
        {
            double curX = firstLineDistance;
            DateTime curDate = firstLineDate;
            TimeSpan displaytime = timeStep;
            int curLine = 0;
            while (curX < Width)
            {
                Line l = new Line();
                l.ToolTip = curDate.ToShortTimeString();
                l.StrokeThickness = MinorUnitThickness;
                if ((majorEvery > 0) && ((curLine - majorEveryOffset) % majorEvery == 0))
                {
                    l.StrokeThickness = MajorUnitThickness;
                }

                l.Stroke = brush;
                l.X1 = 0;
                l.X2 = 0;
                l.Y1 = 0;
                l.Y2 = Math.Max(DesiredSize.Height, Height);
                grid.Children.Add(l);
                Canvas.SetLeft(l, curX);
                if (printtime)
                {
                    TextBlock tb = new TextBlock();
                    if (timeStep.Minutes > 0)
                    {
                        tb.Text = displaytime.TotalMinutes.ToString(" ## m");
                    }
                    else
                    {
                        tb.Text = displaytime.TotalSeconds.ToString(" ## s");
                    }

                    tb.Foreground = new SolidColorBrush(Colors.Black);
                    grid.Children.Add(tb);
                    Canvas.SetLeft(tb, curX);
                }

                curX += unitSize;
                curDate += timeStep;
                displaytime += timeStep;
                curLine++;
            }
        }

        #endregion

        #region mouse enter and leave events

        void TimeLineControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //Keyboard.Focus(this);
        }

        void TimeLineControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //Keyboard.Focus(this);
        }

        #endregion

        #region drag events and fields

        //private bool _dragging;
        private Point _dragStartPosition = new Point(double.MinValue, double.MinValue);

        /// <summary>
        /// When we drag something from an external control over this I need a temp control
        /// that lets me adorn those accordingly as well
        /// </summary>
        private TimeLineControlItem _tmpDraggAdornerControl;

        //TimeLineItemControl _dragObject;
        void item_PreviewDragButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPosition = Mouse.GetPosition(null);
            //_dragObject = sender as TimeLineItemControl;

        }

        void item_PreviewDragButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragStartPosition.X = double.MinValue;
            _dragStartPosition.Y = double.MinValue;
            //_dragObject = null;
        }


        void TimeLineControl_DragOver(object sender, DragEventArgs e)
        {
            TimeLineControlItem d = e.Data.GetData(typeof(TimeLineControlItem)) as TimeLineControlItem;
            if (d != null)
            {
                /*if (Manager != null)
                {
                    if (!Manager.CanAddToTimeLine(d.DataContext as ITimeLineControlData))
                    {
                        e.Effects = DragDropEffects.None;
                        return;
                    }
                }*/
                e.Effects = DragDropEffects.Move;
                //this is an internal drag or a drag from another time line control
                if (DragAdorner == null)
                {
                    _dragAdorner = new TimeLineDragAdorner(d, ItemTemplate);

                }

                if (DragAdorner != null)
                {
                    DragAdorner.MousePosition = e.GetPosition(d);
                    DragAdorner.InvalidateVisual();
                }
            }
            else
            {
                //GongSolutions.Wpf.DragDrop

                var d2 = e.Data.GetData("GongSolutions.Wpf.DragDrop");
                if (d2 != null)
                {
                    /*if (Manager != null)
                    {
                        if (!Manager.CanAddToTimeLine(d2 as ITimeLineControlData))
                        {
                            e.Effects = DragDropEffects.None;
                            return;
                        }
                    }*/

                    e.Effects = DragDropEffects.Move;
                    if (DragAdorner == null)
                    {
                        //we are dragging from an external source and we don't have a timeline item control of any sort
                        Children.Remove(_tmpDraggAdornerControl);
                        //in order to get an adornment layer the control has to be somewhere
                        _tmpDraggAdornerControl = new TimeLineControlItem();
                        _tmpDraggAdornerControl.UnitSize = UnitSize;
                        Children.Add(_tmpDraggAdornerControl);
                        Canvas.SetLeft(_tmpDraggAdornerControl, -1000000);
                        _tmpDraggAdornerControl.DataContext = d2;
                        _tmpDraggAdornerControl.StartTime = StartDate;
                        _tmpDraggAdornerControl.InitializeDefaultLength();
                        _tmpDraggAdornerControl.ContentTemplate = ItemTemplate;

                        _dragAdorner = new TimeLineDragAdorner(_tmpDraggAdornerControl, ItemTemplate);
                    }

                    if (DragAdorner != null)
                    {
                        DragAdorner.MousePosition = e.GetPosition(_tmpDraggAdornerControl);
                        DragAdorner.InvalidateVisual();
                    }
                }
            }

            DragScroll(e);


        }

        void TimeLineControL_DragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            Children.Remove(_tmpDraggAdornerControl);
            _tmpDraggAdornerControl = null;
        }

        void TimeLineControl_Drop(object sender, DragEventArgs e)
        {
            DragAdorner = null;

            TimeLineControlItem dropper = e.Data.GetData(typeof(TimeLineControlItem)) as TimeLineControlItem;
            ITimeLineControlData dropData = null;
            if (dropper == null)
            {
                //dropData = e.Data.GetData(typeof(ITimeLineControlData)) as ITimeLineControlData;
                dropData = e.Data.GetData("GongSolutions.Wpf.DragDrop") as ITimeLineControlData;
                if (dropData != null)
                {
                    //I haven't figured out why but
                    //sometimes when dropping from an external source
                    //the drop event hits twice.
                    //that results in ugly duplicates ending up in the timeline
                    //and it is a mess.
                    if (Items.Contains(dropData))
                        return;
                    //create a new timeline item control from this data
                    dropper = CreateTimeLineItemControl(dropData);
                    dropper.StartTime = StartDate;
                    dropper.InitializeDefaultLength();
                    Children.Remove(_tmpDraggAdornerControl);
                    _tmpDraggAdornerControl = null;

                }
            }

            var dropX = e.GetPosition(this).X;
            int newIndex = GetDroppedNewIndex(dropX);
            if (dropper != null)
            {
                var curData = dropper.DataContext as ITimeLineControlData;
                var curIndex = Items.IndexOf(curData);
                if ((curIndex == newIndex || curIndex + 1 == newIndex) && dropData == null &&
                    dropper.Parent ==
                    this) //dropdata null is to make sure we aren't failing on adding a new data item into the timeline
                    //dropper.parent==this makes it so that we allow a dropper control from another timeline to be inserted in at the start.
                {
                    return; //our drag did nothing meaningful so we do nothing.
                }

                if (newIndex == 0)
                {
                    if (dropData == null)
                    {
                        RemoveTimeLineItemControl(dropper);
                    }

                    if (dropper.Parent != this && dropper.Parent is TimeLineControlLine)
                    {
                        if (dropper.Parent is TimeLineControlLine tlCtrl) tlCtrl.RemoveTimeLineItemControl(dropper);
                    }

                    InsertTimeLineItemControlAt(newIndex, dropper);
                    //dropper.MoveToNewStartTime(start);
                    MakeRoom(newIndex, dropper.Width);
                }
                else //we are moving this after something.
                {
                    //find out if we are moving the existing one back or forward.
                    var placeAfter = GetTimeLineItemControlAt(newIndex - 1);
                    if (placeAfter != null)
                    {
                        var start = placeAfter.EndTime;
                        RemoveTimeLineItemControl(dropper);
                        if (curIndex < newIndex &&
                            curIndex >=
                            0) //-1 is on an insert in which case we definitely don't want to take off on our new index value
                        {
                            //we are moving forward.
                            newIndex--; //when we removed our item, we shifted our insert index back 1
                        }

                        if (dropper.Parent != null && dropper.Parent != this)
                        {
                            var ptl = dropper.Parent as TimeLineControlLine;
                            if (ptl != null) ptl.RemoveTimeLineItemControl(dropper);
                        }

                        InsertTimeLineItemControlAt(newIndex, dropper);
                        dropper.MoveToNewStartTime(dropper.StartTime > start ? dropper.StartTime : start);
                        MakeRoom(newIndex, dropper.Width);
                    }
                }
            }

            //ReDrawChildren();
            DrawBackGround();
            e.Handled = true;
        }

        #region drop helpers

        private void InsertTimeLineItemControlAt(int index, TimeLineControlItem adder)
        {
            var data = adder.DataContext as ITimeLineControlData;
            if (Items.Contains(data))
                return;

            adder.PreviewMouseRightButtonDown -= item_PreviewEditButtonDown;
            adder.MouseMove -= item_MouseMove;
            adder.PreviewMouseRightButtonUp -= item_PreviewEditButtonUp;

            adder.PreviewMouseLeftButtonUp -= item_PreviewDragButtonUp;
            adder.PreviewMouseLeftButtonDown -= item_PreviewDragButtonDown;

            adder.PreviewMouseRightButtonDown += item_PreviewEditButtonDown;
            adder.MouseMove += item_MouseMove;
            adder.PreviewMouseRightButtonUp += item_PreviewEditButtonUp;

            adder.PreviewMouseLeftButtonUp += item_PreviewDragButtonUp;
            adder.PreviewMouseLeftButtonDown += item_PreviewDragButtonDown;
            //child 0 is our grid and we want to keep that there.
            Children.Insert(index + 1, adder);
            Items.Insert(index, data);
        }

        private void RemoveTimeLineItemControl(TimeLineControlItem remover)
        {
            var curData = remover.DataContext as ITimeLineControlData;
            remover.PreviewMouseRightButtonDown -= item_PreviewEditButtonDown;
            remover.MouseMove -= item_MouseMove;
            remover.PreviewMouseRightButtonUp -= item_PreviewEditButtonUp;

            remover.PreviewMouseLeftButtonUp -= item_PreviewDragButtonUp;
            remover.PreviewMouseLeftButtonDown -= item_PreviewDragButtonDown;
            Items.Remove(curData);
            Children.Remove(remover);
        }

        private int GetDroppedNewIndex(double dropX)
        {
            double s = 0;
            double w = 0;
            double e = 0;
            for (int i = 0; i < Items.Count(); i++)
            {
                var checker = GetTimeLineItemControlAt(i);
                if (checker == null)
                    continue;
                checker.GetPlacementInfo(ref s, ref w, ref e);
                if (dropX < s)
                {
                    return i;
                }

                if (s < dropX && e > dropX)
                {
                    double distStart = Math.Abs(dropX - s);
                    double distEnd = Math.Abs(dropX - e);
                    if (distStart < distEnd) //we dropped closer to the start of this item
                    {
                        return i;
                    }

                    //we are closer to the end of this item
                    return i + 1;
                }

                if (e < dropX && i == Items.Count() - 1)
                {
                    return i + 1;
                }

                if (s < dropX && i == Items.Count() - 1)
                {
                    return i;
                }
            }

            return Items.Count;

        }

        private void MakeRoom(int newIndex, double width)
        {
            int moveIndex = newIndex + 1;
            //get our forward chain and gap
            double chainGap;

            //because the grid is child 0 and we are essentially indexing as if it wasn't there
            //the child index of add after is our effective index of next
            var nextCtrl = GetTimeLineItemControlAt(moveIndex);
            if (nextCtrl != null)
            {
                double nL = 0;
                double nW = 0;
                double nE = 0;
                nextCtrl.GetPlacementInfo(ref nL, ref nW, ref nE);

                double droppedIntoSpace = 0;
                if (newIndex == 0)
                {
                    var currentControl = GetTimeLineItemControlAt(newIndex);
                    if (currentControl != null)
                    {
                        double cL = 0;
                        double cW = 0;
                        double cE = 0;
                        currentControl.GetPlacementInfo(ref cL, ref cW, ref cE);
                        droppedIntoSpace = nL - cL;
                    }
                    else
                        droppedIntoSpace = nL;

                }
                else
                {
                    var previousControl = GetTimeLineItemControlAt(newIndex - 1);
                    if (previousControl != null)
                    {
                        double pL = 0;
                        double pW = 0;
                        double pE = 0;
                        previousControl.GetPlacementInfo(ref pL, ref pW, ref pE);
                        droppedIntoSpace = nL - pE;
                    }
                }

                double neededSpace = width - droppedIntoSpace;
                if (neededSpace <= 0)
                    return;

                var forwardChain = GetTimeLineForwardChain(nextCtrl, moveIndex + 1, out chainGap);

                if (chainGap < neededSpace)
                {
                    while (neededSpace > 0)
                    {
                        //move it to the smaller of our values -gap or remaning space
                        double move = Math.Min(chainGap, neededSpace);
                        foreach (var tictrl in forwardChain)
                        {
                            tictrl.MoveItem(move);
                            neededSpace -= move;
                        }

                        //get our new chain and new gap
                        forwardChain = GetTimeLineForwardChain(nextCtrl, moveIndex + 1, out chainGap);
                    }
                }
                else
                {
                    foreach (var tictrl in forwardChain)
                    {
                        tictrl.MoveItem(neededSpace);
                    }
                }

            } //if next ctrl is null we are adding to the very end and there is no work to do to make room.
        }

        #endregion

        //NOT WORKING YET AND I DON'T KNOW WHY 8(
        private void DragScroll(DragEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = GetParentScrollViewer();
            }

            if (_scrollViewer != null)
            {
                //var available = LayoutInformation.GetLayoutSlot(this);
                Point scrollPos = e.GetPosition(_scrollViewer);
                double scrollMargin = 50;
                var actualW = _scrollViewer.ActualWidth;
                if (scrollPos.X >= actualW - scrollMargin &&
                    _scrollViewer.HorizontalOffset <= _scrollViewer.ExtentWidth - _scrollViewer.ViewportWidth)
                {
                    _scrollViewer.LineRight();
                }
                else if (scrollPos.X < scrollMargin && _scrollViewer.HorizontalOffset > 0)
                {
                    _scrollViewer.LineLeft();
                }

                double actualH = _scrollViewer.ActualHeight;
                if (scrollPos.Y >= actualH - scrollMargin &&
                    _scrollViewer.VerticalOffset <= _scrollViewer.ExtentHeight - _scrollViewer.ViewportHeight)
                {
                    _scrollViewer.LineDown();
                }
                else if (scrollPos.Y < scrollMargin && _scrollViewer.VerticalOffset >= 0)
                {
                    _scrollViewer.LineUp();
                }
            }
        }

        #endregion

        #region edit events etc

        private double _curX;
        private TimeLineAction _action;

        void item_PreviewEditButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TimeLineControlItem)?.ReleaseMouseCapture();
            foreach (ITimeLineControlData item in Items)
            {
                item.StartTime=item.StartTime.Value.AddMilliseconds(-item.StartTime.Value.Millisecond);
                item.EndTime=item.EndTime.Value.AddMilliseconds(-item.EndTime.Value.Millisecond);
                
            }
            //(sender as TimeLineControlItem)?.SnapTimes();
            Keyboard.Focus(this);
        }

        void item_PreviewEditButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TimeLineControlItem ctrl) _action = ctrl.GetClickAction();
            (sender as TimeLineControlItem)?.CaptureMouse();
        }

        #region key down and up

        bool _rightCtrlDown;
        bool _leftCtrlDown;

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _rightCtrlDown = e.Key == Key.RightCtrl;
                _leftCtrlDown = e.Key == Key.LeftCtrl;
                ManipulationMode = TimeLineManipulationMode.Linked;
            }
        }

        protected void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                _leftCtrlDown = false;
            if (e.Key == Key.RightCtrl)
                _rightCtrlDown = false;
            if (!_leftCtrlDown && !_rightCtrlDown)
                ManipulationMode = TimeLineManipulationMode.Linked;
        }

        private void item_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TimeLineControlItem tlci = (TimeLineControlItem)sender;
                Items.Remove((ITimeLineControlData)tlci.Content);
            }
        }

        internal void HandleItemManipulation(TimeLineControlItem ctrl, TimeLineItemChangedEventArgs e)
        {

            bool doStretch;
            TimeSpan deltaT = e.DeltaTime;
            TimeSpan zeroT = new TimeSpan();
            int direction = deltaT.CompareTo(zeroT);
            if (direction == 0)
                return; //shouldn't happen

            var after = GetTimeLineItemControlStartingAfter(ctrl.StartTime, out var afterIndex);
            var previous = GetTimeLineItemControlStartingBefore(ctrl.StartTime, out var previousIndex);
            if (after != null)
                after.ReadyToDraw = false;
            ctrl.ReadyToDraw = false;
            double useDeltaX = e.DeltaX;
            double cLeft = 0;
            double cWidth = 0;
            double cEnd = 0;
            ctrl.GetPlacementInfo(ref cLeft, ref cWidth, ref cEnd);

            switch (e.Action)
            {
                case TimeLineAction.Move:

                    #region move

                    double chainGap;
                    if (direction > 0)
                    {
                        //find chain connecteds that are after this one
                        //delta each one in that chain that we are pushing
                        List<TimeLineControlItem> afterChain = GetTimeLineForwardChain(ctrl, afterIndex, out chainGap);

                        if (chainGap < useDeltaX)
                            useDeltaX = chainGap;
                        foreach (var ti in afterChain)
                        {
                            ti.MoveItem(useDeltaX);
                        }

                        //find the size of our chain so we bring it into view
                        var first = afterChain[0];
                        var last = afterChain[afterChain.Count - 1];
                        BringChainIntoView(first, last, direction);


                    }

                    if (direction < 0)
                    {
                        List<TimeLineControlItem> previousChain = GetTimeLineBackwardsChain(ctrl, previousIndex,
                            out var previousBackToStart, out chainGap);
                        if (-chainGap > useDeltaX)
                        {
                            useDeltaX = chainGap;
                        }

                        if (!previousBackToStart)
                        {
                            foreach (var ti in previousChain)
                            {
                                ti.MoveItem(useDeltaX);
                            }
                        }

                        var first = previousChain[0]; //previousChain[previousChain.Count - 1];
                        var last = previousChain[previousChain.Count - 1];
                        BringChainIntoView(last, first, direction);
                    }

                    #endregion

                    break;
                case TimeLineAction.StretchStart:
                    switch (e.Mode)
                    {
                        #region stretchstart

                        case TimeLineManipulationMode.Linked:

                            #region linked

                            double gap = double.MaxValue;
                            if (previous != null)
                            {
                                double pLeft = 0;
                                double pWidth = 0;
                                double pEnd = 0;
                                previous.GetPlacementInfo(ref pLeft, ref pWidth, ref pEnd);
                                gap = cLeft - pEnd;
                            }

                            if (direction < 0 && Math.Abs(gap) < Math.Abs(useDeltaX) &&
                                Math.Abs(gap) > _bumpThreshold) //if we are negative and not linked, but about to bump
                                useDeltaX = -gap;
                            if (Math.Abs(gap) < _bumpThreshold)
                            {
                                //we are linked
                                if (previous != null && ctrl.CanDelta(0, useDeltaX) && previous.CanDelta(1, useDeltaX))
                                {
                                    ctrl.MoveStartTime(useDeltaX);
                                    previous.MoveEndTime(useDeltaX);
                                }
                            }
                            else if (ctrl.CanDelta(0, useDeltaX))
                            {
                                ctrl.MoveStartTime(useDeltaX);
                            }


                            break;

                        #endregion

                        case TimeLineManipulationMode.Free:

                            #region free

                            doStretch = direction > 0;
                            if (direction < 0)
                            {
                                //disallow us from free stretching into another item

                                if (previous != null)
                                {
                                    double pLeft = 0;
                                    double pWidth = 0;
                                    double pEnd = 0;
                                    previous.GetPlacementInfo(ref pLeft, ref pWidth, ref pEnd);
                                    gap = cLeft - pEnd;


                                }

                                else
                                {
                                    //don't allow us to stretch further than the gap between current and start time
                                    //DateTime s = (DateTime)GetValue(StartDateProperty);
                                    gap = cLeft;
                                }

                                doStretch = gap > _bumpThreshold;
                                if (gap < useDeltaX)
                                {
                                    useDeltaX = gap;
                                }
                            }

                            doStretch &= ctrl.CanDelta(0, useDeltaX);

                            if (doStretch)
                            {
                                ctrl.MoveStartTime(useDeltaX);
                            }

                            #endregion

                            break;
                        default:
                            break;

                        #endregion
                    }

                    break;
                case TimeLineAction.StretchEnd:
                    switch (e.Mode)
                    {
                        #region stretchend

                        case TimeLineManipulationMode.Linked:

                            #region linked

                            double gap = double.MaxValue;
                            if (after != null)
                            {
                                double aLeft = 0;
                                double aWidth = 0;
                                double aEnd = 0;
                                after.GetPlacementInfo(ref aLeft, ref aWidth, ref aEnd);
                                gap = aLeft - cEnd;
                            }

                            if (direction > 0 && gap > _bumpThreshold &&
                                gap < useDeltaX) //if we are positive, not linked but about to bump
                                useDeltaX = -gap;
                            if (gap < _bumpThreshold)
                            {
                                //we are linked
                                if (after != null && ctrl.CanDelta(1, useDeltaX) && after.CanDelta(0, useDeltaX))
                                {
                                    ctrl.MoveEndTime(useDeltaX);
                                    after.MoveStartTime(useDeltaX);
                                }
                            }
                            else if (ctrl.CanDelta(0, useDeltaX))
                            {
                                ctrl.MoveEndTime(useDeltaX);
                            }

                            break;

                        #endregion

                        case TimeLineManipulationMode.Free:

                            #region free

                            doStretch = true;
                            if (direction > 0 && after != null)
                            {
                                //disallow us from free stretching into another item
                                double nLeft = 0;
                                double nWidth = 0;
                                double nEnd = 0;
                                after.GetPlacementInfo(ref nLeft, ref nWidth, ref nEnd);
                                var nextGap = nLeft - cEnd;
                                doStretch = nextGap > _bumpThreshold;
                                if (nextGap < useDeltaX)
                                    useDeltaX = nextGap;
                            }


                            doStretch &= ctrl.CanDelta(1, useDeltaX);
                            if (doStretch)
                            {
                                ctrl.MoveEndTime(useDeltaX);
                            }

                            break;

                        #endregion

                        default:
                            break;

                        #endregion
                    }

                    break;
            }
        }

        private void BringChainIntoView(TimeLineControlItem first, TimeLineControlItem last, int direction)
        {
            double l1 = 0;
            double l2 = 0;
            double w = 0;
            double w2 = 0;
            double end = 0;
            first.GetPlacementInfo(ref l1, ref w, ref end);
            last.GetPlacementInfo(ref l2, ref w2, ref end);
            double chainW = end - l1;
            double leadBuffer = 4 * UnitSize;
            chainW += leadBuffer;
            if (direction > 0)
            {

                first.BringIntoView(new Rect(new Point(0, 0), new Point(chainW, Height)));
            }
            else
            {
                first.BringIntoView(new Rect(new Point(-leadBuffer, 0), new Point(chainW, Height)));
            }

        }

        #endregion

        #endregion

        /// <summary>
        /// Mouse move is important for both edit and drag behaviors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_MouseMove(object sender, MouseEventArgs e)
        {
            #region drag - left click and move

            TimeLineControlItem ctrl = sender as TimeLineControlItem;
            if (ctrl == null)
                return;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (ctrl.IsExpanded)
                    return;
                var position = Mouse.GetPosition(null);
                if (Math.Abs(position.X - _dragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _dragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DragDrop.DoDragDrop(this, ctrl, DragDropEffects.Move | DragDropEffects.Scroll);
                    //_dragging = true;
                }

                return;
            }

            #endregion


            #region edits - right click and move

            if (!Equals(Mouse.Captured, ctrl))
            {
                _curX = Mouse.GetPosition(null).X;
                return;
            }

            double mouseX = Mouse.GetPosition(null).X;
            double deltaX = mouseX - _curX;
            TimeSpan deltaT = ctrl.GetDeltaTime(deltaX);
            var curMode = (TimeLineManipulationMode)GetValue(ManipulationModeProperty);
            HandleItemManipulation(ctrl, new TimeLineItemChangedEventArgs()
            {
                Action = _action,
                DeltaTime = deltaT,
                DeltaX = deltaX,
                Mode = curMode
            });

            DrawBackGround();
            _curX = mouseX;

            //When we pressed, this lost focus and we therefore didn't capture any changes to the key status
            //so we check it again after our manipulation finishes.  That way we can be linked and go out of or back into it while dragging
            ManipulationMode = TimeLineManipulationMode.Free;
            _leftCtrlDown = Keyboard.IsKeyDown(Key.LeftCtrl);
            _rightCtrlDown = Keyboard.IsKeyDown(Key.RightCtrl);
            if (_leftCtrlDown || _rightCtrlDown)
            {
                ManipulationMode = TimeLineManipulationMode.Linked;
            }

            #endregion
        }

        #region get children methods

        /// <summary>
        /// Returns a list of all timeline controls starting with the current one and moving forward
        /// so long as they are contiguous.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="afterIndex"></param>
        /// <param name="chainGap"></param>
        /// <returns></returns>
        private List<TimeLineControlItem> GetTimeLineForwardChain(TimeLineControlItem current, int afterIndex, out double chainGap)
        {
            List<TimeLineControlItem> returner = new List<TimeLineControlItem>() { current };
            double left = 0, width = 0, end = 0;
            current.GetPlacementInfo(ref left, ref width, ref end);
            if (afterIndex < 0)
            {
                //we are on the end of the list so there is no limit.
                chainGap = double.MaxValue;
                return returner;
            }

            double bumpThreshold = _bumpThreshold;
            double lastAddedEnd = end;
            while (afterIndex < Items.Count)
            {
                left = width = end = 0;
                var checker = GetTimeLineItemControlAt(afterIndex++);
                if (checker != null)
                {
                    checker.GetPlacementInfo(ref left, ref width, ref end);
                    double gap = left - lastAddedEnd;
                    if (gap > bumpThreshold)
                    {
                        chainGap = gap;
                        return returner;
                    }

                    returner.Add(checker);
                    lastAddedEnd = end;
                }

            }

            //we have chained off to the end and thus have no need to worry about our gap
            chainGap = double.MaxValue;
            return returner;
        }

        /// <summary>
        /// Returns a list of all timeline controls starting with the current one and moving backwoards
        /// so long as they are contiguous.  If the chain reaches back to the start time of the timeline then the
        /// ChainsBackToStart boolean is modified to reflect that.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="prevIndex"></param>
        /// <param name="chainsBackToStart"></param>
        /// <param name="chainGap"></param>
        /// <returns></returns>
        private List<TimeLineControlItem> GetTimeLineBackwardsChain(TimeLineControlItem current, int prevIndex,
            out bool chainsBackToStart, out double chainGap)
        {
            List<TimeLineControlItem> returner = new List<TimeLineControlItem>() { current };
            double left = 0, width = 0, end = 0;
            current.GetPlacementInfo(ref left, ref width, ref end);
            if (prevIndex < 0)
            {
                chainGap = double.MaxValue;
                chainsBackToStart = left == 0;
                return returner;
            }

            double lastAddedLeft = left;
            while (prevIndex >= 0)
            {
                left = width = end = 0;
                var checker = GetTimeLineItemControlAt(prevIndex--);
                if (checker != null)
                {
                    checker.GetPlacementInfo(ref left, ref width, ref end);
                    if (lastAddedLeft - end > _bumpThreshold)
                    {
                        //our chain just broke;
                        chainGap = lastAddedLeft - end;
                        chainsBackToStart = lastAddedLeft == 0;
                        return returner;
                    }

                    returner.Add(checker);
                    lastAddedLeft = left;
                }

            }

            chainsBackToStart = lastAddedLeft == 0;
            chainGap = lastAddedLeft; //gap between us and zero;
            return returner;

        }

        private TimeLineControlItem GetTimeLineItemControlStartingBefore(DateTime dateTime, out int index)
        {
            index = -1;
            for (int i = 0; i < Items.Count; i++)
            {
                var checker = GetTimeLineItemControlAt(i);
                if (checker != null && checker.StartTime == dateTime && i != 0)
                {
                    index = i - 1;
                    return GetTimeLineItemControlAt(i - 1);
                }
            }

            index = -1;
            return null;
        }

        private TimeLineControlItem GetTimeLineItemControlStartingAfter(DateTime dateTime, out int index)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var checker = GetTimeLineItemControlAt(i);
                if (checker != null && checker.StartTime > dateTime)
                {
                    index = i;
                    return checker;
                }
            }

            index = -1;
            return null;
        }

        private TimeLineControlItem GetTimeLineItemControlAt(int i)
        {
            //child 0 is our background grid.
            i++;
            if (i <= 0 || i >= Children.Count)
                return null;
            return Children[i] as TimeLineControlItem;
        }
        #endregion
    }
}
