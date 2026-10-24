using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Media;

namespace CrossLaser
{
    public partial class Form1 : Form
    {
        // ==================== WinDivert API ====================
        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr WinDivertOpen(string filter, int layer, int priority, ulong flags);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern bool WinDivertRecv(
            IntPtr handle, byte[] pPacket, uint packetLen, out uint readLen, IntPtr pAddr);

        [DllImport("WinDivert.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern bool WinDivertClose(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        // ==================== 服务管理 API ====================
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

        // ==================== RawInput API ====================
        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        private static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        private const int WM_INPUT = 0x00FF;
        private const uint RID_INPUT = 0x10000003;
        private const uint RIDEV_INPUTSINK = 0x00000100;
        private const ushort HID_USAGE_PAGE_GENERIC = 0x01;
        private const ushort HID_USAGE_GENERIC_KEYBOARD = 0x06;

        // ==================== 常量 ====================
        private const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        private const int SERVICE_ALL_ACCESS = 0xF01FF;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        private const int SERVICE_DEMAND_START = 0x00000003;
        private const int SERVICE_ERROR_NORMAL = 0x00000001;
        private const int SERVICE_CONTROL_STOP = 0x00000001;
        private const int SERVICE_RUNNING = 0x00000004;
        private const string WIN_DIVERT_SERVICE_NAME = "WinDivert1.4";

        // ==================== 状态字段 ====================
        private Thread? _workerThread;
        private CancellationTokenSource? _cancellationTokenSource;
        private IntPtr _divertHandle = IntPtr.Zero;
        private IntPtr _dllHandle = IntPtr.Zero;

        private long _recvCount = 0;
        private long _droppedCount = 0;

        private System.Windows.Forms.Timer? _updateTimer;
        private long _lastRecv = -1, _lastDropped = -1;

        private string _dllPath = string.Empty;
        private string _sysPath = string.Empty;
        private bool _isDllLoaded = false;

        private bool _isTcpOnlyMode = false;
        private bool _isUdpOnlyMode = false;
        private bool _isCloudSaveMode = false;

        private Keys _selectedHotkey = Keys.F10;
        private bool _isBlocking = false;
        private DateTime _lastHotkeyTime = DateTime.MinValue;
        private bool _suppressCheckEvents = false;

        private string _tradeServer1IP = "192.81.245.200";
        private string _tradeServer2IP = "192.81.245.201";
        private string _cloudSaveIP = "192.81.241.171";

        public Form1()
        {
            InitializeComponent();

            this.comboBox热键.SelectedIndex = 9;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 1000;
            _updateTimer.Tick += UpdateTimer_Tick;

            comboBox热键.SelectedIndexChanged += comboBox热键_SelectedIndexChanged;
            WireModeCheckBoxes();
            ExtractAllFiles();
            ResolveServerIPsAsync();
        }

        // ==================== 复选框互斥逻辑 ====================
        private void WireModeCheckBoxes()
        {
            var modeGroup = new[] { checkBoxTCP, checkBoxUDP, checkBox存档 };
            foreach (var cb in modeGroup)
            {
                cb.CheckedChanged += (s, e) =>
                {
                    if (_suppressCheckEvents || !cb.Checked) return;
                    _suppressCheckEvents = true;
                    try
                    {
                        foreach (var other in modeGroup)
                            if (other != cb) other.Checked = false;
                    }
                    finally { _suppressCheckEvents = false; }
                };
            }
        }

        // ==================== DNS ====================
        private async void ResolveServerIPsAsync()
        {
            _tradeServer1IP = await ResolveWithFallbackAsync("prod.p01sjc.pod.rockstargames.com", "192.81.245.200");
            _tradeServer2IP = await ResolveWithFallbackAsync("prod.p02sjc.pod.rockstargames.com", "192.81.245.201");
            _cloudSaveIP = await ResolveWithFallbackAsync("cs-gta5-prod.ros.rockstargames.com", "192.81.241.171");
        }

        private async Task<string> ResolveWithFallbackAsync(string host, string fallback)
        {
            try
            {
                var dnsTask = Dns.GetHostAddressesAsync(host);
                var completed = await Task.WhenAny(dnsTask, Task.Delay(2000));
                if (completed == dnsTask)
                {
                    var addrs = await dnsTask;
                    var ipv4 = addrs?.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ipv4 != null) return ipv4.ToString();
                }
            }
            catch { }
            return fallback;
        }

        // ==================== 文件释放 ====================
        private void ExtractAllFiles()
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _dllPath = Path.Combine(appDirectory, "WinDivert.dll");
                _sysPath = Path.Combine(appDirectory, "WinDivert64.sys");
                ExtractResource("WinDivert.dll", _dllPath);
                ExtractResource("WinDivert64.sys", _sysPath);
                UpdateStatus("文件已释放，准备就绪");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"释放文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractResource(string fileName, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                if (new FileInfo(targetPath).Length > 0) return;
                try { File.Delete(targetPath); } catch { return; }
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string? resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.Contains(fileName) || n.EndsWith(fileName));
            if (string.IsNullOrEmpty(resourceName)) throw new Exception($"未找到资源: {fileName}");

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) throw new Exception($"无法读取资源: {resourceName}");
            using FileStream fs = File.Create(targetPath);
            stream.CopyTo(fs);
        }

        // ==================== 驱动 ====================
        private bool InstallAndStartDriver()
        {
            try
            {
                if (IsDriverRunning()) { UpdateStatus("WinDivert 驱动已运行"); return true; }
                UpdateStatus("正在安装 WinDivert 驱动...");
                string systemDriverPath = Path.Combine(Environment.SystemDirectory, "drivers", "WinDivert64.sys");

                if (File.Exists(systemDriverPath))
                {
                    try { File.Delete(systemDriverPath); }
                    catch { systemDriverPath = _sysPath; }
                }
                else { File.Copy(_sysPath, systemDriverPath, true); }

                IntPtr scManager = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (scManager == IntPtr.Zero) throw new Exception("无法打开服务管理器");

                try
                {
                    IntPtr service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                    if (service != IntPtr.Zero)
                    {
                        CloseServiceHandle(service);
                        service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                        if (service != IntPtr.Zero)
                        {
                            if (StartService(service, 0, null))
                            {
                                CloseServiceHandle(service);
                                UpdateStatus("WinDivert 驱动已启动");
                                return true;
                            }
                            CloseServiceHandle(service);
                        }
                        service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                        if (service != IntPtr.Zero) { DeleteService(service); CloseServiceHandle(service); }
                    }

                    service = CreateService(scManager, WIN_DIVERT_SERVICE_NAME, "WinDivert Service",
                        SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_DEMAND_START, SERVICE_ERROR_NORMAL,
                        systemDriverPath, null, IntPtr.Zero, null, null, null);

                    if (service == IntPtr.Zero)
                        throw new Exception($"创建服务失败，错误代码: {Marshal.GetLastWin32Error()}");

                    if (!StartService(service, 0, null))
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error != 1056) throw new Exception($"启动服务失败，错误代码: {error}");
                    }

                    CloseServiceHandle(service);
                    UpdateStatus("WinDivert 驱动安装并启动成功");
                    return true;
                }
                finally { CloseServiceHandle(scManager); }
            }
            catch (Exception ex)
            {
                UpdateStatus($"驱动安装失败: {ex.Message}");
                return false;
            }
        }

        private bool IsDriverRunning()
        {
            try
            {
                IntPtr testHandle = WinDivertOpen("false", 0, 0, 0);
                if (testHandle != IntPtr.Zero) { WinDivertClose(testHandle); return true; }

                IntPtr scManager = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (scManager == IntPtr.Zero) return false;

                try
                {
                    IntPtr service = OpenService(scManager, WIN_DIVERT_SERVICE_NAME, SERVICE_ALL_ACCESS);
                    if (service == IntPtr.Zero) return false;

                    ServiceStatus status = new ServiceStatus();
                    bool ok = ControlService(service, SERVICE_CONTROL_STOP, ref status);
                    CloseServiceHandle(service);
                    return ok && status.dwCurrentState == SERVICE_RUNNING;
                }
                finally { CloseServiceHandle(scManager); }
            }
            catch { return false; }
        }

        // ==================== 状态栏刷新 ====================
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            long recv = Interlocked.Read(ref _recvCount);
            long dropped = Interlocked.Read(ref _droppedCount);

            if (recv != _lastRecv || dropped != _lastDropped)
            {
                _lastRecv = recv;
                _lastDropped = dropped;

                UpdateStatus($"丢包中：接收 {recv:N0} / 丢弃 {dropped:N0}");
            }
        }

        private void UpdateStatus(string message)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => lblStatus.Text = message));
            else
                lblStatus.Text = message;
        }

        // ==================== 主按钮 ====================
        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_dllPath))
            {
                MessageBox.Show("WinDivert.dll 不存在，请重新启动程序！", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_isBlocking)
            {
                _isTcpOnlyMode = checkBoxTCP.Checked;
                _isUdpOnlyMode = checkBoxUDP.Checked;
                _isCloudSaveMode = checkBox存档.Checked;
                _isBlocking = true;

                SetSettingsEnabled(false);

                if (!IsDriverRunning())
                {
                    if (!InstallAndStartDriver())
                    {
                        MessageBox.Show("无法安装或启动 WinDivert 驱动！\n请确保以管理员身份运行。",
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _isBlocking = false;
                        SetSettingsEnabled(true);
                        return;
                    }
                    Thread.Sleep(500);
                }

                StartDropping();
                btnToggle.Text = "恢复网络";
                btnToggle.BackColor = System.Drawing.Color.DarkRed;
                btnToggle.ForeColor = System.Drawing.Color.White;
                _updateTimer.Start();
                UpdateStatus("已启动（丢包模式），等待数据包...");
                PlaySound("on.wav");
            }
            else
            {
                StopDropping();
                _isBlocking = false;
                btnToggle.Text = "阻止联网";
                btnToggle.BackColor = System.Drawing.Color.WhiteSmoke;
                btnToggle.ForeColor = System.Drawing.Color.DarkRed;
                _updateTimer.Stop();
                SetSettingsEnabled(true);
                UpdateStatus($"已停止，共丢弃 {_lastDropped:N0} 个数据包。");
                PlaySound("off.wav");
            }
        }

        private void SetSettingsEnabled(bool enabled)
        {
            checkBoxTCP.Enabled = enabled;
            checkBoxUDP.Enabled = enabled;
            checkBox存档.Enabled = enabled;
            comboBox热键.Enabled = enabled;
        }

        private void PlaySound(string resourceSuffix)
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string? name = asm.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));
                if (name == null) return;
                using Stream? s = asm.GetManifestResourceStream(name);
                if (s == null) return;
                using var player = new SoundPlayer(s);
                player.Play();
            }
            catch { }
        }

        // ==================== 启动/停止 ====================
        private void StartDropping()
        {
            Interlocked.Exchange(ref _recvCount, 0);
            Interlocked.Exchange(ref _droppedCount, 0);
            _lastRecv = -1; _lastDropped = -1;

            if (!_isDllLoaded) LoadDll();
            _cancellationTokenSource = new CancellationTokenSource();
            _workerThread = new Thread(() => PacketLoop(_cancellationTokenSource.Token)) { IsBackground = true };
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
                        throw new Exception($"加载 DLL 失败，错误代码: {Marshal.GetLastWin32Error()}");
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
                if (_workerThread != null) { _workerThread.Join(2000); _workerThread = null; }
                if (_divertHandle != IntPtr.Zero)
                {
                    WinDivertClose(_divertHandle);
                    _divertHandle = IntPtr.Zero;
                }
                Thread.Sleep(100);
                UnloadDll();
            }
            catch { }
        }

        // ==================== 核心循环（纯丢包） ====================
        private void PacketLoop(CancellationToken ct)
        {
            IntPtr pAddr = Marshal.AllocHGlobal(80);
            try
            {
                Marshal.Copy(new byte[80], 0, pAddr, 80);

                string filter = BuildFilter();
                _divertHandle = WinDivertOpen(filter, 0, 0, 0);
                if (_divertHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    UpdateStatus($"无法打开 WinDivert，错误代码: {errorCode}");
                    if (InstallAndStartDriver())
                    {
                        Thread.Sleep(500);
                        _divertHandle = WinDivertOpen(filter, 0, 0, 0);
                    }
                    if (_divertHandle == IntPtr.Zero)
                    {
                        UpdateStatus($"重新安装后仍无法打开，错误代码: {Marshal.GetLastWin32Error()}");
                        return;
                    }
                }

                byte[] packetBuffer = new byte[65535];

                while (!ct.IsCancellationRequested)
                {
                    if (!WinDivertRecv(_divertHandle, packetBuffer,
                            (uint)packetBuffer.Length, out uint readLen, pAddr))
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    Interlocked.Increment(ref _recvCount);
                    Interlocked.Increment(ref _droppedCount);

                    // 纯丢包模式：接收到包后直接进入下一次循环，不调用 WinDivertSend 即为丢弃
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"运行异常: {ex.Message}");
            }
            finally
            {
                Marshal.FreeHGlobal(pAddr);
            }
        }

        private string BuildFilter()
        {
            if (_isCloudSaveMode)
                return $"outbound and ip and (ip.DstAddr == {_tradeServer1IP} or ip.DstAddr == {_tradeServer2IP} or ip.DstAddr == {_cloudSaveIP})";
            if (_isTcpOnlyMode) return "outbound and tcp";
            if (_isUdpOnlyMode) return "outbound and udp";
            return "outbound";
        }

        // ==================== RawInput 热键 ====================
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegisterHotkeyRawInput();
        }

        private void RegisterHotkeyRawInput()
        {
            var rid = new RAWINPUTDEVICE[1];
            rid[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            rid[0].usUsage = HID_USAGE_GENERIC_KEYBOARD;
            rid[0].dwFlags = RIDEV_INPUTSINK;
            rid[0].hwndTarget = this.Handle;

            if (!RegisterRawInputDevices(rid, 1, (uint)Marshal.SizeOf<RAWINPUTDEVICE>()))
                Debug.WriteLine($"RawInput 注册失败: {Marshal.GetLastWin32Error()}");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                try
                {
                    uint size = 0;
                    GetRawInputData(m.LParam, RID_INPUT, IntPtr.Zero, ref size,
                        (uint)Marshal.SizeOf<RAWINPUTHEADER>());
                    if (size > 0)
                    {
                        IntPtr buffer = Marshal.AllocHGlobal((int)size);
                        try
                        {
                            if (GetRawInputData(m.LParam, RID_INPUT, buffer, ref size,
                                    (uint)Marshal.SizeOf<RAWINPUTHEADER>()) == size)
                            {
                                var header = Marshal.PtrToStructure<RAWINPUTHEADER>(buffer);
                                if (header.dwType == 1)
                                {
                                    IntPtr kbPtr = IntPtr.Add(buffer, Marshal.SizeOf<RAWINPUTHEADER>());
                                    var kb = Marshal.PtrToStructure<RAWKEYBOARD>(kbPtr);
                                    if ((kb.Flags & 0x01) == 0) HandleHotkey(kb.VKey);
                                }
                            }
                        }
                        finally { Marshal.FreeHGlobal(buffer); }
                    }
                }
                catch { }
            }
            base.WndProc(ref m);
        }

        private void HandleHotkey(ushort vKey)
        {
            if ((DateTime.Now - _lastHotkeyTime).TotalMilliseconds < 150) return;
            if ((Keys)vKey != _selectedHotkey) return;
            _lastHotkeyTime = DateTime.Now;
            if (btnToggle.InvokeRequired) btnToggle.Invoke(new Action(() => btnToggle.PerformClick()));
            else btnToggle.PerformClick();
        }

        // ==================== 清理 ====================
        private void ForceCleanup()
        {
            try
            {
                StopDropping();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Thread.Sleep(500);
                foreach (string file in new[] { _dllPath, _sysPath })
                {
                    if (!string.IsNullOrEmpty(file) && File.Exists(file))
                    {
                        try { File.SetAttributes(file, FileAttributes.Normal); File.Delete(file); }
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

        private void comboBox热键_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (comboBox热键.SelectedItem is string s && s.StartsWith("F"))
            {
                if (int.TryParse(s.Substring(1), out int n) && n >= 1 && n <= 12)
                    _selectedHotkey = (Keys)((int)Keys.F1 + (n - 1));
            }
        }
    }
}