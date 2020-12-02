using System.ComponentModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SeasonalATeam.Classes
{
    public class DecemberWindow : Button, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static DependencyProperty DayOfMonthProperty =
            DependencyProperty.Register(
            "DayOfMonth",
            typeof(int),
            typeof(DecemberWindow),
            null
        );

        public static DependencyProperty OpenProperty =
            DependencyProperty.Register(
            "Open",
            typeof(bool),
            typeof(DecemberWindow),
            null
        );

        public static DependencyProperty PersonOpacityProperty =
            DependencyProperty.Register(
            "PersonOpacity",
            typeof(double),
            typeof(DecemberWindow),
            null
        );

        public static DependencyProperty PersonImageProperty =
            DependencyProperty.Register(
            "PersonImage",
            typeof(BitmapImage),
            typeof(DecemberWindow),
            null
        );

        public static DependencyProperty AccessibleNameProperty =
            DependencyProperty.Register(
            "AccessibleName",
            typeof(string),
            typeof(DecemberWindow),
            null
        );

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DecemberWindowAutomationPeer(this);
        }

        public int DayOfMonth
        {
            get => (int)GetValue(DayOfMonthProperty);
            set => SetValue(DayOfMonthProperty, value);
        }

        public double PersonOpacity
        {
            get => (double)GetValue(PersonOpacityProperty);
            set => SetValue(PersonOpacityProperty, value);
        }

        public BitmapImage PersonImage
        {
            get => (BitmapImage)GetValue(PersonImageProperty);
            set => SetValue(PersonImageProperty, value);
        }

        public string AccessibleName
        {
            get => (string)GetValue(AccessibleNameProperty);
            set => SetValue(AccessibleNameProperty, value);
        }

        public bool Open
        {
            get => (bool)GetValue(OpenProperty);
            set => SetValue(OpenProperty, value);
        }

        private string person;
        public string Person { get => person; set => person = value; }
    }

    public class DecemberWindowAutomationPeer : ButtonAutomationPeer
    {
        private DecemberWindow button;

        public DecemberWindowAutomationPeer(DecemberWindow owner) :
            base(owner)
        {
            button = owner;
        }

        protected override string GetLocalizedControlTypeCore()
        {
            var resourceLoader = new ResourceLoader();
            return resourceLoader.GetString("DecemberWindow");
        }
    }
}
