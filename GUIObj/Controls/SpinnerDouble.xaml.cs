using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUIObj.Controls
{
    /// <summary>
    /// Interaction logic for SpinnerInt.xaml
    /// </summary>
    public partial class SpinnerDouble : UserControl
    {
        private double _value;
        private double _defaultValue;
        private double _minValue=double.MinValue;
        private double _maxValue = double.MaxValue;
        private double _increment = 1;
        private double _incrementShift = 10;
        private double _incrementShiftAlt = 100;
        private double _incrementCtrl = 0.1;
        private double _incrementCtrlAlt = 0.01;


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(0.0, OnValueChange));
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(0.0, OnDefaultValueChange));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(double.MinValue, OnMinValueChange));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(double.MaxValue, OnMaxValueChange));
        public static readonly DependencyProperty IncrementValueProperty = DependencyProperty.Register("IncrementValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(1.0, OnIncrementValueChange));
        public static readonly DependencyProperty IncrementShiftValueProperty = DependencyProperty.Register("IncrementShiftValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(10.0, OnIncrementShiftValueChange));
        public static readonly DependencyProperty IncrementShiftAltValueProperty = DependencyProperty.Register("IncrementShiftAltValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(100.0, OnIncrementShiftAltValueChange));
        public static readonly DependencyProperty IncrementCtrlValueProperty = DependencyProperty.Register("IncrementCtrlValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(0.1, OnIncrementCtrlValueChange));
        public static readonly DependencyProperty IncrementCtrltAltValueProperty = DependencyProperty.Register("IncrementCtrlAltValue", typeof(double), typeof(SpinnerDouble), new FrameworkPropertyMetadata(0.01, OnIncrementCtrlAltValueChange));

        public static readonly DependencyProperty ShowDefaultProperty = DependencyProperty.Register("ShowDefault", typeof(bool), typeof(SpinnerDouble), new FrameworkPropertyMetadata(true, OnShowDefaultChange));
        public static readonly DependencyProperty AllowTextInputProperty = DependencyProperty.Register("AllowTextInput", typeof(bool), typeof(SpinnerDouble), new FrameworkPropertyMetadata(false, OnAllowTextInputChange));


        public event ValueChangedEvent OnValueChanged;

        public delegate void ValueChangedEvent(double value);
        private static void OnAllowTextInputChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
            {
                b.AllowTextInput = (bool)e.NewValue;          
            }
        }

        private static void OnShowDefaultChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
            {
                b.ShowDefault = (bool)e.NewValue;
            }
        }

        private static void OnIncrementValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.Increment = ((double)e.NewValue);
        }

        private static void OnIncrementShiftValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.IncrementShift = ((double)e.NewValue);
        }
        private static void OnIncrementShiftAltValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.IncrementShiftAlt = ((double)e.NewValue);
        }
        private static void OnIncrementCtrlValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.IncrementCtrl = ((double)e.NewValue);
        }
        private static void OnIncrementCtrlAltValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.IncrementCtrlAlt = ((double)e.NewValue);
        }

        private static void OnMaxValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.MaxValue = ((double)e.NewValue);
        }

        private static void OnMinValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.MinValue = ((double)e.NewValue);
        }

        private static void OnDefaultValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.DefaultValue = ((double)e.NewValue);
        }

        private static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerDouble b)
                b.Value =  ((double)e.NewValue);
        }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                try
                {
                    tb_value.Text = _value.ToString("0.0#",CultureInfo.InvariantCulture);
                    OnValueChanged?.Invoke(value);
                }
                catch
                {
                    // ignored
                }
            }

        }

        public double DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public double MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public double MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public double Increment
        {
            get => _increment;
            set => _increment = value;
        }

        public double IncrementShift
        {
            get => _incrementShift;
            set => _incrementShift = value;
        }

        public double IncrementShiftAlt
        {
            get => _incrementShiftAlt;
            set => _incrementShiftAlt = value;
        }
        public double IncrementCtrl
        {
            get => _incrementCtrl;
            set => _incrementCtrl = value;
        }

        public double IncrementCtrlAlt
        {
            get => _incrementCtrlAlt;
            set => _incrementCtrlAlt = value;
        }


        public bool ShowDefault
        {
            get => gr_main.ColumnDefinitions[2].Width.Value > 1;
            set => gr_main.ColumnDefinitions[2].Width = new GridLength(value ? 22 : 0);
        }

        public bool AllowTextInput
        {
            get => tb_value.IsReadOnly;
            set => tb_value.IsReadOnly = !value;
        }
        
        public SpinnerDouble()
        {
            InitializeComponent();
        }

        private void Btn_up_Click(object sender, RoutedEventArgs e)
        {
            double i = _increment;
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                i = _incrementShiftAlt;
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                i = _incrementCtrlAlt;
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                i = _incrementShift;
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                i = _incrementCtrl;


            _value += i; 

            if (_value > _maxValue)
                _value = _maxValue;
            tb_value.Text = _value.ToString("0.0#",CultureInfo.InvariantCulture);
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void Btn_down_Click(object sender, RoutedEventArgs e)
        {

            double i = _increment;
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                i = _incrementShiftAlt;
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                i = _incrementCtrlAlt;
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                i = _incrementShift;
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                i = _incrementCtrl;

            _value -= i;
            if (_value < _minValue)
                _value = _minValue;
            tb_value.Text = _value.ToString("0.0#", CultureInfo.InvariantCulture);
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void Btn_default_Click(object sender, RoutedEventArgs e)
        {
            _value = _defaultValue;
            tb_value.Text = _value.ToString("0.0#", CultureInfo.InvariantCulture);
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void ValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            
            e.Handled = !(double.TryParse(((TextBox)sender).Text + e.Text.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture , out double i) && i >= MinValue && i <= MaxValue);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse(((TextBox)sender).Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double j) || j < MinValue || j > MaxValue)
            {
                _value = j < MinValue ? MinValue : MaxValue;
                ((TextBox)sender).Text = _value.ToString("0.0#", CultureInfo.InvariantCulture);
            }
            else
            {
                ((TextBox)sender).Text = j.ToString("0.0#", CultureInfo.InvariantCulture);
                _value = j;
            }
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void SetIncButtonState()
        {
            if(btn_up!=null)
                btn_up.IsEnabled = !(_value >= _maxValue);
            if (btn_down != null)
                btn_down.IsEnabled = !(_value <= _minValue);
        }
    }
}
