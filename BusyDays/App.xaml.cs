using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace BusyDays {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {
        // private staticにしないとお節介GCが勝手に持ってちゃう
        private static Mutex mutex = null;

        private void Application_Startup(object sender, StartupEventArgs e) {
            // 保存先ファイル取得
            var path = Path.Combine(Environment.CurrentDirectory, "BusyDays.tsv");
            if (0 < e.Args.Length) {
                path = e.Args[0];
            }
            path = Path.GetFullPath(path);

            // 多重起動防止 <-ハマった
            mutex = new Mutex(false, $"BusyDays_{path.GetHashCode()}");
            if (!mutex.WaitOne(0, false)) {
                MessageBox.Show(
                    $"{path} を使用するBusyDaysは既に起動しています。",
                   "既に起動しています",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                mutex.Close();
                mutex = null;
                this.Shutdown();
            }

            new View.MainView(path).Show();
            // Showはモードレスなので実行したらこの関数を抜ける
            // Application_Exitを別で用意してMutexを最後まで保持しなきゃならない
        }

        private void Application_Exit(object sender, ExitEventArgs e) {
            if (mutex != null) {
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            var nr = Environment.NewLine;
            var ex = e.Exception;
            var r = MessageBox.Show(
                "なんかよくわかんないけどバグったです。" + nr +
                    "バグった詳細をファイルに保存します？" + nr +
                    nr +
                    "ちなみに詳細は以下の通りです。" + nr +
                    $"{ex.GetType().FullName}{nr}{ex.Message}",
                "バグりましたッ！！",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Hand);
            if (r == MessageBoxResult.OK) {
                var d = new SaveFileDialog {
                    Title = "保存先のファイルを選択してください",
                    FileName = "DusyDays_Exception.log",
                    Filter = "テキストファイル(*.txt;*.log)|*.txt;*.log|すべてのファイル(*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    OverwritePrompt = true
                };
                if (d.ShowDialog() == true) {
                    WriteException(d.FileName, ex);
                }
            }
        }

        private void WriteException(string path, Exception e) {
            using (var sw = new StreamWriter(path)) {
                try {
                    Action<string> w = (m) => sw.WriteLine(m);
                    w("OS: " + Environment.OSVersion);
                    w(".Net: " + Environment.Version);
                    w("Args: " + Environment.CommandLine);
                    sw.WriteLine();
                    w("Exception: " + e.GetType().FullName);
                    w(e.Message);
                    sw.WriteLine();
                    w("StackTrace: ");
                    w(e.StackTrace);
                } catch (IOException) {
                    MessageBox.Show("ファイル書き込み中にバグりました、指定したファイルは書き込み可能ですか？", "またバグりました");
                }
            }
        }
    }
}
