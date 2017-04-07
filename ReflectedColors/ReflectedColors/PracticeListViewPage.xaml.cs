using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReflectedColors
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PracticeListViewPage : ContentPage
    {
        public PracticeListViewPage()
        {
            InitializeComponent();
            BindingContext = new ColorsViewModel();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            bool result = await DisplayAlert("Row Selected", $"You selected {(e.SelectedItem as SimpleColor).Name}", "OK", "Cancel");
            Debug.WriteLine("dialog closed with a value of " + result);
        }
    }

    public class ColorsViewModel
    {
        private ObservableCollection<SimpleColor> colors;
        public ObservableCollection<SimpleColor> Colors
        {
            get
            {
                return colors;
            }
            private set
            {
                colors = value;
            }
        }

        int index;
        public ColorsViewModel()
        {
            var tmpColors = typeof(Color).GetRuntimeFields()
                .Where(f => f.IsPublic && f.IsStatic && f.Name != "Transparent" && f.Name != "Silver" && f.Name != "Green" && f.Name != "Red")
                .Select(f => new SimpleColor
                {
                    FontColor = (Color)f.GetValue(null),
                    Name = f.Name
                });

            Colors = new ObservableCollection<SimpleColor>(tmpColors);
            index = Colors.Count - 1;

            RefreshDataCommand = new Command(() => RefreshData());

            Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(250);
                        //  RefreshDataCommand.Execute(null);
                    }
                });
        }

        public ICommand RefreshDataCommand { get; set; }

        private void RefreshData()
        {
            string nextStatus = index == 1 ? SimpleColor.STATUS_STOPPED : SimpleColor.STATUS_MOVING;
            var movingColor = Colors[index];
            Colors.Move(index, index - 1);
            index = index > 1 ? index - 1 : Colors.Count - 1;
            movingColor.UpdateStatus(nextStatus);
        }
    }

    public class SimpleColor : INotifyPropertyChanged
    {
        public const string STATUS_MOVING = "Moving";
        public const string STATUS_STOPPED = "Stopped";

        private string detail = STATUS_STOPPED;
        private Color detailColor = Color.Red;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public Color FontColor { get; set; }
        public string Detail
        {
            get { return detail; }
            private set { SetProperty(ref detail, value); }
        }

        public Color DetailColor
        {
            get { return detailColor; }
            private set { SetProperty(ref detailColor, value); }
        }

        public void UpdateStatus(string newStatus)
        {
            Detail = newStatus;
            DetailColor = detail == STATUS_MOVING ? Color.Green : Color.Red;
        }

        private void SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (object.Equals(storage, value))
                return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
