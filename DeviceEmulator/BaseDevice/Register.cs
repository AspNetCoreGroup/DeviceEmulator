using DeviceEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.BaseDevice
{
    public class Register : IRegister
    {
        IncrementTipe _incrementTipe;
        IDeviceRtc Rtc { get; }
        uint _startValue;
        public Register(IDeviceRtc rtc, string sogialName, uint startValue, IScaleAndUnit scaleAndUnit, IncrementTipe incrementTipe)
        {
            Rtc = rtc;
            SogialName = sogialName;
            Value = startValue;
            ScaleAndUnit = scaleAndUnit;
            _startValue = startValue;
        }

        public string SogialName { get; set; }
        public uint Value { get; set; }
        public IScaleAndUnit ScaleAndUnit { get; private set; }


        public  Task StartWatch(CancellationToken token)
        {

            WathTask  =  WatchLoop(token);
            return Task.CompletedTask;
        }
        Task WathTask;

        private async Task WatchLoop(CancellationToken token)
        {
            long oldRtc = Rtc.I;
            while (!token.IsCancellationRequested)
            {
                if (oldRtc != Rtc.I)
                {
                    oldRtc = Rtc.I;
                    if (_incrementTipe == IncrementTipe.UpDown)
                    {
                        Value = _startValue + (uint)rrrr();
                    }
                    else if (_incrementTipe == IncrementTipe.Increment)
                    {
                        Value++;
                    }
                    else if (_incrementTipe == IncrementTipe.Decrement)
                    {
                        if (Value > 0)
                        {
                            Value--;
                        }
                    }
                    Debug.WriteLine("Register " + Value);
                }

                await Task.Delay(100); // Добавляем небольшую задержку
            }
        }

        private static double rrrr()
        {
            Random random = new Random();
            double[] probabilities = new double[] { 0.5, 0.3, 0.15, 0.05 };
            int[] values = new int[] { 10, 50, 75, 100 };

            double cumulative = 0.0;
            double r = random.NextDouble();

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (r < cumulative)
                {
                    return values[i];
                }
            }

            return values[values.Length - 1]; // fallback
        }
    }

    public enum IncrementTipe
    {
        UpDown,
        Increment,
        Decrement
    }

    public class ScaleAndUnit : IScaleAndUnit
    {
        public sbyte Scale { get; set; }
        public byte Unit { get; set; }
    }
}
