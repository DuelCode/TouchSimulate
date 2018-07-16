using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InjectTouchInputTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            TouchInjector.InitializeTouchInjection();
            this.TouchDown += MainWindow_TouchDown;
            this.TouchMove += MainWindow_TouchMove;
            this.TouchUp += MainWindow_TouchUp;
            this.BdrSimulateZm.MouseLeftButtonUp += BdrSimulateZm_MouseLeftButtonUp;
        }

        private Random GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return new Random(BitConverter.ToInt32(bytes, 0));
        }

        private void BdrSimulateZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int x = this.GetRandomSeed().Next(50, 1680 - 100);
            int y = this.GetRandomSeed().Next(50, 1080 - 100);
            SimulateTouch(x, y);
        }
        
        private void MainWindow_TouchUp(object sender, TouchEventArgs e)
        {
            System.Windows.Input.TouchPoint oPos = e.GetTouchPoint(this);
            this.ProxyLine.X2 = oPos.Position.X;
            this.ProxyLine.Y2 = oPos.Position.Y;
            this.GdRootZm.Children.Add(this.ProxyLine);
            Console.WriteLine("TouchID " + e.TouchDevice.Id + " TouchUp " + oPos.Position.X + "    " + oPos.Position.Y);
        }

        private void MainWindow_TouchMove(object sender, TouchEventArgs e)
        {
            System.Windows.Input.TouchPoint oPos = e.GetTouchPoint(this);
            Console.WriteLine("TouchID " + e.TouchDevice.Id + " TouchMove " + oPos.Position.X + "    " + oPos.Position.Y);
        }

        private Line ProxyLine;

        private void MainWindow_TouchDown(object sender, TouchEventArgs e)
        {
            System.Windows.Input.TouchPoint oPos = e.GetTouchPoint(this);
            Line oLine = new Line();
            oLine.Stroke = new SolidColorBrush(Colors.Red);
            oLine.StrokeThickness = 2;
            oLine.X1 = oPos.Position.X;
            oLine.Y1 = oPos.Position.Y;
            this.ProxyLine = oLine;
            Console.WriteLine("TouchID " + e.TouchDevice.Id + "  TouchDown " + oPos.Position.X + "    " + oPos.Position.Y);
        }

        private void SimulateTouch(int x, int y)
        {
            // Touch Down Simulate
            PointerTouchInfo contact = MakePointerTouchInfo(x, y, 5, 1);
            PointerFlags oFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;
            contact.PointerInfo.PointerFlags = oFlags;
            bool bIsSuccess = TouchInjector.InjectTouchInput(1, new[] { contact });

            // Touch Move Simulate
            int nMoveIntervalX = this.GetRandomSeed().Next(-60, 60);
            int nMoveIntervalY = this.GetRandomSeed().Next(-60, 60);
            contact.Move(nMoveIntervalX, nMoveIntervalY);
            oFlags = PointerFlags.INRANGE | PointerFlags.INCONTACT | PointerFlags.UPDATE;
            contact.PointerInfo.PointerFlags = oFlags;
            TouchInjector.InjectTouchInput(1, new[] { contact });

            // Touch Up Simulate
            contact.PointerInfo.PointerFlags = PointerFlags.UP;
            TouchInjector.InjectTouchInput(1, new[] { contact });
        }

        private PointerTouchInfo MakePointerTouchInfo(int x, int y, int radius, 
            uint orientation = 90, uint pressure = 32000)
        {
            PointerTouchInfo contact = new PointerTouchInfo();
            contact.PointerInfo.pointerType = PointerInputType.TOUCH;
            contact.TouchFlags = TouchFlags.NONE;
            contact.Orientation = orientation;
            contact.Pressure = pressure;
            contact.TouchMasks = TouchMask.CONTACTAREA | TouchMask.ORIENTATION | TouchMask.PRESSURE;
            contact.PointerInfo.PtPixelLocation.X = x;
            contact.PointerInfo.PtPixelLocation.Y = y;
            uint unPointerId = IdGenerator.GetUinqueUInt();
            Console.WriteLine("PointerId    " + unPointerId);
            contact.PointerInfo.PointerId = unPointerId;
            contact.ContactArea.left = x - radius;
            contact.ContactArea.right = x + radius;
            contact.ContactArea.top = y - radius;
            contact.ContactArea.bottom = y + radius;
            return contact;
        }
    }
}
