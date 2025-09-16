using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ListBox = System.Windows.Forms.ListBox;

namespace GUIObj.Controls
{
    /// <summary>
    /// Interaction logic for SpinnerInt.xaml
    /// </summary>
    public partial class SpinnerInt : UserControl
    {
        private int _value;
        private int _minValue = int.MinValue;
        private int _maxValue = int.MaxValue;
        private int _increment = 1;
        private int _incrementShift = 10;
        private int _incrementShiftAlt = 100;


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(0, OnValueChange));
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(0, OnDefaultValueChange));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(int.MinValue, OnMinValueChange));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(int.MaxValue, OnMaxValueChange));
        public static readonly DependencyProperty IncrementValueProperty = DependencyProperty.Register("IncrementValue", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(1, OnIncrementValueChange));
        public static readonly DependencyProperty IncrementShiftValueProperty = DependencyProperty.Register("IncrementShiftValue", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(10, OnIncrementShiftValueChange));
        public static readonly DependencyProperty IncrementShiftAltValueProperty = DependencyProperty.Register("IncrementShiftAltValue", typeof(int), typeof(SpinnerInt), new FrameworkPropertyMetadata(100, OnIncrementShiftAltValueChange));

        public static readonly DependencyProperty ShowDefaultProperty = DependencyProperty.Register("ShowDefault", typeof(bool), typeof(SpinnerInt), new FrameworkPropertyMetadata(true, OnShowDefaultChange));
        public static readonly DependencyProperty AllowTextInputProperty = DependencyProperty.Register("AllowTextInput", typeof(bool), typeof(SpinnerInt), new FrameworkPropertyMetadata(false, OnAllowTextInputChange));


        public event ValueChangedEvent OnValueChanged;

        public delegate void ValueChangedEvent(int value);
        private static void OnAllowTextInputChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
            {
                b.AllowTextInput = (bool)e.NewValue;
                
            }
        }

        private static void OnShowDefaultChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
            {
                b.ShowDefault = (bool)e.NewValue;
            }
        }

        private static void OnIncrementValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.Increment = ((int)e.NewValue);
        }
        private static void OnIncrementShiftValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.IncrementShift = ((int)e.NewValue);
        }
        private static void OnIncrementShiftAltValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.IncrementShiftAlt = ((int)e.NewValue);
        }

        private static void OnMaxValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.MaxValue = ((int)e.NewValue);
        }

        private static void OnMinValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.MinValue = ((int)e.NewValue);
        }

        private static void OnDefaultValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.DefaultValue = ((int)e.NewValue);
        }

        private static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerInt b)
                b.Value = ((int)e.NewValue);
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                try
                {
                    tb_value.Text = _value.ToString();
                    OnValueChanged?.Invoke(value);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public int DefaultValue { get; set; }

        public int MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public int MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public int Increment
        {
            get => _increment;
            set => _increment = value;
        }

        public int IncrementShift
        {
            get => _incrementShift;
            set => _incrementShift = value;
        }

        public int IncrementShiftAlt
        {
            get => _incrementShiftAlt;
            set => _incrementShiftAlt = value;
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
        
        public SpinnerInt()
        {
            InitializeComponent();
        }

        private void Btn_up_Click(object sender, RoutedEventArgs e)
        {
            int i= _increment;
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                i = _incrementShiftAlt;
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                i = _incrementShift;     
            _value += i; 

            if (_value > MaxValue)
                _value = MaxValue;
            tb_value.Text = _value.ToString();
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void Btn_down_Click(object sender, RoutedEventArgs e)
        {

            int i = _increment;
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                i = _incrementShiftAlt;
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                i = _incrementShift;
            _value -= i;
            if (_value < MinValue)
                _value = MinValue;
            tb_value.Text = _value.ToString();
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void Btn_default_Click(object sender, RoutedEventArgs e)
        {
            _value = DefaultValue;
            tb_value.Text = _value.ToString();
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void ValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(int.TryParse(((TextBox)sender).Text + e.Text, out int i) && i >= MinValue && i <= MaxValue);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!int.TryParse(((TextBox)sender).Text, out int j) || j < MinValue || j > MaxValue)
            {
                _value = j < MinValue ? MinValue : MaxValue;
                ((TextBox)sender).Text = _value.ToString();
            }
            else
            {
                ((TextBox)sender).Text = j.ToString();
                _value = j;
            }
            OnValueChanged?.Invoke(_value);
            SetIncButtonState();
        }

        private void SetIncButtonState()
        {
            if(btn_up!=null)
                btn_up.IsEnabled = _value < _maxValue;
            if (btn_down != null)
                btn_down.IsEnabled = _value > _minValue;
        }
    }
}
