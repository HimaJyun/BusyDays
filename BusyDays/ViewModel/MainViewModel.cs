using BusyDays.Common;
using BusyDays.Model;
using BusyDays.View;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BusyDays.ViewModel {
    class MainViewModel : NotifyBase {
        private readonly MainModel model;
        private readonly ICommonService service;

        public MainViewModel(ICommonService dialog, string path) {
            this.service = dialog;

            model = new MainModel(path);
            InitCommand();

            model.PropertyChanged += (sender, e) => {
                OnPropertyChanged(
                    nameof(TaskTitle), nameof(TaskDescription),
                    nameof(TaskExist), nameof(TaskNotExist)
                    );
            };
        }

        public ObservableCollection<TaskItem> TaskList => model.TaskList;
        private TaskItem _selectedTask;
        public TaskItem SelectedTask {
            get { return _selectedTask; }
            set { this._selectedTask = value; OnPropertyChanged(nameof(SelectedTask)); }
        }

        public bool TaskExist => model.Task == null;
        public bool TaskNotExist => !TaskExist;

        public string TaskTitle {
            get { return TaskExist ? "タスクがありません" : model.Task.Title; }
            set { model.Task.Title = value; }
        }

        public string TaskDescription {
            get { return TaskExist ? "新しいタスクを登録しましょう！" : model.Task.Description; }
            set { model.Task.Description = value; }
        }

        public ICommand TaskDone { get; private set; }
        public ICommand TaskPostpone { get; private set; }
        public ICommand TaskAdd { get; private set; }
        public ICommand TaskRemove { get; private set; }
        public ICommand TaskUpdate { get; set; }
        public ICommand TaskCopy { get; private set; }
        public ICommand TaskPaste { get; set; }
        public ICommand TaskDoubleClick { get; set; }
        public ICommand TaskUp { get; set; }
        public ICommand TaskDown { get; set; }
        public ICommand TaskWork { get; set; }
        public ICommand TaskSave { get; set; }

        private void InitCommand() {
            TaskDone = new RelayCommand(() => {
                var result = service.MessageBoxShow(
                    $"現在実行中のタスク「{TaskTitle}」を完了しますか？",
                    "タスクを完了しますか？",
                    MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK) {
                    model.Done();
                }
            });

            TaskPostpone = new RelayCommand(() => {
                model.Postpone();
            });

            TaskAdd = new RelayCommand(() => {
                var task = new TaskItem();
                service.ShowDialog(new TaskAddView(task));
                if (task.Title != null) {
                    model.AddTask(task);
                }
            });

            TaskRemove = new RelayCommand(() => {
                if (SelectedTask == null) {
                    return;
                }
                var result = service.MessageBoxShow(
                    $"選択中のタスク「{SelectedTask.Title}」を削除しますか？",
                    "確認",
                    MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK) {
                    model.Remove(SelectedTask);
                }
            });

            TaskUpdate = new RelayCommand(() => {
                var task = SelectedTask.Clone();
                service.ShowDialog(new TaskAddView(task));
                if (task.Title != null) {
                    model.Update(SelectedTask, task);
                }
            });

            TaskCopy = new RelayCommand(() => { service.ClipboardSet(SelectedTask.ToString()); });
            TaskPaste = new RelayCommand(() => {
                // クリップボードの中身を取得して整える
                var array = service.ClipboardGet()
                    .Replace("\r", "")
                    .Replace("\n", Environment.NewLine)
                    .Split(new string[] { Environment.NewLine }, 2, StringSplitOptions.None);
                var task = new TaskItem(array[0]);
                if (array.Length > 1) {
                    task.Description = array[1];
                }
                service.ShowDialog(new TaskAddView(task));
                if (task.Title != null) {
                    model.AddTask(task);
                }
            });

            TaskDoubleClick = new RelayCommand<TaskItem>(item => {
                model.Work(item);
            });

            TaskUp = new RelayCommand(() => { if (SelectedTask != null) { model.Move(SelectedTask, -1); } });
            TaskDown = new RelayCommand(() => { if (SelectedTask != null) { model.Move(SelectedTask, +1); } });
            TaskWork = new RelayCommand(() => { if (SelectedTask != null) { model.Work(SelectedTask); } });

            TaskSave = new RelayCommand(() => { model.Save(); });
            service.AddClosing((sender, e) => { model.Save(); });
        }

        public void TaskDrop(TaskItem from, TaskItem to) {
            model.Drop(from, to);
        }
    }
}
