using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GUIObj.Enums;
using GUIObj.Structs;

namespace GUIObj.Controls
{
    /// <summary>
    /// Interaction logic for TimeLineControl.xaml
    /// </summary>
    public partial class TimeLineControl : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<ITimeLineControlData>[] _data;
        private DateTime _nullDate;
        private TimeLineStatus[] _status;

        private string _displayStatus;
        private int _preloadtime = 30;
        private System.Windows.Forms.Timer _runner;
        private DateTime _currentRun;
        private bool _runPrepare;
        private int _runTimeline;

        public event PrepareEvent OnTimeLinePrepare;
        public event PlayEvent OnTimeLinePlay;
        public event StopEvent OnTimeLineStop;
        public event OffEvent OnTimeLineOff;
        public event PropertyChangedEventHandler PropertyChanged;


        public DateTime NullDate
        {
            get
            {
                return _nullDate;
            }
        }

        public ObservableCollection<ITimeLineControlData>[] TimeLineData
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Preloadtime
        {
            get { return _preloadtime; }
            set { _preloadtime = value; }
        }

        public string DisplayStatus
        {
            get { return _displayStatus; }
            set { _displayStatus = value; }
        }


        public TimeLineControl()
        {
            TimeLineData = new ObservableCollection<ITimeLineControlData>[8];
            _status = new TimeLineStatus[8];

            for (int i = 0; i < 8; i++)
            {
                TimeLineData[i] = new ObservableCollection<ITimeLineControlData>();
                _status[i] = new TimeLineStatus();
            }

            _nullDate = DateTime.Today;

            _displayStatus = "Idle";
            InitializeComponent();

            ttl_line_1.StartDate = _nullDate;
            ttl_line_2.StartDate = _nullDate;
            ttl_line_3.StartDate = _nullDate;
            ttl_line_4.StartDate = _nullDate;
            ttl_line_5.StartDate = _nullDate;
            ttl_line_6.StartDate = _nullDate;
            ttl_line_7.StartDate = _nullDate;
            ttl_line_8.StartDate = _nullDate;

            ttl_line_1.Items = TimeLineData[0];
            ttl_line_2.Items = TimeLineData[1];
            ttl_line_3.Items = TimeLineData[2];
            ttl_line_4.Items = TimeLineData[3];
            ttl_line_5.Items = TimeLineData[4];
            ttl_line_6.Items = TimeLineData[5];
            ttl_line_7.Items = TimeLineData[6];
            ttl_line_8.Items = TimeLineData[7];

            _runner = new System.Windows.Forms.Timer();
            _runner.Interval = 1000;
            _runner.Tick += _runner_Tick;
        }

        private void _runner_Tick(object sender, EventArgs e)
        {
            _currentRun = _currentRun.AddSeconds(1);
            for (int i = 0; i < 8; i++)
            {
                if (_runTimeline == i || _runTimeline == -1)
                {
                    OffNextItem(i, _currentRun);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (_runTimeline == i || _runTimeline == -1)
                {
                    StopNextItem(i, _currentRun);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (_runTimeline == i || _runTimeline == -1)
                {
                    PlayNextItem(i, _currentRun);
                }
            }

            if (_runPrepare)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (_runTimeline == i || _runTimeline == -1)
                    {
                        PrepareNextItem(i, _currentRun, _preloadtime);
                    }
                }
            }
            _displayStatus = "Running " + _currentRun.ToLongTimeString();
            UpdateMarker();
            OnPropertyChanged(nameof(DisplayStatus));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate bool PrepareEvent(TimeLineControl sender, int timeline, TimeLineControlData item);
        public delegate bool PlayEvent(TimeLineControl sender, int timeline, TimeLineControlData item);
        public delegate bool StopEvent(TimeLineControl sender, int timeline, TimeLineControlData item);
        public delegate bool OffEvent(TimeLineControl sender, int timeline, TimeLineControlData item);

        private void Sl_scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ttl_line_1.UnitSize = sl_scale.Value;
            ttl_line_2.UnitSize = sl_scale.Value;
            ttl_line_3.UnitSize = sl_scale.Value;
            ttl_line_4.UnitSize = sl_scale.Value;
            ttl_line_5.UnitSize = sl_scale.Value;
            ttl_line_6.UnitSize = sl_scale.Value;
            ttl_line_7.UnitSize = sl_scale.Value;
            ttl_line_8.UnitSize = sl_scale.Value;
        }

        public void AddItemAuto(TimeLineControlData i, int timeline)
        {
            i.MoveStartTime(GetLastEndTime(timeline));
            TimeLineData[timeline].Add(i);
        }

        public void Prepare(int timeline = -1)
        {
            for (int i = 0; i < 8; i++)
            {
                if (timeline == i || timeline == -1)
                {
                    _status[i].Reset();
                    foreach (TimeLineControlData item in _data[i])
                    {
                        item.State = TimeLineItemState.NEUTRAL;
                    }
                    PrepareNextItem(i, NullDate, _preloadtime);
                }
            }

            _displayStatus = "Prepared";
            _currentRun = NullDate;
            UpdateMarker();
            OnPropertyChanged(nameof(DisplayStatus));
        }

        public void Play(int timeline = -1)
        {
            _currentRun = NullDate;
            _runPrepare = true;
            _runTimeline = timeline;
            for (int i = 0; i < 8; i++)
            {
                if (timeline == i || timeline == -1)
                {
                    PlayNextItem(i, _currentRun);
                }
            }
            _runner.Start();
            _displayStatus = "Running " + _currentRun.ToLongTimeString();
            UpdateMarker();
            OnPropertyChanged(nameof(DisplayStatus));
        }

        public void PlayOnly(int timeline = -1)
        {
            _currentRun = NullDate;
            _runPrepare = false;
            _runTimeline = timeline;
            for (int i = 0; i < 8; i++)
            {
                if (timeline == i || timeline == -1)
                {
                    _status[i].Reset();
                    foreach (TimeLineControlData item in _data[i])
                    {
                        item.State = TimeLineItemState.NEUTRAL;
                    }
                    PlayNextItem(i, _currentRun);
                }
            }
            _runner.Start();
            _displayStatus = "Running " + _currentRun.ToLongTimeString();
            UpdateMarker();
            OnPropertyChanged(nameof(DisplayStatus));
        }

        public void Stop(int timeline = -1)
        {
            _runner.Stop();
            _displayStatus = "Stopped";
            _currentRun= DateTime.MinValue;
            UpdateMarker();
            OnPropertyChanged(nameof(DisplayStatus));
        }


        private DateTime GetLastEndTime(int timeline)
        {
            DateTime r = _nullDate;
            foreach (ITimeLineControlData item in TimeLineData[timeline])
            {
                if (item.EndTime > r)
                    r = (DateTime)item.EndTime;
            }

            return r;
        }

        private int SortStart(TimeLineControlData x, TimeLineControlData y)
        {
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            if (x.StartTime > y.StartTime)
                return 1;
            if (x.StartTime < y.StartTime)
                return -1;
            return 0;
        }

        private void PrepareNextItem(int tl, DateTime current, int futuretime)
        {
            if (_status[tl].ItemPrepared != 0)
                return;
            foreach (TimeLineControlData item in _data[tl])
            {
                if (item.StartTime >= current && item.StartTime <= current.AddSeconds(futuretime) && item.RunId != _status[tl].ItemPrepared && item.RunId != _status[tl].ItemPlaying)
                {
                    if (item.State == TimeLineItemState.ERROR)
                        return;
                    bool r = true;
                    if (OnTimeLinePrepare != null)
                        r = OnTimeLinePrepare(this, tl, item);
                    if (r)
                    {
                        _status[tl].ItemPrepared = item.RunId;
                        item.State = TimeLineItemState.PREPARED;
                        //Debug.Write("\nPrepare " + item.ItemName + " " + DateTime.Now.ToLongTimeString());
                    }
                    else
                    {
                        item.State = TimeLineItemState.ERROR;
                    }

                    break;
                }
            }
        }

        private void PlayNextItem(int tl, DateTime currentRun)
        {
            if (_runPrepare)
            {
                long pid = _status[tl].ItemPrepared;
                if (pid == 0)
                    return;
                foreach (TimeLineControlData item in _data[tl])
                {
                    if (item.RunId == pid && item.StartTime == currentRun)
                    {
                        bool r = true;
                        if (OnTimeLinePlay != null)
                            r = OnTimeLinePlay(this, tl, item);
                        if (r)
                        {
                            _status[tl].Play();
                            item.State = TimeLineItemState.PLAY;
                           // Debug.Write("\nPlay Prepared " + item.ItemName + " " + DateTime.Now.ToLongTimeString());
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach (TimeLineControlData item in _data[tl])
                {
                    if (item.StartTime == currentRun)
                    {
                        bool r = true;
                        if (OnTimeLinePlay != null)
                            r = OnTimeLinePlay(this, tl, item);
                        if (r)
                        {
                            _status[tl].Play(item.RunId);
                            item.State = TimeLineItemState.PLAY;
                           // Debug.Write("\nPlay " + item.ItemName + " " + DateTime.Now.ToLongTimeString());
                        }
                        break;
                    }
                }
            }
        }

        private void StopNextItem(int tl, DateTime currentRun)
        {
            foreach (TimeLineControlData item in _data[tl])
            {
                if (item.EndTime == currentRun)
                {
                    bool r = true;
                    if (OnTimeLineStop != null)
                        r = OnTimeLineStop(this, tl, item);
                    if (r)
                    {
                        _status[tl].Stop();
                        item.State = TimeLineItemState.FINISH;
                        //Debug.Write("\nStop " + item.ItemName + " " + DateTime.Now.ToLongTimeString());
                    }
                    break;
                }
            }
        }

        private void OffNextItem(int tl, DateTime currentRun)
        {
            foreach (TimeLineControlData item in _data[tl])
            {
                if(item.Offtime<0)
                if (item.OffTime == currentRun )
                {
                    bool r = true;
                    if (OnTimeLineOff != null)
                        r = OnTimeLineOff(this, tl, item);
                    if (r)
                    {
                        _status[tl].Stopping();
                        item.State = TimeLineItemState.STOPPING;
                        //Debug.Write("\nStop " + item.ItemName + " " + DateTime.Now.ToLongTimeString());
                    }
                    break;
                }
            }
        }

        void UpdateMarker()
        {
            if (_runTimeline == 0 || _runTimeline == -1)
                ttl_line_1.RunTime = _currentRun;
            if (_runTimeline == 1 || _runTimeline == -1)
                ttl_line_2.RunTime = _currentRun;
            if (_runTimeline == 2 || _runTimeline == -1)
                ttl_line_3.RunTime = _currentRun;
            if (_runTimeline == 3 || _runTimeline == -1)
                ttl_line_4.RunTime = _currentRun;
            if (_runTimeline == 4 || _runTimeline == -1)
                ttl_line_5.RunTime = _currentRun;
            if (_runTimeline == 5 || _runTimeline == -1)
                ttl_line_6.RunTime = _currentRun;
            if (_runTimeline == 6 || _runTimeline == -1)
                ttl_line_7.RunTime = _currentRun;
            if (_runTimeline == 7 || _runTimeline == -1)
                ttl_line_8.RunTime = _currentRun;
        }
    }

    internal class TimeLineStatus
    {
        private long _itemPrepared = 0;
        private long _itemPlaying = 0;

        public long ItemPrepared
        {
            get { return _itemPrepared; }
            set { _itemPrepared = value; }
        }

        public long ItemPlaying
        {
            get { return _itemPlaying; }
            set { _itemPlaying = value; }
        }

        public void Reset()
        {
            _itemPlaying = _itemPrepared = 0;
        }

        public void Prepare(long id)
        {
            _itemPrepared = id;

        }
        public void Play(long id)
        {
            _itemPlaying = id;
        }
        public void Play()
        {
            _itemPlaying = _itemPrepared;
            _itemPrepared = 0;
        }

        public void Stop()
        {
            _itemPlaying = 0;
        }

        public void Stopping()
        {
            
        }
    }
}
