using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SchoolQuizMachine.Models.Services
{
    public class GpioButtonService
    {
        private GpioPin ButtonOne;
        private GpioPin ButtonTwo;
        private GpioPin ButtonThree;
        private bool buttonOneStatement;
        private bool buttonTwoStatement;
        private bool buttonThreeStatement;
        private GpioController controller;

        public event EventHandler ButtonOnePressed;
        public event EventHandler ButtonTwoPressed;
        public event EventHandler ButtonThreePressed;

        public GpioButtonService()
        {
            buttonOneStatement = false;
            buttonTwoStatement = false;
            buttonThreeStatement = false;
            controller = GpioController.GetDefault();
            if (controller != null)
            {
                ButtonOne = controller.OpenPin(16);
                ButtonTwo = controller.OpenPin(20);
                ButtonThree = controller.OpenPin(21);
                ButtonOne.SetDriveMode(GpioPinDriveMode.Input);
                ButtonTwo.SetDriveMode(GpioPinDriveMode.Input);
                ButtonThree.SetDriveMode(GpioPinDriveMode.Input);
                ButtonOne.Write(GpioPinValue.Low);
                ButtonTwo.Write(GpioPinValue.Low);
                ButtonThree.Write(GpioPinValue.Low);
                ButtonOne.DebounceTimeout = TimeSpan.FromMilliseconds(100);
                ButtonTwo.DebounceTimeout = TimeSpan.FromMilliseconds(100);
                ButtonThree.DebounceTimeout = TimeSpan.FromMilliseconds(100);
                ButtonOne.ValueChanged += ButtonOne_ValueChanged;
                ButtonTwo.ValueChanged += ButtonTwo_ValueChanged;
                ButtonThree.ValueChanged += ButtonThree_ValueChanged;
            }
        }


        private void ButtonThree_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (buttonThreeStatement == false)
            {
                buttonThreeStatement = true;
                ButtonThreePressed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                buttonThreeStatement = false;
            }
        }

        private void ButtonTwo_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (buttonTwoStatement == false)
            {
                buttonTwoStatement = true;
                ButtonTwoPressed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                buttonTwoStatement = false;
            }

        }

        private void ButtonOne_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (buttonOneStatement == false)
            {
                buttonOneStatement = true;
                ButtonOnePressed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                buttonOneStatement = false;
            }

        }

        public Task Dispose()
        {
            var task = Task.Factory.StartNew(() =>
            {
                ButtonOne.Dispose();
                ButtonTwo.Dispose();
                ButtonThree.Dispose();
                controller = null;
            });
            return task;
        }
    }
}
