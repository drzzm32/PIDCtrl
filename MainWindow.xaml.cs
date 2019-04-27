using System;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Collections.Generic;

namespace PIDCtrl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private double Kp, Ki, Kd, Set, Val;
        private double iCtrl, dCtrl;

        private Queue<double> theVals;

        private Timer theTimer;

        public MainWindow()
        {
            InitializeComponent();

            Kp = Ki = Kd = 0;
            Set = Val = 0;
            iCtrl = dCtrl = 0;

            theVals = new Queue<double>();
            theVals.Enqueue(0); theVals.Enqueue(0); theVals.Enqueue(0);

            theTimer = new Timer((obj) => {
                double[] a = theVals.ToArray();
                double e0 = a[2];
                double e1 = a[1];
                double e2 = a[0];

                Val = Kp * (e0 - e1);
                Val += iCtrl * Ki * e0;
                Val += dCtrl * Kd * (e0 - 2 * e1 + e2);

                theVals.Dequeue();
                theVals.Enqueue(Val);
                setVal(Val);
            });
        }

        private void boxKp_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(boxKp.Text, out double val);
            Kp = val;
            if (sliderKp != null) sliderKp.Value = Kp;
        }

        private void sliderKp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Kp = sliderKp.Value;
            if (boxKp != null) boxKp.Text = Kp.ToString("F2");
        }

        private void boxKi_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(boxKi.Text, out double val);
            Ki = val;
            if (sliderKi != null) sliderKi.Value = Ki;
        }

        private void sliderKi_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Ki = sliderKi.Value;
            if (boxKi != null) boxKi.Text = Ki.ToString("F2");
        }

        private void boxKd_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(boxKd.Text, out double val);
            Kd = val;
            if (sliderKd != null) sliderKd.Value = Kd;
        }

        private void sliderKd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Kd = sliderKd.Value;
            if (boxKd != null) boxKd.Text = Kd.ToString("F2");
        }

        private void boxSet_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(boxSet.Text, out double val);
            Set = val;
            if (sliderSet != null) sliderSet.Value = Set;

            Val = Set;
        }

        private void sliderSet_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Set = sliderSet.Value;
            if (boxSet != null) boxSet.Text = Set.ToString("F2");

            Val = Set;
        }

        private void setVal(double val)
        {
            boxVal.Dispatcher.Invoke(() => boxVal.Text = val.ToString("F2"));
            sliderVal.Dispatcher.Invoke(() => sliderVal.Value = val);
        }

        private void iToggle_Click(object sender, RoutedEventArgs e)
        {
            if (iCtrl == 0) iCtrl = 1;
            else if (iCtrl == 1) iCtrl = 0;
            else iCtrl = 0;

            iState.IsChecked = iCtrl != 0;
        }

        private void dToggle_Click(object sender, RoutedEventArgs e)
        {
            if (dCtrl == 0) dCtrl = 1;
            else if (dCtrl == 1) dCtrl = 0;
            else dCtrl = 0;

            dState.IsChecked = dCtrl != 0;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            boxFreq.IsEnabled = false; boxState.IsChecked = true;
            double.TryParse(boxFreq.Text, out double freq);
            if (freq <= 0) freq = 1;
            theTimer.Change(0, (int)(1000 / freq));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            boxFreq.IsEnabled = true; boxState.IsChecked = false;
            theTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
