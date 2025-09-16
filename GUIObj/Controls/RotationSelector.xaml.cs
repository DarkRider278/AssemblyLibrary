using System.Windows;

namespace GUIObj.Controls
{
	/// <summary>
	/// Interaction logic for player_rot_sel.xaml
	/// </summary>
	public partial class RotationSelector
	{
		public RotationSelector()
		{
            _setvalue = true;
			InitializeComponent();
		    _setvalue = false;

		}

	    private bool _setvalue;
        public delegate void DirectionChangeEventHandler(object sender, int pan);
        public event DirectionChangeEventHandler DirectionChange;

        private void rb_bottom_Checked(object sender, RoutedEventArgs e)
        {
            if (_setvalue)
                return;
            if(DirectionChange==null)
                return;
            if(rb_bottom.IsChecked==true)
            {
                DirectionChange(this, 0);
                VisualStateManager.GoToState(this, "s_bottom", true);
            }
            else if(rb_right.IsChecked==true)
            {
                DirectionChange(this, 90);
                VisualStateManager.GoToState(this, "s_right", true);
            }
            else if(rb_top.IsChecked==true)
            {
                DirectionChange(this, 180);
                VisualStateManager.GoToState(this, "s_top", true);
            }
			else if(rb_bottomleft.IsChecked==true)
            {
                DirectionChange(this, 315);
                VisualStateManager.GoToState(this, "s_bottomleft", true);
            }
			else if(rb_bottomright.IsChecked==true)
            {
                DirectionChange(this, 45);
                VisualStateManager.GoToState(this, "s_bottomright", true);
            }
			else if(rb_topleft.IsChecked==true)
            {
                DirectionChange(this, 225);
                VisualStateManager.GoToState(this, "s_topleft", true);
            }
			else if(rb_topright.IsChecked==true)
            {
                DirectionChange(this, 135);
                VisualStateManager.GoToState(this, "s_topright", true);
            }
            else
            {
                DirectionChange(this, 270);
                VisualStateManager.GoToState(this, "s_left", true);
            }
        }

        public void SetDirection(int pan)
        {
            _setvalue = true;
            switch(pan)
            {
                case 0:
                    rb_bottom.IsChecked = true;
                    break;
				case 45:
                    rb_bottomright.IsChecked = true;
                    break;
                case 90:
                    rb_right.IsChecked = true;
                    break;
				 case 1350:
                    rb_topright.IsChecked = true;
                    break;
                case 180:
                    rb_top.IsChecked = true;
                    break;
				case 225:
                    rb_topleft.IsChecked = true;
                    break;
                case 270:
                    rb_left.IsChecked = true;
                    break;								 
                case 315:
                    rb_bottomleft.IsChecked = true;
                    break;
                default:
                    rb_bottom.IsChecked = true;
                    break;
            }
            _setvalue = false;
        }

	}

   
}