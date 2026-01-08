using System.Windows;
using System.Windows.Controls;

namespace MQTT.Sharing.Controls
{
    public partial class DisplayInfo : UserControl
    {
        #region DependencyProperty
        public string TextLeftUp
        {
            get { return (string)GetValue(TextLeftUpProperty); }
            set { SetValue(TextLeftUpProperty, value); }
        }
        public static readonly DependencyProperty TextLeftUpProperty =
            DependencyProperty.Register(nameof(TextLeftUp), typeof(string), typeof(DisplayInfo), new PropertyMetadata(null));

        public string TextLeftDown
        {
            get { return (string)GetValue(TextLeftDownProperty); }
            set { SetValue(TextLeftDownProperty, value); }
        }
        public static readonly DependencyProperty TextLeftDownProperty =
            DependencyProperty.Register(nameof(TextLeftDown), typeof(string), typeof(DisplayInfo), new PropertyMetadata(null));
        
        public string TextRightUp
        {
            get { return (string)GetValue(TextRightUpProperty); }
            set { SetValue(TextRightUpProperty, value); }
        }
        public static readonly DependencyProperty TextRightUpProperty =
            DependencyProperty.Register(nameof(TextRightUp), typeof(string), typeof(DisplayInfo), new PropertyMetadata(null));

        public string TextRightDown
        {
            get { return (string)GetValue(TextRightDownProperty); }
            set { SetValue(TextRightDownProperty, value); }
        }
        public static readonly DependencyProperty TextRightDownProperty =
            DependencyProperty.Register(nameof(TextRightDown), typeof(string), typeof(DisplayInfo), new PropertyMetadata(null));
        #endregion

        #region Builder
        public DisplayInfo()
        {
            InitializeComponent();
        }
        #endregion
    }
}
