/******************************************************************************/
/*                                                                            */
/*   Program: MyWpfAsyncAwait                                                 */
/*   Example for binding an ObservableCollection to a ListView and using      */
/*   async await Task to update the data                                      */
/*                                                                            */
/*   28.07.2016 0.0.0.0 uhwgmxorg Start                                       */
/*                                                                            */
/******************************************************************************/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyWpfAsyncAwait
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum ActionType
        {
            OneTime,
            IsContinuousChange
        }


        private Random _random = new Random();
        private object _thradLock = new object();

        private ActionType activActionType = ActionType.OneTime;
        public ActionType ActivActionType
        {
            get
            {
                return activActionType;
            }
            set
            {
                activActionType = value;
                OnPropertyChanged("OneTime");
                OnPropertyChanged("ContinuousChange");
            }
        }
        public bool IsOneTime
        {
            get { return ActivActionType == ActionType.OneTime; }
            set { ActivActionType = value ? ActionType.OneTime : ActivActionType; }
        }
        public bool IsContinuousChange
        {
            get { return ActivActionType == ActionType.IsContinuousChange; }
            set { ActivActionType = value ? ActionType.IsContinuousChange : ActivActionType; }
        }

        private bool taskRunning;
        public bool TaskRunning
        {
            get
            {
                lock (_thradLock)
                {
                    return taskRunning;
                }
            }
            set
            {
                lock (_thradLock)
                {
                    taskRunning = value;
                }
            }
        }

        public ObservableCollection<SubClass> SubClassList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            SubClassList = new ObservableCollection<SubClass>();
            FillList();
            EnableDisableControls();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// button_Start_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button_Start_Click(object sender, RoutedEventArgs e)
        {
            switch(ActivActionType)
            {
                case ActionType.OneTime:
                    TaskRunning = false;
                    UpdateList();
                    break;
                case ActionType.IsContinuousChange:
                    TaskRunning = true;
                    EnableDisableControls();
                    var UpdateTask = Task.Factory.StartNew(() => UpdateList());
                    await UpdateTask;
                    break;
            }
            EnableDisableControls();
        }

        /// <summary>
        /// button_Stop_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Stop_Click(object sender, RoutedEventArgs e)
        {
            TaskRunning = false;
            EnableDisableControls();
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// FillList
        /// </summary>
        private void FillList()
        {
            for (int i = 1; i <= 10; i++)
                SubClassList.Add(new SubClass { Id = i, SubId = i + 10, DValue = RandomDouble(10, 99) });
        }

        /// <summary>
        /// UpdateList
        /// </summary>
        private void UpdateList()
        {
            do
            {
                foreach (var sc in SubClassList)
                    sc.DValue = RandomDouble(10, 99);
                Thread.Sleep(100);
            }
            while (TaskRunning);
        }

        /// <summary>
        /// EnableDisableControls
        /// </summary>
        private void EnableDisableControls()
        {
            button_Start.IsEnabled = !TaskRunning;
            button_Stop.IsEnabled = TaskRunning;
        }

        /// <summary>
        /// Get a random double betwen min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private double RandomDouble(double min, double max,int digits=3)
        {
            double F;
            F = Math.Round(_random.NextDouble() * (max - min) + min, digits);
            return F;
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }

    #region Help Classes
    public class SubClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                if (value != Id)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        private long subId;
        public long SubId
        {
            get { return subId; }
            set
            {
                if (value != SubId)
                {
                    subId = value;
                    OnPropertyChanged("SubId");
                }
            }
        }
        private double dvalue;
        public double DValue
        {
            get { return dvalue; }
            set
            {
                if (value != DValue)
                {
                    dvalue = value;
                    OnPropertyChanged("DValue");
                }
            }
        }
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
    #endregion
}
