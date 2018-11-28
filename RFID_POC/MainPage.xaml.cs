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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RFID_POC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioController gpio;
        private DispatcherTimer timer;
        private const int RED_LED_PIN = 5;
        private const int GREEN_LED_PIN = 12;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPinValue pinValue;
        private Mfrc522 mfrc;

        public MainPage()
        {
            this.InitializeComponent();

            InitGPIO();

            SetupMfrcDevice();
        }

        private void InitGPIO()
        {
            gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                redPin = null;
                txtCardNumber.Text = "There is no GPIO controller on this device.";
                return;
            }
        }

        public async void SetupMfrcDevice()
        {
            mfrc = new Mfrc522();
            await mfrc.InitIO();

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

            // set the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += ReadRfidCard;
            timer.Start();
        }

        private void ReadRfidCard(object o, object e)
        {
            if (mfrc.IsTagPresent())
            {
                var uid = mfrc.ReadUid();
                if (uid.IsValid)
                {
                    if (uid.ToString() == "28723002")
                    {
                        setGreenLight();
                        seat_6c.Visibility = Visibility.Visible;
                        seat_6c.Fill = greenBrush;
                        txt6C.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        setRedLight();
                        txtCardNumber.Text = uid.ToString();
                        seat_6c.Visibility = Visibility.Visible;
                        seat_6c.Fill = redBrush;
                        txt6C.Visibility = Visibility.Visible;
                    }
                }
                mfrc.HaltTag();
            }
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
