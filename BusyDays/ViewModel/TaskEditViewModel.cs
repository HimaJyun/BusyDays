using BusyDays.Common;
using BusyDays.Model;
using System.Windows;
using System.Windows.Input;

namespace BusyDays.ViewModel {
    public class TaskEditViewModel {
        private readonly TaskItem item;
        public string TaskTitle {
            get { return item.Title; }
            set { item.Title = value; dirty = true; }
        }
        public string TaskDescription {
            get { return item.Description; }
            set { item.Description = value; dirty = true; }
        }

        private bool dirty = false;
        public ICommand OK { get; private set; }

        public TaskEditViewModel(ICommonService service, TaskItem item) {
            this.item = item;
            var hash = item.GetHashCode();

            OK = new RelayCommand(() => {
                dirty = false;
                hash = item.GetHashCode() + 1; // OKを押したときは必ず保存する(コピペ用)
                service.Close();
            });

            service.AddClosing((sender, e) => {
                if (!dirty) {
                    // 確認不要
                    if (hash == item.GetHashCode()) { // 変更なし
                        item.Title = null;
                    }
                    return;
                }
                var result = service.MessageBoxShow(
                    "編集内容を保存しますか？",
                    "タスクの更新",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (MessageBoxResult.Cancel == result) {// キャンセル
                    e.Cancel = true;
                } else if (MessageBoxResult.No == result) { // 破棄
                    item.Title = null;
                }
                // それ以外(OKの時も)は何もせず閉じる
            });
        }
    }
}
