using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents a Metro UI search control.
    /// </summary>
    /// <remarks>
    /// Public Properties:
    ///     PlaceholderText       : string
    ///     ListBackground        : Brush
    ///     ListForeground        : Brush
    ///     ButtonHoverBackground : Brush
    ///     ButtonHoverForeground : Brush
    ///     SelectedBackground    : Brush
    ///     SelectedForeground    : Brush
    ///     SeperatorColor        : Brush
    ///     
    /// Public Events:
    ///     SearchClick           : Bubble
    ///     SettingBtnClick       :
    ///     SettingBtnChecked
    /// </remarks>
    public class SearchBox : ComboBox
    {
        /// <summary>
        /// Initializes a new instance of the SearchBox class.
        /// </summary>
        public SearchBox()
            : base()
        {
            DefaultStyleKey = typeof(SearchBox);
        }


        /// <summary>
        /// Adds or removes the event handler 
        /// </summary>
        public event RoutedEventHandler SettingBtnChecked
        {
            add { AddHandler(SettingBtnCheckedEvent, value); }
            remove { RemoveHandler(SettingBtnCheckedEvent, value); }
        }

        /// <summary>
        /// Routed event register for the SettingBtnChecked event.
        /// </summary>
        public static readonly RoutedEvent SettingBtnCheckedEvent =
            EventManager.RegisterRoutedEvent("SettingBtnChecked",
                                              RoutingStrategy.Bubble,
                                              typeof(RoutedEventHandler),
                                              typeof(SearchBox)
                                            );



        /// <summary>
        /// Adds or removes the event handler 
        /// </summary>
        public event RoutedEventHandler SettingBtnClick
        {
            add { AddHandler(SettingBtnClickEvent, value); }
            remove { RemoveHandler(SettingBtnClickEvent, value); }
        }

        /// <summary>
        /// Routed event register for the SettingBtnClick event.
        /// </summary>
        public static readonly RoutedEvent SettingBtnClickEvent =
            EventManager.RegisterRoutedEvent("SettingBtnClick",
                                              RoutingStrategy.Bubble,
                                              typeof(RoutedEventHandler),
                                              typeof(SearchBox)
                                            );



        /// <summary>
        /// Adds or removes the event handler for the TabCloseClick event.
        /// </summary>
        public event RoutedEventHandler SearchClick
        {
            add { AddHandler(SearchClickEvent, value); }
            remove { RemoveHandler(SearchClickEvent, value); }
        }

        /// <summary>
        /// Routed event register for the SearchClick event.
        /// </summary>
        public static readonly RoutedEvent SearchClickEvent =
            EventManager.RegisterRoutedEvent("SearchClick",
                                              RoutingStrategy.Bubble,
                                              typeof(RoutedEventHandler),
                                              typeof(SearchBox)
                                            );


        /// <summary>
        /// Gets or sets the placeholder text value.
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Dependency Property for SearchHolderText.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked",
                                         typeof(bool),
                                         typeof(SearchBox),
                                         new PropertyMetadata(false)
                                       );

        /// <summary>
        /// Gets or sets the placeholder text value.
        /// </summary>
        public bool IsShowSettingButton
        {
            get { return (bool)GetValue(IsShowSettingButtonProperty); }
            set { SetValue(IsShowSettingButtonProperty, value); }
        }

        /// <summary>
        /// Dependency Property for SearchHolderText.
        /// </summary>
        public static readonly DependencyProperty IsShowSettingButtonProperty =
            DependencyProperty.Register("IsShowSettingButton",
                                         typeof(bool),
                                         typeof(SearchBox),
                                         new PropertyMetadata(false)
                                       );


        /// <summary>
        /// Gets or sets the placeholder text value.
        /// </summary>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        /// <summary>
        /// Dependency Property for SearchHolderText.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText",
                                         typeof(string),
                                         typeof(SearchBox),
                                         new PropertyMetadata(string.Empty)
                                       );

        /// <summary>
        /// Gets or sets the dropdown list background color.
        /// </summary>
        public Brush ListBackground
        {
            get { return (Brush)GetValue(ListBackgroundProperty); }
            set { SetValue(ListBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency Property for ListBackground.
        /// </summary>
        public static readonly DependencyProperty ListBackgroundProperty =
            DependencyProperty.Register("ListBackground",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 255, 255, 255)))
                                       );

        /// <summary>
        /// Gets or sets the dropdown list foreground color.
        /// </summary>
        public Brush ListForeground
        {
            get { return (Brush)GetValue(ListForegroundProperty); }
            set { SetValue(ListForegroundProperty, value); }
        }

        /// <summary>
        /// Dependency Property for ListForeground.
        /// </summary>
        public static readonly DependencyProperty ListForegroundProperty =
            DependencyProperty.Register("ListForeground",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 51, 51, 51)))
                                       );

        /// <summary>
        /// Gets or sets the button background color while in the hover-over state.
        /// </summary>
        public Brush ButtonHoverBackground
        {
            get { return (Brush)GetValue(ButtonHoverBackgroundProperty); }
            set { SetValue(ButtonHoverBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency Property for ButtonHoverBackground.
        /// </summary>
        public static readonly DependencyProperty ButtonHoverBackgroundProperty =
            DependencyProperty.Register("ButtonHoverBackground",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 27, 161, 226)))
                                       );

        /// <summary>
        /// Gets or sets the button foreground color while in the hover-over state.
        /// </summary>
        public Brush ButtonHoverForeground
        {
            get { return (Brush)GetValue(ButtonHoverForegroundProperty); }
            set { SetValue(ButtonHoverForegroundProperty, value); }
        }

        /// <summary>
        /// Dependency Property for ButtonHoverForeground.
        /// </summary>
        public static readonly DependencyProperty ButtonHoverForegroundProperty =
            DependencyProperty.Register("ButtonHoverForeground",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 255, 255, 255)))
                                       );

        /// <summary>
        /// Gets or sets the list item background color while in the selected state.
        /// </summary>
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency Property for SelectedBackground.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 27, 161, 226)))
                                       );

        /// <summary>
        /// Gets or sets the list item foreground color while in the selected state.
        /// </summary>
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        /// <summary>
        /// Dependency Property for SelectedForeground.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 255, 255, 255)))
                                       );

        /// <summary>
        /// Gets or sets the list item separator color.
        /// </summary>
        public Brush SeparatorColor
        {
            get { return (Brush)GetValue(SeparatorColorProperty); }
            set { SetValue(SeparatorColorProperty, value); }
        }

        /// <summary>
        /// Dependency Property for SeparatorColor.
        /// </summary>
        public static readonly DependencyProperty SeparatorColorProperty =
            DependencyProperty.Register("SeparatorColor",
                                         typeof(Brush),
                                         typeof(SearchBox),
                                         new PropertyMetadata(
                                             new SolidColorBrush(
                                                 Color.FromArgb(255, 204, 204, 204)))
                                       );

        private void searchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (this.Text != string.Empty)
            //{
            //    if (!this.Items.Contains(this.Text))
            //    {
            //        this.Items.Add(this.Text);
            //    }
            //}

            this.RaiseEvent(new RoutedEventArgs(SearchClickEvent, this));
        }



        private void SettingBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IsChecked = !IsChecked;

            this.RaiseEvent(new RoutedEventArgs(SettingBtnClickEvent, this));
        }


        private void SettingBtn_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(SettingBtnCheckedEvent, this));
        }




        /// <summary>
        /// Invoked whenever application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button searchButton = base.GetTemplateChild("BtnSearch") as Button;
            if (searchButton != null)
            {
                searchButton.Click += new RoutedEventHandler(searchButton_Click);
            }

            ToggleButton settingBtn = base.GetTemplateChild("ToggleBtnSetting") as ToggleButton;
            if (settingBtn != null)
            {
                settingBtn.Click += new RoutedEventHandler(SettingBtn_Click);
                settingBtn.Checked += new RoutedEventHandler(SettingBtn_Checked);
            }


        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Let ComboBox's original method handle it
            /*base.OnPreviewKeyDown(e);

            if (e.Key == Key.Enter)
                e.Handled = false;
            return;*/
        }

    }
}
