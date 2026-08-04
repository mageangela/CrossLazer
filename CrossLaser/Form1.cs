using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CrossLaser
{
    public partial class Form1 : Form
    {
        // WinDivert API
        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr WinDivertOpen(string filter, int layer, int priority, ulong flags);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WinDivertRecv(IntPtr handle, byte[] pPacket, uint packetLen, ref uint readLen, IntPtr pAddr);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WinDivertSend(IntPtr handle, byte[] pPacket, uint packetLen, ref uint sendLen, IntPtr pAddr);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WinDivertClose(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        // 用于驱动管理
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName,
            int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl,
            string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId,
            string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DeleteService(IntPtr hService);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool ControlService(IntPtr hService, int dwControl, ref ServiceStatus lpServiceStatus);

        [StructLayout(LayoutKind.Sequential)]
        private struct ServiceStatus
        {
            public int dwServiceType;
            public int dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }

        // 服务相关常量
        private const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        private const int SERVICE_ALL_ACCESS = 0xF01FF;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        private const int SERVICE_DEMAND_START = 0x00000003;
        private const int SERVICE_ERROR_NORMAL = 0x00000001;
        private const int SERVICE_CONTROL_STOP = 0x00000001;
        private const int SERVICE_RUNNING = 0x00000004;
        private bool _isTcpOnlyMode = false;

        private const string WIN_DIVERT_SERVICE_NAME = "WinDivert1.4";

        private Thread? _workerThread;
        private CancellationTokenSource? _cancellationTokenSource;
        private IntPtr _divertHandle = IntPtr.Zero;
        private IntPtr _dllHandle = IntPtr.Zero;

        private long _droppedCount = 0;
        private System.Windows.Forms.Timer? _updateTimer;
        private long _displayCount = 0;

        private string _dllPath = string.Empty;
        private string _sysPath = string.Empty;
        private bool _isDllLoaded = false;

        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 1000;
            _updateTimer.Tick += UpdateTimer_Tick;

            // 先提取所有文件
            ExtractAllFiles();
        }

        /// <summary>
        /// 提取所有需要的文件（DLL 和 SYS）
        /// </summary>
        private void ExtractAllFiles()
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _dllPath = Path.Combine(appDirectory, "WinDivert.dll");
                _sysPath = Path.Combine(appDirectory, "WinDivert64.sys");

                // 提取 DLL
                ExtractResource("WinDivert.dll", _dllPath);
                // 提取 SYS
                ExtractResource("WinDivert64.sys", _sysPath);

                UpdateStatus("文件已释放，准备就绪");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"释放文件失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 提取资源文件到目标路径（如果文件已存在则跳过）
        /// </summary>
        private void ExtractResource(string fileName, string targetPath)
        {
            try
            {
                // 检查文件是否已存在
                if (File.Exists(targetPath))
                {
                    // 文件已存在，验证文件是否完整（检查大小是否大于0）
                    FileInfo fileInfo = new FileInfo(targetPath);
                    if (fileInfo.Length > 0)
                    {
                        // 文件存在且有效，跳过释放
                        return;
                    }
                    else
                    {
                        // 文件存在但为空，删除后重新释放
                        try
                        {
                            File.Delete(targetPath);
                        }
                        catch
                        {
                            // 如果删除失败，说明文件被占用，跳过
                            return;
                        }
                    }
                }

                // 如果文件不存在或已被删除，从资源中释放
                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] resourceNames = assembly.GetManifestResourceNames();
                string? resourceName = null;

                foreach (string name in resourceNames)
                {
                    if (name.Contains(fileName) || name.EndsWith(fileName))
                    {
                        resourceName = name;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(resourceName))
                {
                    throw new Exception($"未找到资源: {fileName}");
                }

                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        throw new Exception($"无法读取资源: {resourceName}");
                    }

                    using (FileStream fileStream = File.Create(targetPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"提取 {fileName} 失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 安装并启动 WinDivert 驱动
        /// </summary>
        private bool InstallAndStartDriver()
        {
            try
            {
                // 1. 检查驱动是否已经安装并运行
                if (IsDriverRunning())
                {
                    UpdateStatus("WinDivert 驱动已运行");
                    return true;
                }

                UpdateStatus("正在安装 WinDivert 驱动...");

                // 2. 将 SYS 文件复制到系统目录
                string systemDriverPath = Path.Combine(Environment.SystemDirectory, "drivers", "WinDivert64.sys");

                // 如果系统目录已有驱动，先删除
                if (File.Exists(systemDriverPath))
                {
                    try
                    {
                        File.Delete(systemDriverPath);
                    }
                    catch
                    {
                        // 可能正在使用，尝试使用当前目录的驱动
                        systemDriverPath = _sysPath;
                    }
                }
                else
                {
                    // 复制驱动到系统目录
                    File.Copy(_sysPath, systemDriverPath, true);
                }

                // 3. 创建并启动服务
                IntPtr scManager = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (scManager == IntPtr.Zero)
                {
                    throw new Exception("无法打开服务管理器");
                }

                try
                {
                    // 检查服务是否已存在
                    IntPtr service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);

                    if (service != IntPtr.Zero)
                    {
                        // 服务已存在，尝试启动
                        CloseServiceHandle(service);

                        // 尝试启动服务
                        service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                        if (service != IntPtr.Zero)
                        {
                            ServiceStatus status = new ServiceStatus();
                            if (StartService(service, 0, null))
                            {
                                CloseServiceHandle(service);
                                CloseServiceHandle(scManager);
                                UpdateStatus("WinDivert 驱动已启动");
                                return true;
                            }
                            CloseServiceHandle(service);
                        }

                        // 如果启动失败，删除旧服务重新创建
                        service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                        if (service != IntPtr.Zero)
                        {
                            DeleteService(service);
                            CloseServiceHandle(service);
                        }
                    }

                    // 创建新服务
                    service = CreateService(
                        scManager,
                        WIN_DIVERT_SERVICE_NAME,
                        "WinDivert Service",
                        SERVICE_ALL_ACCESS,
                        SERVICE_WIN32_OWN_PROCESS,
                        SERVICE_DEMAND_START,
                        SERVICE_ERROR_NORMAL,
                        systemDriverPath,
                        null,
                        IntPtr.Zero,
                        null,
                        null,
                        null
                    );

                    if (service == IntPtr.Zero)
                    {
                        throw new Exception($"创建服务失败，错误代码: {Marshal.GetLastWin32Error()}");
                    }

                    // 启动服务 - 使用不同的变量名避免冲突
                    ServiceStatus serviceStatus = new ServiceStatus();
                    if (!StartService(service, 0, null))
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error != 1056) // 1056 = 服务已在运行
                        {
                            throw new Exception($"启动服务失败，错误代码: {error}");
                        }
                    }

                    CloseServiceHandle(service);
                    CloseServiceHandle(scManager);

                    UpdateStatus("WinDivert 驱动安装并启动成功");
                    return true;
                }
                finally
                {
                    CloseServiceHandle(scManager);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"驱动安装失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查 WinDivert 驱动是否正在运行
        /// </summary>
        private bool IsDriverRunning()
        {
            try
            {
                // 尝试打开一个临时句柄来测试驱动是否可用
                IntPtr testHandle = WinDivertOpen("false", 0, 0, 0);
                if (testHandle != IntPtr.Zero)
                {
                    WinDivertClose(testHandle);
                    return true;
                }

                // 如果打开失败，检查服务状态
                IntPtr scManager = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (scManager == IntPtr.Zero)
                {
                    return false;
                }

                try
                {
                    IntPtr service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                    if (service == IntPtr.Zero)
                    {
                        return false;
                    }

                    ServiceStatus status = new ServiceStatus();
                    if (ControlService(service, SERVICE_CONTROL_STOP, ref status))
                    {
                        CloseServiceHandle(service);
                        return status.dwCurrentState == SERVICE_RUNNING;
                    }

                    CloseServiceHandle(service);
                    return false;
                }
                finally
                {
                    CloseServiceHandle(scManager);
                }
            }
            catch
            {
                return false;
            }
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            long currentCount = Interlocked.Read(ref _droppedCount);
            if (currentCount != _displayCount)
            {
                _displayCount = currentCount;
                UpdateStatus($"丢弃数据包状态: {_displayCount:N0}");
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
                // 读取复选框状态 - 在启动前读取
                _isTcpOnlyMode = chkOnlyTCP.Checked;

                // 禁用复选框（运行时不允许更改模式）
                chkOnlyTCP.Enabled = false;

                // 在启动前，确保驱动已安装并运行
                if (!IsDriverRunning())
                {
                    if (!InstallAndStartDriver())
                    {
                        MessageBox.Show("无法安装或启动 WinDivert 驱动！\n请确保以管理员身份运行。",
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 等待驱动初始化
                    Thread.Sleep(500);
                }

                StartDropping();
                btnToggle.Text = "恢复网络";
                btnToggle.BackColor = System.Drawing.Color.DarkRed;
                btnToggle.ForeColor = System.Drawing.Color.White;
                _updateTimer.Start();
                UpdateStatus("等待数据包到达...");
            }
            else
            {
                StopDropping();
                btnToggle.Text = "阻止联网";
                btnToggle.BackColor = System.Drawing.Color.WhiteSmoke;
                btnToggle.ForeColor = System.Drawing.Color.DarkRed;
                _updateTimer.Stop();
                UpdateStatus($"已停止，丢弃数据包状态 {_displayCount:N0} 。");
            }
        }

        private void StartDropping()
        {
            Interlocked.Exchange(ref _droppedCount, 0);
            _displayCount = 0;

            if (!_isDllLoaded)
            {
                LoadDll();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _workerThread = new Thread(() => DropPacketsLoop(_cancellationTokenSource.Token));
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载 WinDivert.dll 失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnloadDll()
        {
            try
            {
                if (_dllHandle != IntPtr.Zero)
                {
                    FreeLibrary(_dllHandle);
                    _dllHandle = IntPtr.Zero;
                    _isDllLoaded = false;
                }
            }
            catch { }
        }

        private void StopDropping()
        {
            try
            {
                _cancellationTokenSource?.Cancel();

                if (_workerThread != null)
                {
                    _workerThread.Join(2000);
                    _workerThread = null;
                }

                if (_divertHandle != IntPtr.Zero)
                {
                    WinDivertClose(_divertHandle);
                    _divertHandle = IntPtr.Zero;
                }

                Thread.Sleep(100);
                UnloadDll();

                // 确保复选框被启用（防止异常退出后复选框仍被禁用）
                if (chkOnlyTCP.InvokeRequired)
                {
                    chkOnlyTCP.Invoke(new Action(() => chkOnlyTCP.Enabled = true));
                }
                else
                {
                    chkOnlyTCP.Enabled = true;
                }
            }
            catch { }
        }

        private void DropPacketsLoop(CancellationToken ct)
        {
            try
            {
                // 根据模式构建过滤器
                string filter;
                if (_isTcpOnlyMode)
                {
                    // 仅TCP模式：只捕获TCP数据包
                    filter = "tcp";
                }
                else
                {
                    // 全部模式：捕获所有出站数据包
                    filter = "outbound";
                }

                _divertHandle = WinDivertOpen(filter, 0, 0, 0);

                if (_divertHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    UpdateStatus($"无法打开 WinDivert，错误代码: {errorCode}");

                    if (InstallAndStartDriver())
                    {
                        Thread.Sleep(500);
                        _divertHandle = WinDivertOpen(filter, 0, 0, 0);

                        if (_divertHandle == IntPtr.Zero)
                        {
                            UpdateStatus($"重新安装后仍无法打开，错误代码: {Marshal.GetLastWin32Error()}");
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
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

                if (_divertHandle != IntPtr.Zero)
                {
                    WinDivertClose(_divertHandle);
                    _divertHandle = IntPtr.Zero;
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

        private void ForceCleanup()
        {
            try
            {
                StopDropping();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Thread.Sleep(500);

                // 删除释放的文件
                foreach (string file in new[] { _dllPath, _sysPath })
                {
                    if (!string.IsNullOrEmpty(file) && File.Exists(file))
                    {
                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ForceCleanup();
            base.OnFormClosing(e);
        }
    }
}