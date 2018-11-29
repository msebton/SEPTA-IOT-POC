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
        private const int IR_EMITTER_PIN = 17;
        private const int IR_RECEIVER_PIN = 4;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);
        private SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);
        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPin emitterPin;
        private GpioPin receiverPin;
        bool seatOccupied = false;
        bool validCard = false;

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

            // emitter IR LED - turn on IR LED
            emitterPin = gpio.OpenPin(IR_EMITTER_PIN);
            emitterPin.SetDriveMode(GpioPinDriveMode.Output);
            pinValue = GpioPinValue.High;
            emitterPin.Write(pinValue);

            // receiver IR Photo LED
            receiverPin = gpio.OpenPin(IR_RECEIVER_PIN);
            receiverPin.SetDriveMode(GpioPinDriveMode.InputPullUp);

            txtCardNumber.Text = "GPIO pin initialized correctly.";

            // set the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += ReadSensors;
            timer.Start();
        }

        private void ReadSensors(object o, object e)
        {
            Uid uid = null;

            // start with a vacant seat
            setVacantSeat();

            // RFID Card (MFRC522)
            if (mfrc.IsTagPresent())
            {
                uid = mfrc.ReadUid();
                if (uid.IsValid)
                {
                    if (uid.ToString() == "28723002")
                    {
                        validCard = true;
                        setGreenLight();
                        setValidSeat();
                    }
                    else
                    {
                        validCard = false;
                        setRedLight();
                        setInvalidSeat();
                    }
                }
                mfrc.HaltTag();
            }

            // IR Receiver
            var receiverValue = receiverPin.Read();
            if (receiverValue == GpioPinValue.High) // seat is occupied
            {
                seatOccupied = true;
            }
            else // seat is vacant
            {
                seatOccupied = false;
                validCard = false;
                setVacantSeat();
            }

            if (!seatOccupied)
            {
                setVacantSeat();
            }
            else if (seatOccupied && !validCard)
            {
                setInvalidSeat();
            }
            else if (seatOccupied && validCard)
            {
                setValidSeat();
            }

        }

        private void setVacantSeat()
        {
            if (seat_6c.Fill != null)
            {
                seat_6c.Visibility = Visibility.Visible;
                seat_6c.Fill = whiteBrush;
                txt6C.Foreground = blackBrush;
                txt6C.Visibility = Visibility.Visible;
            }
        }

        private void setValidSeat()
        {
            seat_6c.Visibility = Visibility.Visible;
            seat_6c.Fill = greenBrush;
            txt6C.Foreground = whiteBrush;
            txt6C.Visibility = Visibility.Visible;
        }

        private void setInvalidSeat()
        {
            if (seat_6c.Fill != greenBrush)
            {
                seat_6c.Visibility = Visibility.Visible;
                seat_6c.Fill = redBrush;
                txt6C.Foreground = whiteBrush;
                txt6C.Visibility = Visibility.Visible; 
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
