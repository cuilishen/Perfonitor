﻿using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace Perfonitor
{
    /// <summary>
    /// ProcessorMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class MemoryMonitor : UserControl
    {

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryKilobytes);

        private ObservableDataSource<Point> dataSource;
        private PerformanceCounter performanceCounter;
        private int currentSecond = TIMESPAN;

        private const int TIMESPAN = 10;

        public MemoryMonitor()
        {
            InitializeComponent();
            InitPerformance();
            InitDataSource();
            InitPlotter();
        }

        private void InitPerformance()
        {
            performanceCounter = new PerformanceCounter()
            {
                CategoryName = "Memory",
                CounterName = "Available MBytes",
            };
        }

        private void InitDataSource()
        {
            dataSource = new ObservableDataSource<Point>();
            for (int _ = 0; _ < TIMESPAN; ++_)
            {
                dataSource.AppendAsync(base.Dispatcher, new Point(0, 0));
            }
        }

        private void InitPlotter()
        {
            plotter.AddLineGraph(dataSource, Color.FromRgb(0x28, 0x2c, 0x37), 1);
            plotter.Viewport.FitToView();
            plotter.Children.Remove(plotter.MouseNavigation);
            plotter.Children.Remove(plotter.KeyboardNavigation);
            plotter.Children.Remove(plotter.Legend);
            plotter.Children.Remove(plotter.DefaultContextMenu);
            plotter.AxisGrid.Remove();
        }

        public void Update()
        {
            GetPhysicallyInstalledSystemMemory(out long memKB);
            double totalMB = memKB / 1024.0;
            double availMB = performanceCounter.NextValue();

            if (plotter.Visibility == Visibility.Visible)
            {
                Point p = new Point()
                {
                    X = currentSecond,
                    Y = 1 - availMB / totalMB
                };
                dataSource.AppendAsync(base.Dispatcher, p);
                double xaxis = currentSecond > 10 ? currentSecond - 10
                                              : 0;
                ++currentSecond;
                plotter.Viewport.Visible = new System.Windows.Rect(xaxis, 0, 10, 1);
            }
            double usedMB = totalMB - availMB;
            string memoryUsage = string.Format("{0:F1}/{1:F1} GB", usedMB / 1024, totalMB / 1024);
            usageText.Text = memoryUsage;
        }

        public void ShowLineChart()
        {
            plotter.Visibility = Visibility.Visible;
        }

        public void UnshowLineChart()
        {
            plotter.Visibility = Visibility.Collapsed;
        }
    }
}
