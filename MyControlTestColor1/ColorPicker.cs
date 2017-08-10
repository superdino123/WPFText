using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MyControlTestColor1
{
    /*记录模板部件，技术非必需，便于查看*/

    [TemplatePart(Name = "PART_RedSlider", Type = typeof(RangeBase))]
    [TemplatePart(Name = "PART_GreenSlider", Type = typeof(RangeBase))]
    [TemplatePart(Name = "PART_BlueSlider", Type = typeof(RangeBase))]
    [TemplatePart(Name = "PART_PreviewBrush", Type = typeof(SolidColorBrush))]
    public class ColorPicker : Control
    {
        #region 私有字段

        private Color? previousColor;

        #endregion

        #region 静态构造函数

        static ColorPicker()
        {
            #region 注册依赖项属性

            ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.Black, OnColorChanged));
            RedProperty = DependencyProperty.Register("Red", typeof(byte), typeof(ColorPicker),
                new FrameworkPropertyMetadata(OnColorRGBChanged));
            GreenProperty = DependencyProperty.Register("Green", typeof(byte), typeof(ColorPicker),
                new FrameworkPropertyMetadata(OnColorRGBChanged));
            BlueProperty = DependencyProperty.Register("Blue", typeof(byte), typeof(ColorPicker),
                new FrameworkPropertyMetadata(OnColorRGBChanged));

            #endregion

            #region 注册事件

            ColorChangedEvent = EventManager.RegisterRoutedEvent(
                "ColorChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));

            #endregion

            #region 命令

            CommandManager.RegisterClassCommandBinding(typeof(ColorPicker), new CommandBinding(ApplicationCommands.Undo, UndoCommand_Executed, UndoCommand_CanExecute));

            #endregion

            #region 为自定义控件定义默认样式的依赖项属性

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

            #endregion
        }

        #endregion

        #region 后台添加绑定与事件

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var sliderRed = GetTemplateChild("PART_RedSlider") as RangeBase;
            if (sliderRed != null)
            {
                var binding = new Binding("Red");
                binding.Source = this;
                binding.Mode = BindingMode.TwoWay;
                sliderRed.SetBinding(RangeBase.ValueProperty, binding);
            }

            var sliderGreen = GetTemplateChild("PART_GreenSlider") as RangeBase;
            if (sliderGreen != null)
            {
                var binding = new Binding("Green");
                binding.Source = this;
                binding.Mode = BindingMode.TwoWay;
                sliderGreen.SetBinding(RangeBase.ValueProperty, binding);
            }

            var sliderBlue = GetTemplateChild("PART_BlueSlider") as RangeBase;
            if (sliderBlue != null)
            {
                var binding = new Binding("Blue");
                binding.Source = this;
                binding.Mode = BindingMode.TwoWay;
                sliderBlue.SetBinding(RangeBase.ValueProperty, binding);
            }

            var brush = GetTemplateChild("PART_PreviewBrush") as SolidColorBrush;
            if (brush != null)
            {
                var binding = new Binding("Color");
                binding.Source = brush;
                binding.Mode = BindingMode.OneWayToSource;
                SetBinding(ColorProperty, binding);
            }
        }

        #endregion

        #region 静态字段

        #region 依赖属性

        public static DependencyProperty ColorProperty;
        public static DependencyProperty RedProperty;
        public static DependencyProperty GreenProperty;
        public static DependencyProperty BlueProperty;

        #endregion

        #region 创建路由事件

        public static readonly RoutedEvent ColorChangedEvent;

        #endregion

        #endregion

        #region 变量封装器

        #region 依赖属性封装器

        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public byte Red
        {
            get { return (byte) GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }

        public byte Green
        {
            get { return (byte) GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        public byte Blue
        {
            get { return (byte) GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        #endregion

        #region 路由事件封装器

        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        #endregion

        #endregion

        #region 私有方法

        private static void OnColorRGBChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var colorUserControl = (ColorPicker) sender;
            var color = colorUserControl.Color;

            if (e.Property == RedProperty)
                color.R = (byte) e.NewValue;
            else if (e.Property == GreenProperty)
                color.G = (byte) e.NewValue;
            else if (e.Property == BlueProperty)
                color.B = (byte) e.NewValue;
            colorUserControl.Color = color;
        }

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newColor = (Color) e.NewValue;
            var oldColor = (Color) e.OldValue;

            var colorUserControl = (ColorPicker) sender;
            colorUserControl.Red = newColor.R;
            colorUserControl.Green = newColor.G;
            colorUserControl.Blue = newColor.B;

            var args = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor);
            args.RoutedEvent = ColorChangedEvent;
            colorUserControl.RaiseEvent(args);

            colorUserControl.previousColor = (Color) e.OldValue;
        }

        #endregion

        #region Command

        private void SetUpCommands()
        {
            /*CommandBinding binding = new CommandBinding(ApplicationCommands.Undo, UndoCommand_Executed, UndoCommand_CanExecute);
            this.CommandBindings.Add(binding);*/
        }

        private static void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = previousColor.HasValue;
            var colorUserControl = (ColorPicker) sender;
            e.CanExecute = colorUserControl.previousColor.HasValue;
        }

        private static void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //this.Color = (Color) previousColor;
            var colorUserControl = (ColorPicker) sender;
            var currentColor = colorUserControl.Color;
            colorUserControl.Color = (Color) colorUserControl.previousColor;
        }

        #endregion
    }
}