// このソースコードはソフトウェア本体のライセンスに関わらす「zlib license」で自由に利用できます。
// This source code can be used freely with "zlib license" regardless of the license of the software itself.
// http://zlib.net/zlib_license.html
using System.ComponentModel;
using System.Windows;

namespace BusyDays.Common {
    /// <summary>
    /// ViewModelからView切り離し、テスト可能にするためのService
    /// </summary>
    public interface ICommonService {
        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="messageBoxText">表示するテキスト</param>
        /// <returns>ユーザーがクリックしたメッセージボックスボタン</returns>
        MessageBoxResult MessageBoxShow(string messageBoxText);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        MessageBoxResult MessageBoxShow(string messageBoxText, string caption);
        MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button);
        MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);

        /// <summary>
        /// モードレスウインドウを表示します。(モードレス：ユーザーの操作を制限しない)
        /// </summary>
        /// <typeparam name="T">表示するウインドウの型</typeparam>
        void Show<T>() where T : Window, new();
        /// <summary>
        /// モードレスウインドウを表示します。(モードレス：ユーザーの操作を制限しない)
        /// </summary>
        /// <typeparam name="T">表示するウインドウの型</typeparam>
        /// <param name="window">表示するウインドウのインスタンス</param>
        void Show<T>(T window) where T : Window;

        /// <summary>
        /// モーダルウインドウを表示します。(モーダル：ユーザーの操作を制限する)
        /// </summary>
        /// <typeparam name="T">表示するウインドウの型</typeparam>
        /// <returns>ShowDialog()の戻り値</returns>
        bool? ShowDialog<T>() where T : Window, new();
        /// <summary>
        /// モーダルウインドウを表示します。(モーダル：ユーザーの操作を制限する)
        /// </summary>
        /// <typeparam name="T">表示するウインドウの型</typeparam>
        /// <param name="window">表示するウインドウのインスタンス</param>
        /// <returns>ShowDialog()の戻り値</returns>
        bool? ShowDialog<T>(T window) where T : Window;

        /// <summary>
        /// このServiceを利用するWindowをClose()します。
        /// </summary>
        void Close();
        /// <summary>
        /// Closingイベントに処理を追加します。
        /// </summary>
        void AddClosing(CancelEventHandler handler);

        void ClipboardSet(string value);
        string ClipboardGet();
    }

    public class CommonService : ICommonService {
        private readonly Window window;

        /// <summary>
        /// ダイアログのService
        /// </summary>
        /// <param name="window">ViewModelのオーナーとなるView</param>
        public CommonService(Window window) {
            this.window = window;
        }

        public MessageBoxResult MessageBoxShow(string messageBoxText) {
            return MessageBox.Show(window, messageBoxText);
        }

        public MessageBoxResult MessageBoxShow(string messageBoxText, string caption) {
            return MessageBox.Show(window, messageBoxText, caption);
        }

        public MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button) {
            return MessageBox.Show(window, messageBoxText, caption, button);
        }

        public MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) {
            return MessageBox.Show(window, messageBoxText, caption, button, icon);
        }

        public void Show<T>() where T : Window, new() {
            this.Show(new T());
        }

        public void Show<T>(T window) where T : Window {
            window.Show();
        }

        public bool? ShowDialog<T>() where T : Window, new() {
            return this.ShowDialog(new T());
        }

        public bool? ShowDialog<T>(T window) where T : Window {
            return window.ShowDialog();
        }

        public void Close() {
            window.Close();
        }

        public void AddClosing(CancelEventHandler handler) {
            window.Closing += handler;
        }

        public void ClipboardSet(string value) {
            Clipboard.SetText(value);
        }

        public string ClipboardGet() {
            return Clipboard.GetText();
        }
    }
}
