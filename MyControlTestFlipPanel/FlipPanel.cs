using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MyControlTestFlipPanel
{
    [TemplateVisualState (Name = "Normal",GroupName = "ViewStates")]
    [TemplateVisualState (Name = "Flipped",GroupName = "ViewStates")]
    [TemplatePart(Name = "FlipButton",Type =typeof(ToggleButton))]
    [TemplatePart(Name = "FlipButtonAlternate",Type =typeof(ToggleButton))]
    public class FlipPanel : Control
    {
        #region 静态构造函数

        static FlipPanel()
        {
            #region 注册依赖属性

            FrontContentProperty = DependencyProperty.Register("FrontContent", typeof(object), typeof(FlipPanel), null);
            BackContentProperty = DependencyProperty.Register("BackContent", typeof(object), typeof(FlipPanel), null);
            IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(FlipPanel), null);
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FlipPanel), null);

            #endregion

            #region 通知控件从generic.xaml文件获取默认样式

            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipPanel), new FrameworkPropertyMetadata(typeof(FlipPanel)));

            #endregion
        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty FrontContentProperty;
        public static readonly DependencyProperty BackContentProperty;
        public static readonly DependencyProperty IsFlippedProperty;
        public static readonly DependencyProperty CornerRadiusProperty;

        #endregion

        #region 依赖属性包装器

        public object FrontContent
        {
            get { return GetValue(FrontContentProperty); }
            set { SetValue(FrontContentProperty, value); }
        }

        public object BackContent
        {
            get { return GetValue(BackContentProperty); }
            set { SetValue(BackContentProperty, value); }
        }

        public bool IsFlipped
        {
            get { return (bool) GetValue(IsFlippedProperty); }
            set
            {
                SetValue(IsFlippedProperty, value);
                ChangedVisualState(true);
            }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region 公共方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ToggleButton flipButton = base.GetTemplateChild("FlipButton") as ToggleButton;
            if (flipButton != null) flipButton.Click += flipButton_Click;

            ToggleButton flipButtonAlternate = base.GetTemplateChild("FlipButtonAlternate") as ToggleButton;
            if (flipButtonAlternate != null) flipButtonAlternate.Click += flipButton_Click;

            this.ChangeVisualState(false);
        }

        #endregion

        #region 私有方法

        private void flipButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsFlipped = !this.IsFlipped;
            ChangedVisualState(true);
        }

        #endregion
    }
}