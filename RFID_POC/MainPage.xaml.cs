using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Mfrc522Lib;
//using Microsoft.SPOT.Hardware;
//using SecretLabs.NETMF.Hardware.NetduinoPlus;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RFID_POC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            InitRC522Async();
            //MainAsync().GetAwaiter().GetResult();
        }


        public async void InitRC522Async()
        {

            var mfrc = new Mfrc522();
            await mfrc.InitIO();

            //var uid = mfrc.ReadUid();
            ////var mainPage = new MainPage();
            //string txt_Result = "";
            //foreach (byte byt in uid.FullUid)
            //{
            //    txt_Result = txt_Result + byt.ToString("x2");
            //    txtCardNumber.Text = txt_Result;
            //}
            //mfrc.HaltTag();

            while (true)
            {
                if (mfrc.IsTagPresent())
                {
                    var uid = mfrc.ReadUid();
                    if (uid.IsValid)
                    {
                        mfrc.HaltTag();
                        txtCardNumber.Text = uid.ToString();
                        break;
                    }
                }
            }
        }



        static async Task MainAsync()
        {
            var mfrc = new Mfrc522();
            await mfrc.InitIO();

            var uid = mfrc.ReadUid();
            var mainPage = new MainPage();
            string txt_Result = "";
            foreach (byte byt in uid.FullUid)
            {
                txt_Result = txt_Result + byt.ToString("x2");
                mainPage.txtCardNumber.Text = txt_Result;
            }
            mfrc.HaltTag();

            //while (true)
            //{
            //    if (mfrc.IsTagPresent())
            //    {
            //        var uid = mfrc.ReadUid();
            //        txtCardNumber.Text = uid.ToString();

            //        mfrc.HaltTag();
            //    }

            //}
        }
    }
}
