using BusyDays.Common;
using BusyDays.Model;
using BusyDays.ViewModel;
using System.Windows;

namespace BusyDays.View {
    /// <summary>
    /// TaskAddView.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskAddView : Window {
        public TaskAddView(TaskItem item) {
            InitializeComponent();
            this.DataContext = new TaskEditViewModel(new CommonService(this), item);
        }

        // 位置調整
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var main = Application.Current.MainWindow;
            // 素早く入力に移れるように親ウインドウの中央に移動させる
            this.Left = main.Left + (main.ActualWidth - this.ActualWidth) / 2;
            this.Top = main.Top + (main.ActualHeight - this.ActualHeight) / 2;
        }
    }
}
