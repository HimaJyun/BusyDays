using BusyDays.Common;
using System;
using System.Collections.ObjectModel;

namespace BusyDays.Model {
    public class MainModel : NotifyBase {

        private readonly TsvSerializer<TaskItem> serializer;
        public ObservableCollection<TaskItem> TaskList { get; private set; }

        private TaskItem _task;
        public TaskItem Task {
            get { return _task; }
            set {
                _task = value;
                OnPropertyChanged(nameof(Task));
            }
        }

        public MainModel(string filePath) {
            serializer = new TsvSerializer<TaskItem>(filePath);
            TaskList = new ObservableCollection<TaskItem>(serializer.Deserialize());

            // タスクがあればロードする
            LoadTask();
            // ロード後にバックアップ
            var info = serializer.GetFileInfo();
            info.Refresh();
            if (info.Exists) { info.CopyTo(info.FullName + ".old", true); }
        }

        /// <summary>
        /// 現在のタスクを完了させる
        /// </summary>
        public void Done() {
            TaskList.RemoveAt(0);
            LoadTask();
        }

        /// <summary>
        /// 後回し
        /// </summary>
        public void Postpone() {
            if (TaskList.Count < 2) {
                // 2未満なら何もしない(何も出来ない)
                return;
            }
            TaskList.Move(0, 1);
            Task = TaskList[0];
        }

        /// <summary>
        /// タスクを削除する
        /// </summary>
        /// <param name="item">削除するタスク</param>
        public void Remove(TaskItem item) {
            // 同じ内容のアイテムがあったらバグる可能性あるのでReferenceEquals
            if (object.ReferenceEquals(item, Task)) {
                // 実質DoneなのでDoneを呼ぶ
                Done();
            }


            // 他にもいくつか方法はあったんだけど……パフォーマンスとのコスパ的にこれがベストっぽそう
            var i = ReferenceIndexOf(item);
            if (i != -1) {
                TaskList.RemoveAt(i);
            }
        }

        /// <summary>
        /// タスクを追加する
        /// </summary>
        /// <param name="item">追加するタスク</param>
        public void AddTask(TaskItem item) {
            TaskList.Add(item);
            // これが1個めなら入れておく
            if (TaskList.Count == 1) { Task = TaskList[0]; }
        }

        /// <summary>
        /// タスクを作業する
        /// </summary>
        /// <param name="item">作業するタスク</param>
        public void Work(TaskItem item) {
            var i = ReferenceIndexOf(item);
            if (i == -1) {// ここにくるのはあり得ない事なんだけどね。
                return;
            }
            TaskList.Move(i, 0);
            Task = item;
        }

        /// <summary>
        /// タスクをファイルに保存する
        /// </summary>
        public void Save() {
            serializer.Serialize(TaskList);
        }

        /// <summary>
        /// タスクリストから該当のタスクを動かします。
        /// </summary>
        /// <param name="item">動かすタスク</param>
        /// <param name="amount">動かす量</param>
        public void Move(TaskItem item, int amount) {
            var i = ReferenceIndexOf(item);
            int moveTo = i + amount;
            if (i == -1 || // 見当たらない
                moveTo < 0 || // 戻りすぎ
                moveTo >= TaskList.Count) { // 進みすぎ
                return;
            }

            TaskList.Move(i, moveTo);
            if (Task != TaskList[0]) {
                Task = TaskList[0];
            }
        }

        /// <summary>
        /// タスクリストの位置を変更します。
        /// </summary>
        /// <param name="from">変更するタスク</param>
        /// <param name="to">変更先のタスク</param>
        public void Drop(TaskItem from, TaskItem to) {
            if (from == to) {
                return;
            }

            var fromIndex = ReferenceIndexOf(from);
            var toIndex = ReferenceIndexOf(to);
            if (fromIndex == -1 || toIndex == -1) {
                return;
            }

            TaskList.RemoveAt(fromIndex);
            TaskList.Insert(toIndex, from);
            if (Task != TaskList[0]) {
                Task = TaskList[0];
            }
        }

        /// <summary>
        /// タスクを更新します
        /// </summary>
        /// <param name="oldItem">古いタスク</param>
        /// <param name="newItem">新しいタスク</param>
        public void Update(TaskItem oldItem, TaskItem newItem) {
            var index = ReferenceIndexOf(oldItem);
            if (index != -1) {
                TaskList[index] = newItem;
            }
        }

        private int ReferenceIndexOf(TaskItem item) {
            for (int i = 0, count = TaskList.Count; i < count; ++i) {
                if (object.ReferenceEquals(item, TaskList[i])) {
                    return i;
                }
            }
            return -1;
        }

        private void LoadTask() {
            Task = (TaskList.Count == 0 ? null : TaskList[0]);
        }
    }

    public class TaskItem : NotifyBase, IEquatable<TaskItem> {
        private string _title = "";
        private string _description = "";
        public string Title {
            get { return this._title; }
            set {
                this._title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Description {
            get { return this._description; }
            set {
                this._description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public TaskItem() { }
        public TaskItem(string title) {
            this.Title = title;
        }
        public TaskItem(string title, string description) : this(title) {
            this.Description = description;
        }

        #region override達
        public bool Equals(TaskItem other) {
            return (this.Title == other.Title) && (this.Description == other.Description);
        }
        public override bool Equals(object obj) {
            var o = obj as TaskItem;
            if (o == null) {
                return false;
            }
            return Equals(o);
        }
        public override int GetHashCode() {
            var i = 0;
            i ^= Title?.GetHashCode() ?? 0;
            i ^= Description?.GetHashCode() ?? 0;
            return i;
        }
        public override string ToString() {
            return Title + Environment.NewLine + Description;
        }
        public TaskItem Clone() {
            return new TaskItem(this.Title, this.Description);
        }
        #endregion
    }
}
