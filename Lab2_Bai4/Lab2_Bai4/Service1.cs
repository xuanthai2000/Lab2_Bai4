using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Bai4
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.I4)] int SessionId,String pTitle,
            [MarshalAs(UnmanagedType.U4)] int TitleLength,String pMessage,
            [MarshalAs(UnmanagedType.U4)] int MessageLength,
            [MarshalAs(UnmanagedType.U4)] int Style,
            [MarshalAs(UnmanagedType.U4)] int Timeout,
            [MarshalAs(UnmanagedType.U4)] out int pResponse, bool bWait);

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = 1; //giá trị này thay đổi mỗi khi mở máy

        /// <summary>
        /// Hàm có chức năng pop-up MSSV
        /// </summary>
        public static void PopUp()
        {
            bool result = false;
            String title = "PopUp";
            int tlen = title.Length;
            String msg = "18521379";
            int mlen = msg.Length;
            int resp = 0;
            result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, title, tlen, msg, mlen, 0, 0, out resp, false);
        }


        //Ghi file log để test ( không quan trọng lắm ở bài này)
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        /// <summary>
        /// Hàm có chức năng bắt sự kiện lock, unlock,... của máy tính
        /// </summary>
        /// <param name="changeDescription"></param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {

            switch (changeDescription.Reason)
            {
                //trường hợp user login và unlock máy tính thì gọi hàm PopUp
                case SessionChangeReason.SessionLogon:
                case SessionChangeReason.SessionUnlock:
                    {
                        PopUp();
                        WriteToFile("Lock " + DateTime.Now);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
