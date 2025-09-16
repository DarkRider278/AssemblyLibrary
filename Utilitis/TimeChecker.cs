using System;

namespace HelpUtility
{
    public class TimeChecker
    {
        private bool _checkDays;
        private bool _checkTime;

        private int _startH;
        private int _startM;
        private int _endH;
        private int _endM;

        private string _days;

        private bool _isOverlappind;
        private int _startTime;
        private int _endTime;

        public TimeChecker()
        {
            
        }

        public bool CheckDays
        {
            get { return _checkDays; }
            set { _checkDays = value; }
        }

        public bool CheckTime
        {
            get { return _checkTime; }
            set { _checkTime = value; }
        }

        public int StartH
        {
            get { return _startH; }
            set
            {
                _startH = value;
                SetOverlapping();
            }
        }

        public int StartM
        {
            get { return _startM; }
            set
            {
                _startM = value;
                SetOverlapping();
            }
        }

        public int EndH
        {
            get { return _endH; }
            set
            {
                _endH = value;
                SetOverlapping();
            }
        }

        public int EndM
        {
            get { return _endM; }
            set
            {
                _endM = value;
                SetOverlapping();
            }
        }

        public string Days
        {
            get { return _days; }
            set { _days = value; }
        }

        void SetOverlapping()
        {
            _startTime = _startH * 60 + _startM;
            _endTime = _endH * 60 + _endM;
            _isOverlappind = _endTime < _startTime;
        }

        public bool IsValid()
        {
            return IsValid(DateTime.Now);
        }

        public bool IsValid(DateTime dt)
        {
            int d, d2;
            

            if (_checkDays)
            {
                d = (int)dt.DayOfWeek;
                if (_isOverlappind)
                {
                    d2 = d + 1;
                    if (d2 > 6)
                        d2 = 0;
                    if (_days[d] != '1' && _days[d2]!='1')
                        return false;
                }
                else
                {
                    if (_days[d] != '1')
                        return false;
                }
            }

            if (_checkTime)
            {
                d = dt.Hour * 60 + dt.Minute;
                if (_isOverlappind)
                {
                    if (d >= _startTime || d <= _endTime)
                    {
                        return true;
                    }
                }
                else
                {
                    if (d >= _startTime && d <= _endTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetConfig()
        {
            return string.Format("1#{0}#{1}#{2}#{3}#{4}#{5}#{6}", _checkDays ? "1" : "0", _checkTime ? "1" : "0", _days,
                _startH, _startM, _endH, _endM);
        }

        public void SetConfig(string config)
        {
            if (config.StartsWith("1"))
            {
                string[] p = config.Split(new[] { "#" }, StringSplitOptions.None);
                if(p.Length<8)
                    return;
                _checkDays = p[1] == "1";
                _checkTime = p[2] == "1";
                _days = p[3];
                while (_days.Length < 7)
                    _days += "0";
                Int32.TryParse(p[4], out _startH);
                Int32.TryParse(p[5], out _startM);
                Int32.TryParse(p[6], out _endH);
                Int32.TryParse(p[7], out _endM);
            }
        }
    }
}
