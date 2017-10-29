using BusyDays.Common;
using BusyDays.Model;
using BusyDays.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BusyDays.View {
    /// <summary>
    /// MainView.xaml の相互作用ロジック
    /// </summary>
    public partial class MainView : Window {
        private MainViewModel vm;

        public MainView(string path) {
            InitializeComponent();
            vm = new MainViewModel(new CommonService(this), path);
            this.DataContext = vm;
        }

        #region D&D
        private ListBoxItem dragItem;
        private Point dragStartPos;

        private void listBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            dragItem = sender as ListBoxItem;
            dragStartPos = e.GetPosition(dragItem);
        }

        void listBoxItem_Drop(object sender, DragEventArgs e) {
            var dropped = dragItem.DataContext as TaskItem;
            var target = ((ListBoxItem)sender).DataContext as TaskItem;
            vm.TaskDrop(dropped, target);
        }

        private void listBoxItem_PreviewMouseMove(object sender, MouseEventArgs e) {
            var item = sender as ListBoxItem;
            if (e.LeftButton != MouseButtonState.Pressed || dragItem == item) {
                return;
            }

            var nowPos = e.GetPosition(item);
            if (Math.Abs(nowPos.X - dragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(nowPos.Y - dragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance) {
                DragDrop.DoDragDrop(item, item.DataContext, DragDropEffects.Move);
            }
        }
        #endregion
    }
}
