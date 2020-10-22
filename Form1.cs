using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.Example.JavascriptBinding;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace Chrome
{
    public partial class Form1 : Form
    {

        public ChromiumWebBrowser chromeBrowser;

        public IFocusHandler customFocusHandler;

        public bool multiThreadedMessageLoop;


        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("https://www.google.co.th");


            chromeBrowser.LoadingStateChanged += OnBrowserLoadingStateChanged;
            chromeBrowser.ConsoleMessage += OnBrowserConsoleMessage;
            chromeBrowser.StatusMessage += OnBrowserStatusMessage;
            chromeBrowser.TitleChanged += OnBrowserTitleChanged;
            chromeBrowser.AddressChanged += OnBrowserAddressChanged;
#if !NETCOREAPP
            chromeBrowser.JavascriptObjectRepository.Register("bound", new BoundObject(), false);
#endif

            if (!multiThreadedMessageLoop)
            {
                chromeBrowser.FocusHandler = null;
            }

            //Only override if we have a custom handler
            if (customFocusHandler != null)
            {
                chromeBrowser.FocusHandler = customFocusHandler;
            }


        }



        public Form1()
        {
            InitializeComponent();

            InitializeChromium();
        }



        //RandomNumber
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        //RandomNumber



        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

                        

        //Check Status Message
        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => toolStripStatusLabel1.Text = args.Value);
        }
        //Check Status Message






        //SATIC SENDKEY

        //How to use =>  SendKeys(chromeBrowser);        

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        static string GetCharsFromKeys(Keys keys, bool shift, bool altGr)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[(int)Keys.ShiftKey] = 0xff;
            if (altGr)
            {
                keyboardState[(int)Keys.ControlKey] = 0xff;
                keyboardState[(int)Keys.Menu] = 0xff;
            }
            ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }
        //SATIC SENDKEY



        //SATIC SENDKEY
        static void chromeBrowserSendKeys(ChromiumWebBrowser chromeBrowser)
        {
            KeyEvent[] events = new KeyEvent[] {
                new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.R, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false }, // Just the letter R, no shift (so no caps...?)
                new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.R, true, false)[0], Modifiers = CefEventFlags.ShiftDown, Type = KeyEventType.Char, IsSystemKey = false }, // Capital R?
                new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D4, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false }, // Just the number 4
                new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D4, true, false)[0], Modifiers = CefEventFlags.ShiftDown, Type = KeyEventType.Char, IsSystemKey = false }, // Shift 4 (should be $)
                  
            };

            foreach (KeyEvent ev in events)
            {
                Thread.Sleep(100);
                chromeBrowser.GetBrowser().GetHost().SendKeyEvent(ev);
            }
        }
        //SATIC SENDKEY





        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => txtURL.Text = args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            //this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            //this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }



        private void SetIsLoading(bool isLoading)
        {
          
            HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            //this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
        
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            chromeBrowser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(txtURL.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            chromeBrowser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            chromeBrowser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(txtURL.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                chromeBrowser.Load(url);
            }
        }




        //Find Windows Name
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        //Find Windows Name



        //Form1 Load
        private void Form1_Load(object sender, EventArgs e)
        {
            //Form1 Load




            //Form Properties
            try
            {

                txtURL.TabStop = false;

                statusStrip1.TabStop = false;

                this.ShowIcon = false;

                this.ShowInTaskbar = false;

            }
            catch
            {

            }
            //Form Properties



            //Add Chrome Browesr to Form Control
            try
            {

                this.Controls.Add(chromeBrowser);

                chromeBrowser.AddressChanged += AddressTarget;

            }
            catch
            {

            }
            //Add Chrome Browesr to Form Control




            //Form1 Load
        }
        //Form1 Load



        private void AddressTarget(object sender, AddressChangedEventArgs e)
        {

            this.Invoke(new MethodInvoker(() =>
            {
                txtURL.Text = e.Address;
            }
            ));
        }

        private void txtURL_KeyUp(object sender, KeyEventArgs e)
        {

            if(e.KeyCode == Keys.Enter)
            {

                chromeBrowser.Load(txtURL.Text);

            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                chromeBrowser.Back();
            }
            else if (e.KeyCode == Keys.End)
            {
                chromeBrowser.Forward();
            }
            else if (e.KeyCode == Keys.F5)
            {
                chromeBrowser.Refresh();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Cef.Shutdown();
            }
            catch
            {

            }
        }





        //Class
    }
    //Class




    //Class ControlExtensions
    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// No action will be performed if the control doesn't have a valid handle or the control is Disposed/Disposing.
        /// </summary>
        /// <param name="control">the control for which the update is required</param>
        /// <param name="action">action to be performed on the control</param>
        public static void InvokeOnUiThreadIfRequired(this Control control, Action action)
        {
            //See https://stackoverflow.com/questions/1874728/avoid-calling-invoke-when-the-control-is-disposed
            //for background and some guidance when implementing your own version.
            //No action
            if (control.Disposing || control.IsDisposed || !control.IsHandleCreated)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                control.BeginInvoke((Action)(() =>
                {
                    //No action
                    if (control.Disposing || control.IsDisposed || !control.IsHandleCreated)
                    {
                        return;
                    }

                    action();
                }));
            }
            else
            {
                action.Invoke();
            }
        }
    }
    //Class ControlExtensions








}
