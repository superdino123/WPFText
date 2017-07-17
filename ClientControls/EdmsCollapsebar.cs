using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace ClientControls
{
    /// <summary>
    /// </summary>
    public class EdmsCollapsebar : Expander
    {
        public enum ModeOptions
        {
            Normal,
            WrapOnly
        }

        // Using a DependencyProperty as the backing store for GripOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GripOffsetProperty =
            DependencyProperty.Register("GripOffset", typeof(double), typeof(EdmsCollapsebar));

        // Using a DependencyProperty as the backing store for CollapsedSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapsedSizeProperty =
            DependencyProperty.Register("CollapsedSize", typeof(GridLength), typeof(EdmsCollapsebar), new UIPropertyMetadata(new GridLength(0, GridUnitType.Auto)));

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ModeOptions), typeof(EdmsCollapsebar), new UIPropertyMetadata(ModeOptions.Normal));


        private GridLength _startupSize;
        private GridLength SizeCollapsedBackup = new GridLength(200);

        private GridLength? SizeExpandedBackup;


        static EdmsCollapsebar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EdmsCollapsebar), new FrameworkPropertyMetadata(typeof(EdmsCollapsebar)));
        }

        public EdmsCollapsebar()
        {
            Expanded += Sidebar_Expanded;
            Collapsed += Sidebar_Collapsed;
        }

        public GridLength StartupSize
        {
            get { return _startupSize; }
            set
            {
                SizeExpandedBackup = value;
                _startupSize = value;
                SetCurrentValue(StoredWidthProperty, value);
            }
        }


        /// <summary>
        ///     The offset of the toggle grip
        /// </summary>
        public double GripOffset
        {
            get { return (double)GetValue(GripOffsetProperty); }
            set { SetValue(GripOffsetProperty, value); }
        }


        /// <summary>
        ///     The size of the collapsed sidebar
        /// </summary>
        public GridLength CollapsedSize
        {
            get { return (GridLength)GetValue(CollapsedSizeProperty); }
            set { SetValue(CollapsedSizeProperty, value); }
        }


        public ModeOptions Mode
        {
            get { return (ModeOptions)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        //private GridLength MinWidthBackup = new GridLength(0);

        private void Sidebar_Collapsed(object sender, RoutedEventArgs e)
        {
            if (Mode == ModeOptions.Normal)
            {
                if (ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right)
                {
                    SizeExpandedBackup = StoredWidth = new GridLength(ActualWidth);
                    //  MinWidthBackup = MinWidth;
                    SetCurrentValue(StoredWidthProperty, GridLength.Auto);
                }
                if (ExpandDirection == ExpandDirection.Down || ExpandDirection == ExpandDirection.Up)
                {
                    SizeExpandedBackup = StoredHeight = new GridLength(ActualHeight);
                    SetCurrentValue(StoredHeightProperty, GridLength.Auto);
                }
            }
        }

        private void Sidebar_Expanded(object sender, RoutedEventArgs e)
        {
            if (Mode == ModeOptions.Normal)
            {
                if (ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right)
                {
                    SizeCollapsedBackup = StoredWidth;
                    if (SizeExpandedBackup == null)
                    {
                        SizeExpandedBackup = new GridLength(200);
                    }
                    SetCurrentValue(StoredWidthProperty, SizeExpandedBackup);
                }
                if (ExpandDirection == ExpandDirection.Down || ExpandDirection == ExpandDirection.Up)
                {
                    SizeCollapsedBackup = StoredHeight;
                    if (SizeCollapsedBackup == null)
                    {
                        SizeExpandedBackup = new GridLength(200);
                    }
                    SetCurrentValue(StoredHeightProperty, SizeExpandedBackup);
                }

            }
        }

        #region DependencyProperty StoredWidth

        /// <summary>
        /// </summary>
        public GridLength StoredWidth
        {
            get { return (GridLength)GetValue(StoredWidthProperty); }
            set { SetValue(StoredWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StoredWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoredWidthProperty =
            DependencyProperty.RegisterAttached("StoredWidth", typeof(GridLength), typeof(EdmsCollapsebar));

        #endregion


        #region DependencyProperty StoredHeight
        public static readonly DependencyProperty StoredHeightProperty = DependencyProperty.Register(
            "StoredHeight", typeof(GridLength), typeof(EdmsCollapsebar), new PropertyMetadata(default(GridLength)));

        public GridLength StoredHeight
        {
            get { return (GridLength)GetValue(StoredHeightProperty); }
            set { SetValue(StoredHeightProperty, value); }
        }
        #endregion


    }


    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var margin = (double)value;
                var direction = (string)parameter;
                switch (direction)
                {
                    case "Left":
                        return new Thickness(-margin, 0, 0, 0); // 8 is the width of the expand button itself
                    case "Right":
                        return new Thickness(0, 0, -margin, 0); // 8 is the width of the expand button itself
                    case "Up":
                        return new Thickness(0, -margin, 0, 0);
                    case "Down":
                        return new Thickness(0, 0, 0, -margin);
                    default:
                        return new Thickness(0);
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back from a replacement!");
        }
    }
}
