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
        private const int RED_LED_PIN = 5;
        private const int GREEN_LED_PIN = 12;
        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPinValue pinValue;

        public MainPage()
        {
            this.InitializeComponent();

            InitGPIO();

            InitRC522Async();
            //MainAsync().GetAwaiter().GetResult();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                redPin = null;
                txtCardNumber.Text = "There is no GPIO controller on this device.";
                return;
            }

            // red LED
            redPin = gpio.OpenPin(RED_LED_PIN);
            redPin.SetDriveMode(GpioPinDriveMode.Output);
            pinValue = GpioPinValue.High;
            redPin.Write(pinValue);

            // green LED
            greenPin = gpio.OpenPin(GREEN_LED_PIN);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
            pinValue = GpioPinValue.Low;
            greenPin.Write(pinValue);

            txtCardNumber.Text = "GPIO pin initialized correctly.";

        }

        public async void InitRC522Async()
        {

            var mfrc = new Mfrc522();
            var redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            var greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

            await mfrc.InitIO();

            //while (true)
            //{
            //    if (mfrc.IsTagPresent())
            //    {
            //        var uid = mfrc.ReadUid();
            //        if (uid.IsValid)
            //        {
            //            if (uid.ToString() == "28723002")
            //            {
            //                setGreenLight();
            //                seat_6c.Visibility = Visibility.Visible;
            //                seat_6c.Fill = greenBrush;
            //                txt6C.Visibility = Visibility.Visible;
            //            }
            //            else
            //            {
            //                setRedLight();
            //                txtCardNumber.Text = uid.ToString();
            //                seat_6c.Visibility = Visibility.Visible;
            //                seat_6c.Fill = redBrush;
            //                txt6C.Visibility = Visibility.Visible;
            //            }

            //            //pinValue = GpioPinValue.Low;
            //            //redPin.Write(pinValue);
            //            //pinValue = GpioPinValue.High;
            //            //greenPin.Write(pinValue);
            //        }
            //        mfrc.HaltTag();
            //    }
            //}
        }

        private void setRedLight()
        {
            resetLights();

            redPin.Write(GpioPinValue.High);
            greenPin.Write(GpioPinValue.Low);
        }

        private void setGreenLight()
        {
            resetLights();

            redPin.Write(GpioPinValue.Low);
            greenPin.Write(GpioPinValue.High);
        }

        private void resetLights()
        {
            redPin.Write(GpioPinValue.Low);
            greenPin.Write(GpioPinValue.Low);
        }
    }
}
