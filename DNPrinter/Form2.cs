using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNPrinter
{
    public partial class Form2 : Form
    {
        Form1 Caller;

        public Form2(Form1 caller)
        {
            Caller = caller;
            InitializeComponent();

            foreach(var printer in PrinterSettings.InstalledPrinters)
                this.comboPrinterNames.Items.Add(printer);

            var settings = new PrinterSettings();
            this.comboPrinterNames.SelectedItem = settings.PrinterName;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            var progress = new Ookii.Dialogs.WinForms.ProgressDialog();
            progress.WindowTitle = "Printing Delivery Documents";
            progress.Text = "Printing delivery documents. Why don't you find something else nice to do with your time until this is done?";
            progress.DoWork += DoPrintWork();
            progress.RunWorkerCompleted += OnPrintComplete();
            progress.ShowDialog(this);
            progress.ProgressBarStyle = Ookii.Dialogs.WinForms.ProgressBarStyle.MarqueeProgressBar;
            progress.ShowTimeRemaining = false;
            */
            var printerName = this.comboPrinterNames.Text;
            Caller.BeginPrinting();

            using (var projDialog = new ProgressDialog())
            {
                projDialog.Description = "Printing delivery documents...";
                projDialog.RunWorkerCompleted += (senderInner, args) =>
                {
                    this.Close();
                };
                projDialog.DoWork += (senderInner, args) => 
                {
                    while (Caller.IsPrinting)
                    {
                        Thread.Sleep(1);
                        if (projDialog.CancellationPending)
                        {
                            //TODO: figure out how the hell to actually stop printing!
                            this.Close();
                        }
                    }

                };
                projDialog.ShowDialog(this);
            }

            Caller.PrintFiles2(printerName);
        }

        private RunWorkerCompletedEventHandler OnPrintComplete()
        {
            this.Close();
            throw new Exception("Not yet implemented");
        }

        private DoWorkEventHandler DoPrintWork()
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
