using SeasonalATeam.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace SeasonalATeam
{
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private const int dayCount = 31;
        private const int gridSize = 10;

        private DecemberWindow[] decemberWindows = new DecemberWindow[dayCount];

        private string[] People = {
            "Herbi 1",
            "Herbi 2",
            "Herbi 3",
            "Herbi 4",
            "Herbi 5",
            "Herbi 6",
            "Herbi 7",
            "Herbi 8",
            "Herbi 9",
            "Herbi 10",
            "Herbi 11",
            "Herbi 12",
            "Herbi 13",
            "Herbi 14",
            "Herbi 15",
            "Herbi 16",
            "Herbi 17",
            "Herbi 18",
            "Herbi 19",
            "Herbi 20",
            "Herbi 21",
            "Herbi 22",
            "Herbi 23",
            "Herbi 24",
            "Herbi 25",
            "Herbi 26",
            "Herbi 27",
            "Herbi 28",
            "Herbi 29",
            "Herbi 30",
            "Herbi 31",
        };

        public MainPage()
        {
            this.InitializeComponent();

            this.KeyDown += MainPage_KeyDown;

            if (!LoadSettings())
            {
                RestartCalendar();
            }

            decemberWindows[0].Focus(FocusState.Keyboard);
        }

        private void RestartCalendar()
        {
            CalendarContainer.Children.Clear();

            bool[,] cellFilled = new bool[gridSize, gridSize];

            var shuffler = new Shuffler();
            shuffler.Shuffle(People);

            int row;
            int column;

            int[] decemberWindowTabIndices = new int[dayCount];

            for (int i = 0; i < dayCount; ++i)
            {
                var decemberWindow = new DecemberWindow();

                decemberWindow.DayOfMonth = (i + 1);
                decemberWindow.Content = decemberWindow.DayOfMonth.ToString();

                string suffix = GetDayOfMonthSuffix(decemberWindow.DayOfMonth);

                SetDecemberWindowAccessibleName(decemberWindow);

                decemberWindow.Name = "DecemberWindow" + i;

                decemberWindow.Template = (ControlTemplate)Application.Current.Resources["DecemberCellTemplate"];

                decemberWindow.Person = People[i];
                decemberWindow.PersonImage = new BitmapImage(
                    new Uri("ms-appx:///Assets/People/" + decemberWindow.Person + ".png"));

                decemberWindow.Click += DecemberWindow_Click;

                do
                {
                    var randRow = new Random();
                    row = randRow.Next() % gridSize;

                    var randColumn = new Random();
                    column = randColumn.Next() % gridSize;
                }
                while (cellFilled[row, column]);

                // Is doesn't matter that the TabIndex aren't contiguous,
                // only that they're int the required order.
                decemberWindow.TabIndex = (row * gridSize) + column;

                decemberWindows[i] = decemberWindow;
                decemberWindowTabIndices[i] = decemberWindow.TabIndex;

                cellFilled[row, column] = true;

                Grid.SetRow(decemberWindow, row);
                Grid.SetColumn(decemberWindow, column);

                CalendarContainer.Children.Add(decemberWindow);

                // Persist this window.
                localSettings.Values[i + "DayOfMonth"] =
                    decemberWindow.DayOfMonth;
                localSettings.Values[i + "Person"] = 
                    decemberWindow.Person;
                localSettings.Values[i + "Row"] = row;
                localSettings.Values[i + "Column"] = column;
                localSettings.Values[i + "Open"] = false;
            }

            SetUIFlow(decemberWindowTabIndices);
        }

        public bool LoadSettings()
        {
            bool loadedWindows = true;

            if (!localSettings.Values.ContainsKey("0DayOfMonth"))
            {
                return false;
            }

            try
            {
                int[] decemberWindowTabIndices = new int[dayCount];

                for (int i = 0; i < dayCount; ++i)
                {
                    var decemberWindow = new DecemberWindow();

                    decemberWindow.Person = (string)localSettings.Values[i + "Person"];
                    decemberWindow.DayOfMonth = (int)localSettings.Values[i + "DayOfMonth"];

                    if (localSettings.Values.ContainsKey(decemberWindow.DayOfMonth + "Open"))
                    {
                        decemberWindow.Open = (bool)localSettings.Values[decemberWindow.DayOfMonth + "Open"];
                        decemberWindow.PersonOpacity = (decemberWindow.Open ? 1 : 0);
                    }

                    decemberWindow.Content = decemberWindow.DayOfMonth.ToString();

                    string suffix = GetDayOfMonthSuffix(decemberWindow.DayOfMonth);

                    SetDecemberWindowAccessibleName(decemberWindow);

                    decemberWindow.Name = "DecemberWindow" + i;

                    decemberWindow.Template = (ControlTemplate)Application.Current.Resources["DecemberCellTemplate"];

                    decemberWindow.PersonImage = new BitmapImage(
                        new Uri("ms-appx:///Assets/People/" + decemberWindow.Person + ".png"));

                    decemberWindow.Click += DecemberWindow_Click;

                    var row = (int)localSettings.Values[i + "Row"];
                    var column = (int)localSettings.Values[i + "Column"];

                    decemberWindow.TabIndex = (row * gridSize) + column;

                    decemberWindows[i] = decemberWindow;
                    decemberWindowTabIndices[i] = decemberWindow.TabIndex;

                    Grid.SetRow(decemberWindow, row);
                    Grid.SetColumn(decemberWindow, column);

                    CalendarContainer.Children.Add(decemberWindow);
                }

                SetUIFlow(decemberWindowTabIndices);
            }
            catch (Exception ex)
            {
                loadedWindows = false;
            }

            return loadedWindows;
        }

        private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.F5)
            {
                RestartCalendar();

                decemberWindows[0].Focus(FocusState.Keyboard);

                var peer = FrameworkElementAutomationPeer.FromElement(decemberWindows[0]);
                if (peer != null)
                {
                    var resourceLoader = new ResourceLoader();

                    peer.RaiseNotificationEvent(
                        AutomationNotificationKind.ActionCompleted,
                        AutomationNotificationProcessing.ImportantAll,
                        resourceLoader.GetString("Restarted"),
                        "DecemberActivity");
                }
            }
        }

        private string GetDayOfMonthSuffix(int dayOfMonth)
        {
            string suffix;
            if ((dayOfMonth != 11) && (dayOfMonth % 10 == 1))
            {
                suffix = "st";
            }
            else if ((dayOfMonth != 12) && (dayOfMonth % 10 == 2))
            {
                suffix = "nd";
            }
            else if ((dayOfMonth != 13) && (dayOfMonth % 10 == 3))
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }

            return suffix;
        }

        private void SetDecemberWindowAccessibleName(DecemberWindow decemberWindow)
        {
            var resourceLoader = new ResourceLoader();

            string decemberWindowAccessibleName =
                decemberWindow.DayOfMonth + GetDayOfMonthSuffix(decemberWindow.DayOfMonth) + ", " +
                (decemberWindow.Open ? decemberWindow.Person : resourceLoader.GetString("Closed"));
            AutomationProperties.SetName(decemberWindow, decemberWindowAccessibleName);
        }

        private void SetUIFlow(int[] decemberWindowTabIndices)
        { 
            int temp = 0;
            DecemberWindow tempDecemberWindow;

            for (int write = 0; write < decemberWindowTabIndices.Length; write++)
            {
                for (int sort = 0; sort < decemberWindowTabIndices.Length - 1; sort++)
                {
                    if (decemberWindowTabIndices[sort] > decemberWindowTabIndices[sort + 1])
                    {
                        temp = decemberWindowTabIndices[sort + 1];
                        decemberWindowTabIndices[sort + 1] = decemberWindowTabIndices[sort];
                        decemberWindowTabIndices[sort] = temp;

                        tempDecemberWindow = decemberWindows[sort + 1];
                        decemberWindows[sort + 1] = decemberWindows[sort];
                        decemberWindows[sort] = tempDecemberWindow;
                    }
                }
            }

            IList<DependencyObject> flowsTo;
            IList<DependencyObject> flowsFrom;

            // Set the flow through the UIA tree.
            for (int i = 1; i < 30; ++i)
            {
                flowsTo = AutomationProperties.GetFlowsTo(decemberWindows[i]);
                flowsTo.Add(decemberWindows[i + 1]);

                flowsFrom = AutomationProperties.GetFlowsFrom(decemberWindows[i]);
                flowsFrom.Add(decemberWindows[i - 1]);
            }

            flowsTo = AutomationProperties.GetFlowsTo(decemberWindows[0]);
            flowsTo.Add(decemberWindows[1]);

            flowsFrom = AutomationProperties.GetFlowsFrom(decemberWindows[0]);
            flowsFrom.Add(decemberWindows[30]);

            flowsTo = AutomationProperties.GetFlowsTo(decemberWindows[30]);
            flowsTo.Add(decemberWindows[0]);

            flowsFrom = AutomationProperties.GetFlowsFrom(decemberWindows[30]);
            flowsFrom.Add(decemberWindows[29]);
        }

        private async Task<bool> CanWindowBeOpened(DecemberWindow decemberWindow)
        {
            bool windowCanBeOpened = true;

            if (decemberWindow.DayOfMonth > DateTime.Now.Date.Day)
            {
                var resourceLoader = new ResourceLoader();

                var tooSoonDialog = new ContentDialog()
                {
                    Title = resourceLoader.GetString("TooSoonTitle"),
                    Content = resourceLoader.GetString("TooSoonContentPrefix") + " " +
                        decemberWindow.DayOfMonth +
                        GetDayOfMonthSuffix(decemberWindow.DayOfMonth) + ".",
                    CloseButtonText = resourceLoader.GetString("OK")
                };

                await tooSoonDialog.ShowAsync();

                windowCanBeOpened = false;
            }
            else
            {
                if (decemberWindow.DayOfMonth > 1)
                {
                    for (int i = 0; i < dayCount; i++)
                    {
                        if (decemberWindows[i].DayOfMonth == decemberWindow.DayOfMonth - 1)
                        {
                            if (!decemberWindows[i].Open)
                            {
                                var resourceLoader = new ResourceLoader();

                                var outOfOrderDialog = new ContentDialog()
                                {
                                    Title = resourceLoader.GetString("OutOfOrderTitle"),
                                    Content = resourceLoader.GetString(
                                        "OutOfOrderOpeningContentPrefix") + " " +
                                        decemberWindows[i].DayOfMonth +
                                        GetDayOfMonthSuffix(decemberWindows[i].DayOfMonth) + ".",
                                    CloseButtonText = resourceLoader.GetString("OK")
                                };

                                await outOfOrderDialog.ShowAsync();

                                windowCanBeOpened = false;
                            }

                            break;
                        }
                    }
                }

                if (windowCanBeOpened && (decemberWindow.DayOfMonth < dayCount))
                {
                    for (int i = 0; i < dayCount; i++)
                    {
                        if (decemberWindows[i].DayOfMonth == decemberWindow.DayOfMonth + 1)
                        {
                            if (decemberWindows[i].Open)
                            {
                                var resourceLoader = new ResourceLoader();

                                var outOfOrderDialog = new ContentDialog()
                                {
                                    Title = resourceLoader.GetString("OutOfOrderTitle"),
                                    Content = resourceLoader.GetString(
                                        "OutOfOrderClosingContentPrefix") + " " +
                                        decemberWindows[i].DayOfMonth +
                                        GetDayOfMonthSuffix(decemberWindows[i].DayOfMonth) + ".",
                                    CloseButtonText = resourceLoader.GetString("OK")
                                };

                                await outOfOrderDialog.ShowAsync();

                                windowCanBeOpened = false;
                            }

                            break;
                        }
                    }
                }

            }

            return windowCanBeOpened;
        }

        private async void DecemberWindow_Click(object sender, RoutedEventArgs e)
        {
            var decemberWindow = sender as DecemberWindow;

            var canWindowBeOpened = await CanWindowBeOpened(decemberWindow);

            if (canWindowBeOpened)
            {
                decemberWindow.PersonOpacity = 1 - decemberWindow.PersonOpacity;

                decemberWindow.Open = !decemberWindow.Open;

                SetDecemberWindowAccessibleName(decemberWindow);

                var peer = FrameworkElementAutomationPeer.FromElement(decemberWindow);
                if (peer != null)
                {
                    var resourceLoader = new ResourceLoader();

                    string message = resourceLoader.GetString(
                        decemberWindow.Open ? "Opening" : "Closing");

                    peer.RaiseNotificationEvent(
                        AutomationNotificationKind.ActionCompleted,
                        AutomationNotificationProcessing.MostRecent,
                        message,
                        "DecemberActivity");
                }

                // Persist the new state of this window.
                localSettings.Values[decemberWindow.DayOfMonth + "Open"] = 
                    decemberWindow.Open;
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            RestartCalendar();
        }
    }

    public class Shuffler
    {
        private readonly Random _random;

        public Shuffler()
        {
            this._random = new Random();
        }

        public void Shuffle<T>(IList<T> array)
        {
            for (int i = array.Count; i > 1;)
            {
                int j = this._random.Next(i);

                --i;

                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
