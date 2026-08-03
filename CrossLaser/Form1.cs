using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CrossLaser
{
    public partial class Form1 : Form
    {
        // 直接 P/Invoke WinDivert API
        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr WinDivertOpen(string filter, int layer, int priority, ulong flags);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WinDivertRecv(IntPtr handle, byte[] pPacket, uint packetLen, ref uint readLen, IntPtr pAddr);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WinDivertSend(IntPtr handle, byte[] pPacket, uint packetLen, ref uint sendLen, IntPtr pAddr);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WinDivertClose(IntPtr handle);

        // 添加 LoadLibrary 和 FreeLibrary 用于手动控制 DLL 加载/卸载
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private Thread? _workerThread;
        private CancellationTokenSource? _cancellationTokenSource;
        private IntPtr _divertHandle = IntPtr.Zero;
        private IntPtr _dllHandle = IntPtr.Zero;  // 记录 DLL 句柄

        private long _droppedCount = 0;
        private System.Windows.Forms.Timer? _updateTimer;
        private long _displayCount = 0;

        private string _dllPath = string.Empty;
        private bool _isDllLoaded = false;

        public Form1()
        {
            InitializeComponent();

            // 设置窗口为固定大小
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 1000;
            _updateTimer.Tick += UpdateTimer_Tick;

            ExtractDllToAppDirectory();
        }

        private void ExtractDllToAppDirectory()
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _dllPath = Path.Combine(appDirectory, "WinDivert.dll");

                if (File.Exists(_dllPath))
                {
                    File.Delete(_dllPath);
                }

                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] resourceNames = assembly.GetManifestResourceNames();
                string? resourceName = null;

                foreach (string name in resourceNames)
                {
                    if (name.Contains("WinDivert.dll") || name.EndsWith("WinDivert.dll"))
                    {
                        resourceName = name;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(resourceName))
                {
                    MessageBox.Show("未找到 WinDivert.dll 资源！", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        MessageBox.Show("无法读取嵌入的 WinDivert.dll！", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (FileStream fileStream = File.Create(_dllPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                if (File.Exists(_dllPath))
                {
                    // 不自动加载 DLL，等点击"开始"时再加载
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"释放 WinDivert.dll 失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            long currentCount = Interlocked.Read(ref _droppedCount);
            if (currentCount != _displayCount)
            {
                _displayCount = currentCount;
                UpdateStatus($"已丢弃数据包: {_displayCount:N0}");
            }
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_dllPath))
            {
                MessageBox.Show("WinDivert.dll 不存在，请重新启动程序！", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_workerThread == null || !_workerThread.IsAlive)
            {
                StartDropping();
                btnToggle.Text = "恢复网络";
                btnToggle.BackColor = System.Drawing.Color.LightGreen;
                _updateTimer.Start();
                UpdateStatus("正在拦截并丢弃所有外发数据包...");
            }
            else
            {
                StopDropping();
                btnToggle.Text = "阻止联网";
                btnToggle.BackColor = System.Drawing.SystemColors.Control;
                _updateTimer.Stop();
                UpdateStatus($"已停止，共丢弃 {_displayCount:N0} 个数据包。");
            }
        }

        private void StartDropping()
        {
            Interlocked.Exchange(ref _droppedCount, 0);
            _displayCount = 0;

            // 加载 DLL（如果尚未加载）
            if (!_isDllLoaded)
            {
                LoadDll();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _workerThread = new Thread(() => DropPacketsLoop(_cancellationTokenSource.Token));
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        /// <summary>
        /// 加载 WinDivert.dll
        /// </summary>
        private void LoadDll()
        {
            try
            {
                if (_dllHandle == IntPtr.Zero)
                {
                    _dllHandle = LoadLibrary(_dllPath);
                    if (_dllHandle == IntPtr.Zero)
                    {
                        throw new Exception($"加载 DLL 失败，错误代码: {Marshal.GetLastWin32Error()}");
                    }
                    _isDllLoaded = true;
                    Console.WriteLine("WinDivert.dll 已加载");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载 WinDivert.dll 失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 卸载 WinDivert.dll
        /// </summary>
        private void UnloadDll()
        {
            try
            {
                if (_dllHandle != IntPtr.Zero)
                {
                    bool result = FreeLibrary(_dllHandle);
                    if (result)
                    {
                        _dllHandle = IntPtr.Zero;
                        _isDllLoaded = false;
                        Console.WriteLine("WinDivert.dll 已卸载");
                    }
                    else
                    {
                        Console.WriteLine($"卸载 DLL 失败，错误代码: {Marshal.GetLastWin32Error()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"卸载 DLL 异常: {ex.Message}");
            }
        }

        private void StopDropping()
        {
            try
            {
                // 1. 取消线程
                _cancellationTokenSource?.Cancel();

                // 2. 等待线程结束
                if (_workerThread != null)
                {
                    _workerThread.Join(2000); // 等待最多2秒
                    _workerThread = null;
                }

                // 3. 关闭 WinDivert 句柄
                if (_divertHandle != IntPtr.Zero)
                {
                    WinDivertClose(_divertHandle);
                    _divertHandle = IntPtr.Zero;
                    Console.WriteLine("WinDivert 句柄已关闭");
                }

                // 4. 给系统一点时间释放资源
                Thread.Sleep(100);

                // 5. 卸载 DLL
                UnloadDll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"停止丢包时出错: {ex.Message}");
            }
        }

        private void DropPacketsLoop(CancellationToken ct)
        {
            try
            {
                _divertHandle = WinDivertOpen("outbound", 0, 0, 0);

                if (_divertHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    UpdateStatus($"无法打开 WinDivert，错误代码: {errorCode}，请以管理员身份运行！");
                    return;
                }

                byte[] packetBuffer = new byte[65535];
                uint packetLength = 0;
                IntPtr pAddress = IntPtr.Zero;

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        bool success = WinDivertRecv(_divertHandle, packetBuffer, (uint)packetBuffer.Length, ref packetLength, pAddress);

                        if (success)
                        {
                            Interlocked.Increment(ref _droppedCount);
                        }
                    }
                    catch
                    {
                        // 忽略错误
                    }
                }

                // 循环结束，关闭句柄
                if (_divertHandle != IntPtr.Zero)
                {
                    WinDivertClose(_divertHandle);
                    _divertHandle = IntPtr.Zero;
                    Console.WriteLine("WinDivert 句柄已关闭（循环结束）");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"初始化失败: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = message));
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        /// <summary>
        /// 强制清理，确保 DLL 被删除
        /// </summary>
        private void ForceCleanup()
        {
            try
            {
                // 1. 停止丢包
                StopDropping();

                // 2. 强制垃圾回收，确保所有资源被释放
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // 3. 等待系统释放文件句柄
                Thread.Sleep(500);

                // 4. 删除 DLL 文件
                if (!string.IsNullOrEmpty(_dllPath) && File.Exists(_dllPath))
                {
                    // 尝试重置文件属性
                    File.SetAttributes(_dllPath, FileAttributes.Normal);

                    // 尝试删除
                    File.Delete(_dllPath);
                    Console.WriteLine($"WinDivert.dll 已删除");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除 DLL 失败: {ex.Message}");

                // 如果删除失败，使用 cmd 延迟删除
                try
                {
                    string cmd = $"cmd.exe /c del /f /q \"{_dllPath}\"";
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c choice /C Y /N /D Y /T 1 & del /f /q \"{_dllPath}\"",
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    });
                    Console.WriteLine("已启动延迟删除任务");
                }
                catch { }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ForceCleanup();
            base.OnFormClosing(e);
        }
    }
}