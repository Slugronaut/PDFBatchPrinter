using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using org.pdfclown.tools;

namespace DNPrinter
{
    public partial class Form1 : Form
    {
        const int ModCell = 0;
        const int DrawingCell = 1;
        const int DNCell = 2;
        const int PrintCell = 3;


        public Form1()
        {
            InitializeComponent();
            this.dataGrid.AllowDrop = true;
            this.dataGrid.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.dataGrid.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericDNCount_ValueChanged(object sender, EventArgs e)
        {

        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                var row = new DataGridViewRow();
                //TODO: open pfd and parse module info
                var modInfo = FindModuleIdInPDF(file);
                string modId = null;// modInfo[0];
                string modRev = null;// modInfo[1];

                if (modInfo != null && modInfo.Length >= 2)
                {
                    modId = modInfo[0];
                    modRev = modInfo[1];
                }

                //if (string.IsNullOrEmpty(modId))
                //{
                //    if(MessageBox.Show(this, "The module could not be determined for the pdf file\n\n\"" + file + "\"\n\nDo you want to add this file to the rpint list anyway?", "Add PDF?", MessageBoxButtons.YesNo) == DialogResult.No)
                //        continue;
                //}
                if (string.IsNullOrEmpty(modRev))
                    modRev = "100"; //just assume an obsurdly high revision by default, the printer will find the correct one
                string drawingDoc = FindDrawingFromModId(modId, modRev);
                dataGrid.Rows.Add(modId, drawingDoc, file, true);
                
            }
        }

        private void buttonPrintClick(object sender, EventArgs e)
        {
            var items = this.dataGrid.Rows;
            if(items == null || items.Count < 1)
            {
                MessageBox.Show(this, "There are no files selected to print.\nPlease drag n' drop some files into the list box above.");
                return;
            }
            else
            {
                //if (MessageBox.Show(this, "Are you sure you want to print these files and their drawings?", "Confirm Print", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //    PrintFiles();
                var form2 = new Form2(this);
                form2.ShowDialog(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearClick(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to clear the list of items to be printed?", "Confirm Clear", MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.dataGrid.Rows.Clear();
        }

        public bool IsPrinting { get; private set; }
        public void BeginPrinting()
        {
            IsPrinting = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        public void PrintFiles2(string printerName)
        {
            IsPrinting = true;
            

            var printSettings = new PrinterSettings
            {
                PrinterName = printerName,
                Copies = (short)numericDNCount.Value,
            };
            var pageSettings = new PageSettings(printSettings)
            {
                Margins = new Margins(0, 0, 0, 0),
            };
            foreach (PaperSize paperSize in printSettings.PaperSizes)
            {
                if (paperSize.PaperName == "Letter")
                {
                    pageSettings.PaperSize = paperSize;
                    break;
                }
            }


            #region Actual Printing
            //for each row we want to print x number of DNs and y number of drawings
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];

                //skip printing if print checkbox is unmarked
                if((bool)row.Cells[3].Value == false)
                {
                    continue;
                }
                string dnPath = row.Cells[DNCell].Value as string;
                if (!string.IsNullOrEmpty(dnPath)) //TODO: add row to a 'did not print' list
                {
                    Console.WriteLine("PRINT DN: " + dnPath);
                    printSettings.Copies = (short)numericDNCount.Value;
                    using (var doc = PdfiumViewer.PdfDocument.Load(dnPath))
                    {
                        using (var pDoc = doc.CreatePrintDocument())
                        {
                            pDoc.PrinterSettings = printSettings;
                            pDoc.DefaultPageSettings = pageSettings;
                            pDoc.PrintController = new StandardPrintController();
                            pDoc.Print();
                        }
                    }
                }

                string drawingPath = row.Cells[DrawingCell].Value as string;
                if (!string.IsNullOrEmpty(drawingPath)) //TODO: add row to a 'did not print' list
                {
                    Console.WriteLine("PRINT DRAWING: " + drawingPath);
                    printSettings.Copies = (short)numericDrawingCount.Value;
                    using (var doc = PdfiumViewer.PdfDocument.Load(drawingPath))
                    {
                        using (var pDoc = doc.CreatePrintDocument())
                        {
                            pDoc.PrinterSettings = printSettings;
                            pDoc.DefaultPageSettings = pageSettings;
                            pDoc.PrintController = new StandardPrintController();
                            pDoc.Print();
                        }
                    }
                }
            }
            
            #endregion

            IsPrinting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PrintFiles(string printerName)
        {
            this.IsPrinting = true;
            var defaultPrinterSettings = new PrinterSettings();
            PrinterInterop.SetDefaultPrinter(printerName);


            List<string> dnFiles = new List<string>(100);
            List<string> drawFiles = new List<string>(100);
            for(int i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];
                if (!(bool)row.Cells[PrintCell].Value) continue;

                dnFiles.Add(row.Cells[DNCell].Value as string);
                drawFiles.Add(row.Cells[DrawingCell].Value as string);
            }

            #region BEGIN OUTTER PRINT LOOP
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                #region BEGIN PRINT DELIVERY NOTES
                for (int dni = 0; dni < this.numericDNCount.Value; dni++)
                {
                    string filename = dnFiles[i];
                    Console.WriteLine("PRINT DN: " + filename);
                    //StartPrinterProcess(filename);
                }
                #endregion
                
                #region BEGIN PRINT DRAWINGS
                for (int dni = 0; dni < this.numericDrawingCount.Value; dni++)
                {
                    string filename = drawFiles[i];
                    Console.WriteLine("PRINT DRAWING: " + filename);

                    var ud = GetUserDomain();
                    SecureString pass = MakeSecureString("!57jcl57!");
                    StartPrinterProcessAuthenticated(filename, ud[0], ud[1], pass);
                }
                #endregion
                
                }
            #endregion

            PrinterInterop.SetDefaultPrinter(defaultPrinterSettings.PrinterName);
            this.IsPrinting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        SecureString MakeSecureString(string password)
        {
            unsafe
            {
                fixed (char* passwordChars = password)
                {
                    var securePassword = new SecureString(passwordChars, password.Length);
                    securePassword.MakeReadOnly();
                    return securePassword;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        void StartPrinterProcess(string filename)
        {
            ProcessStartInfo printProcessInfo = new ProcessStartInfo()
            {
                Verb = "print",
                CreateNoWindow = true,
                FileName = filename,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            Process printProcess = new Process();
            printProcess.StartInfo = printProcessInfo;

            try
            {
                printProcess.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show(this, "There was an error printing your stuff.\n\n" + e.Message);
                return;
            }

            printProcess.WaitForInputIdle();

            Thread.Sleep(3000);

            if (false == printProcess.CloseMainWindow())
            {
                printProcess.Kill();
                printProcess.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetUserDomain()
        {
            var wid = WindowsIdentity.GetCurrent();
            string userName = wid.Name.Split('\\')[1];
            string domain = wid.Name.Split('\\')[0];

            return new string[] { userName, domain};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        void StartPrinterProcessAuthenticated(string filename, string user, string domain, SecureString password)
        {
            var ud = GetUserDomain();

            //ImpersonationHelper.Impersonate(domain, ud[0], "!57jcl57!", delegate
            //{
                //Your code here 
                //Let's say file copy:

                var unp = WinInteropInterop.GetUniversalName(filename);
                var cwd = System.IO.Directory.GetCurrentDirectory();

                Console.WriteLine("CURRENT USER: " + user + "@" + domain + "\n\t\tPRINTING: " + filename + "\n\t\tUNP: " + unp);
                ProcessStartInfo printProcessInfo = new ProcessStartInfo()
                {
                    Verb = "print",
                    CreateNoWindow = true,
                    FileName = unp,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,

                    UserName = user + "@" + domain,
                    Domain = null,
                    Password = password,
                };

                Process printProcess = new Process();
                printProcess.StartInfo = printProcessInfo;

                try
                {
                    printProcess.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show(this, "There was an error printing your stuff.\n\n" + e.Message);
                    return;
                }

                printProcess.WaitForInputIdle();

                Thread.Sleep(3000);

                if (false == printProcess.CloseMainWindow())
                {
                    printProcess.Kill();
                    printProcess.Dispose();
                }
            //});
        }

        const string regStr = @"\(([^)]*)\)"; 
        static Regex reg = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string[] FindModuleIdInPDF(string file)
        {
            org.pdfclown.files.File pdfFile = new org.pdfclown.files.File(file);
            
            TextExtractor textExt = new TextExtractor(true, true);
            foreach(var page in pdfFile.Document.Pages)
            {
                var texts = textExt.Extract(page);
                foreach(var t in texts)
                {
                    var str = t.Value;
                    foreach(var s in str)
                    {
                        var token = Regex.Replace(s.Text, @"\s", "");
                        if(token.Contains("KR002"))
                        {
                            var match = reg.Match(token);
                            if (match.Success)
                            {
                                var modId = match.Value.Trim();
                                modId = modId.Remove(0, 1);
                                modId = modId.Remove(modId.Length - 1, 1);

                                var temp = modId.Split('D');
                                var modNum = temp[1];
                                modNum = modNum.Split('R')[1];
                                modNum = modNum.Split('G')[0];

                                var revNum = "0";
                                temp = modId.Split('_');
                                if (temp != null && temp.Length > 1)
                                    revNum = temp[1];

                                return new string[] { modNum, revNum };
                            }
                            ///else MessageBox.Show(this, "Mod token found but could not parse. " + match.Value);
                        }
                        else if(token.ToUpper().Contains("MOD"))
                        {
                            //we have a 'quick n dirty' id for our item. see if we can get a number for the module from it
                            var id = token.ToUpper().Replace("MOD", string.Empty);
                            id = id.Replace("M", string.Empty);


                            //at this point, if the format is "Mod XXX" or "M XXX" doesn't matter. we *should* have just our mod number
                            id = id.Trim();

                            if(int.TryParse(id, out int idNum))
                            {
                                return new string[] { id, null }; //we don't know the rev o we'll send null and the outter system will have to figure it out
                            }
                        }
                    }
                }

                /*
                var txt = TextExtractor.ToString(texts);
                var match = reg.Match(txt);
                if (match.Success)
                    MessageBox.Show(this, "matched the following string:\n\n" + match.Value);
                else MessageBox.Show(this, "No match found for \""+regStr+"\"");
                */
            }

            return new string[] { string.Empty, string.Empty };
        }

        static string fixedPath = @"X:\KR002_AVELIA\9_3D_2D_Design\REVISION\";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modId"></param>
        /// <returns></returns>
        string FindDrawingFromModId(string modId, string rev)
        {
            if(string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(rev))
            {
                return string.Empty;
            }

            //gather a list of all folders for a given mod, for any revision level
            int revCount = int.Parse(rev);
            while(revCount >= 0)
            {
                var dirs = Directory.GetDirectories(fixedPath, modId + "_BOM*_REV*"+revCount, SearchOption.TopDirectoryOnly);
                if (dirs.Length == 0)
                    Console.WriteLine("No directories found for mod " + modId + " at revision " + rev);
                foreach(var dir in dirs)
                {
                    //try all dirs that match
                    var files = Directory.GetFiles(dir, "kr002*" + modId + "*g_001*.pdf", SearchOption.AllDirectories);

                    if (files.Length > 1)
                        Console.WriteLine("Warning: For some reason there is more than one g-level drawing for mod " + modId + "Rev + " + rev);
                    foreach (var file in files)
                    {
                        //check for a pdf that is a g-level drawing
                        Console.WriteLine("Found a drawing file at: " + file);
                        return file;
                    }
                }
                revCount--;
            }
            
            
            return string.Empty;
        }
    }
}
