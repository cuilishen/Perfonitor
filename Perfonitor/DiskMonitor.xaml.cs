﻿using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
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
    public partial class DiskMonitor : UserControl
    {
        private PerformanceCounter readCounter;
        private PerformanceCounter writeCounter;

        public DiskMonitor()
        {
            InitializeComponent();
            InitPerformance();
        }

        private void InitPerformance()
        {
            readCounter = new PerformanceCounter()
            {
                CategoryName = "PhysicalDisk",
                CounterName = "Disk Read Bytes/sec",
                InstanceName = "_Total"
            };
            writeCounter = new PerformanceCounter()
            {
                CategoryName = "PhysicalDisk",
                CounterName = "Disk Write Bytes/sec",
                InstanceName = "_Total"
            };
        }

        public void Update()
        {
            double readB = readCounter.NextValue();
            double writeB = writeCounter.NextValue();
            string diskWrite = Format(writeB);
            string diskRead = Format(readB);
            writeText.Text = diskWrite;
            readText.Text = diskRead;
        }

        private string Format(double value)
        {
            if (value < 1024)
            {
                return string.Format("{0:F0} B/s", value);
            }
            if (value < 1024 * 1024)
            {
                return string.Format("{0:F1} KB/s", value / 1024);
            }
            if (value < 1024 * 1024 * 1024)
            {
                return string.Format("{0:F1} MB/s", value / 1024 / 1024);
            }
            return string.Format("{0:F1} GB/s", value / 1024 / 1024 / 1024);
        }
    }
}
