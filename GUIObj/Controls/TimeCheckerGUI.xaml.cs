using System;
using System.Windows;
using HelpUtility;
using log4net;

namespace GUIObj.Controls
{
    /// <summary>
    /// Interaction logic for TimeCheckerGUI.xaml
    /// </summary>
    public partial class TimeCheckerGui
    {
        private static readonly ILog Log = LogManager.GetLogger("GUIObj");
        private readonly TimeChecker _checker;
        public TimeCheckerGui()
        {
            _checker = new TimeChecker();
            InitializeComponent();

            _checker.CheckDays = cb_check_days.IsChecked == true;
            _checker.CheckTime = cb_check_time.IsChecked == true;
            _checker.Days = BuildDaysData();
            _checker.StartH = iud_start_h.Value;
            _checker.StartM = iud_start_m.Value;
            _checker.EndH = iud_end_h.Value;
            _checker.EndM = iud_end_m.Value;
            
        }   

        private string BuildDaysData()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}", cb_day_sunday.IsChecked == true ? "1" : "0",
                cb_day_monday.IsChecked == true ? "1" : "0", cb_day_tuesday.IsChecked == true ? "1" : "0",
                cb_day_wednesday.IsChecked == true ? "1" : "0", cb_day_thursday.IsChecked == true ? "1" : "0",
                cb_day_friday.IsChecked == true ? "1" : "0", cb_day_saturday.IsChecked == true ? "1" : "0");
        }

        private void SetDayData(string data)
        {
            try
            {
                cb_day_sunday.IsChecked = data[0] =='1';
                cb_day_monday.IsChecked = data[1] == '1';
                cb_day_tuesday.IsChecked = data[2] == '1';
                cb_day_wednesday.IsChecked = data[3] == '1';
                cb_day_thursday.IsChecked = data[4] == '1';
                cb_day_friday.IsChecked = data[5] == '1';
                cb_day_saturday.IsChecked = data[6] == '1';
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Set Day Data: {0}",e.Message);
            }
        }

        private void Cb_check_days_Click(object sender, RoutedEventArgs e)
        {
            _checker.CheckDays = cb_check_days.IsChecked == true;
        }

        private void Cb_check_time_Click(object sender, RoutedEventArgs e)
        {
            _checker.CheckTime = cb_check_time.IsChecked == true;
        }

        private void Cb_day_monday_Click(object sender, RoutedEventArgs e)
        {
            _checker.Days = BuildDaysData();
        }
        public bool IsValid()
        {
            return _checker.IsValid();
        }

        public bool IsValid(DateTime dt)
        {
            return _checker.IsValid(dt);
        }

        public string GetConfig()
        {
            return _checker.GetConfig();
        }

        public void SetConfig(string config)
        {
            _checker.SetConfig(config);
            cb_check_days.IsChecked = _checker.CheckDays;
            cb_check_time.IsChecked = _checker.CheckTime;

            iud_start_h.Value = _checker.StartH;
            iud_start_m.Value = _checker.StartM;
            iud_end_h.Value = _checker.EndH;
            iud_end_m.Value = _checker.EndM;

            SetDayData(_checker.Days);
        }

        private void iud_start_h_OnValueChanged(int value)
        {
            _checker.StartH = iud_start_h.Value;
        }

        private void iud_start_m_OnValueChanged(int value)
        {
            _checker.StartM = iud_start_m.Value;
        }

        private void iud_end_h_OnValueChanged(int value)
        {
            _checker.EndH = iud_end_h.Value;
        }

        private void iud_end_m_OnValueChanged(int value)
        {
            _checker.EndM = iud_end_m.Value;
        }
    }
}
