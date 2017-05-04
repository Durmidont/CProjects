using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextLibrary;

using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using ScopeLoad;
using SerialPorts;
using FormatsConvert;
using System.Diagnostics;


namespace UPTF_Scope
{
    public partial class MainForm : Form
    {
        public static ElementsStrings elementStrings;// = new ElementsStrings("strings.xml");
        public static int Language { get; set; } = 1;
        string appName = "UPTF";
        
        //public List<DataRecord> DataRecords;
        public static string ParameterName(int ParamNum)
        {
            try
            {
                return (elementStrings.Parameters[ParamNum].ParamNames[Language]);
            }
            catch
            {
                return ("Error");
            }
        }
   

        /// <summary>
        /// Возвращает название параметра, где пробел заменен на символ переноса каретки
        /// </summary>
        public static string ParameterNameMultiString(int ParamNum)
        {
            string strs = "";
            string[] paramListstr;
            char[] delimiterChars = {' '};
            paramListstr = ParameterName(ParamNum).Split(delimiterChars);
            int i;
            for (i = 0; i < paramListstr.Length; i++)
            {
                strs= strs+paramListstr[i]+"\n";
            }
            return strs;
        }
        
        SerialDataProvider _dataProvider;

        ComSetForm comsetForm;// = new ComSetForm();
        public MainForm()
        {
            InitializeComponent();
            



        }


#region РАБОТА С COM-ПОРТОМ
        void SetConnection()
        {
            try
            {
                
                if (_dataProvider != null &&_dataProvider.IsOpen) { return; }
            }
            catch
            {
            }
            _dataProvider = new SerialDataProvider(ComsetClass.ComPortName, 1, ComsetClass.BaudRate, ComsetClass.SerialPortParity, ComsetClass.SerialPortStopBits);
            _dataProvider.CrashSerialPort += new EventHandler(dataProvider_CrashSerialPort);
            if (!_dataProvider.TryOpen())
            {
                MessageBox.Show(ParameterName(17), appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            
            if (_dataProvider.IsOpen)
            {
                connectBtn.Enabled = false;
                disconnectBtn.Enabled = true;
                comsetBtn.Enabled = false;

                nowLoadTimeStamps = false;
                loadingConfig = false;
               
                cicle = 0;
                
                countSys = ComsetClass.DataRecords.Count;
                configLoaded = new bool[countSys];
                loadScopeNum = new int[countSys];
                

                //LoadConfig();

                loadScopeTimer.Enabled = true;
            }
            else
            {
                MessageBox.Show(ParameterName(17), appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataProvider_CrashSerialPort(object sender, EventArgs e)
        {
            MessageBox.Show(ParameterName(17), appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ComPortClosed(object senderm, EventArgs e)
        {
            
            Action ClosePort = () =>
            {
               
                changeCellsColor(true);
            };
            if (InvokeRequired)
                Invoke(ClosePort);
            else
                ClosePort();
            
            
        }

        
   
        

        
#endregion

        private void connectBtn_Click(object sender, EventArgs e)
        {

            SetConnection();
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            //ModbusSerialPort.ClosePort();
            loadScopeTimer.Enabled = false;
            _dataProvider.Close();
            connectBtn.Enabled = true;
            disconnectBtn.Enabled = false;
            comsetBtn.Enabled = true;
            changeCellsColor(true);
        }

        private void comsetBtn_Click(object sender, EventArgs e)
        {
            comsetForm = new ComSetForm();
            try
            {
                comsetForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        
        


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if
                    (
                        MessageBox.Show(ParameterName(10), this.Text,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                    )
                {
                    e.Cancel = false;
                }
                else
                e.Cancel = true;
            }
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                elementStrings = new ElementsStrings("strings.xml");
                ComsetClass.errAppExit = ParameterName(13);
                ComsetClass.errCreateFolder = ParameterName(15);
                ComsetClass.errFoundFile = ParameterName(12);
                ComsetClass.errRead = ParameterName(14);
                ComsetClass.errWrite = ParameterName(16);
                ComsetClass.LoadSettingsFromFile();
                ComsetClass.LoadUPTFFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            Language = 1;

            
            connectBtn.Text = ParameterNameMultiString(0);
            disconnectBtn.Text = ParameterNameMultiString(1);
            button3.Text = ParameterNameMultiString(3);
            comsetBtn.Text = ParameterNameMultiString(2);
            Cursor.Hide();
                       

            var column1 = new DataGridViewTextBoxColumn();
            var column2 = new DataGridViewTextBoxColumn();
            var column3 = new DataGridViewTextBoxColumn();
            var column4 = new DataGridViewTextBoxColumn();
            var column5 = new DataGridViewProgressColumn();
            var column6 = new DataGridViewButtonColumn();

            // 
            // Column1
            // 



            column1.HeaderText = ParameterName(18); // "Адр.";
            column1.MinimumWidth = 50;
            column1.Name = "Address";
            column1.ReadOnly = true;
            column1.Width = 55;
            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column1.SortMode = DataGridViewColumnSortMode.NotSortable;
            column1.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // 
            // Column2
            // 
            column2.HeaderText = ParameterName(19); // "Название";
            column2.Name = "Path";
            column2.ReadOnly = true;
            column2.Width = 198;
            column2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            column3.HeaderText = ParameterName(20); //"Кол.";
            column3.Name = "Number";
            column3.ReadOnly = true;
            column3.Width = 50;
            column3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column3.SortMode = DataGridViewColumnSortMode.NotSortable;
            column3.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // 
            // Column4
            // 
            column4.HeaderText = ParameterName(21); // "Нов.";
            column4.Name = "NumberNew";
            column4.ReadOnly = true;
            column4.Width = 50;
            column4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column4.SortMode = DataGridViewColumnSortMode.NotSortable;
            column4.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 
            // Column5
            // 
            column5.HeaderText = ParameterName(22); // "Загрузка";
            column5.Name = "Progress";
            column5.ReadOnly = true;
            column5.Width = 150;
            column5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column5.SortMode = DataGridViewColumnSortMode.NotSortable;
            column5.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 
            // Column6
            // 
            column6.HeaderText = ParameterName(23); // "Показать";
            column6.Name = "Column6";
            column6.ReadOnly = true;
            column6.Text = "Показать";
            column6.UseColumnTextForButtonValue = true;
            column6.Width = 110;

            //dataGridView1.ColumnCount = 6;
            dataGridView1.Columns.AddRange(column1, column2, column3, column4, column5, column6);

            

            
            

            
           // serialPort = new AsynchSerialPort();
           // serialPort.SerialPortMode = SerialPortModes.RSMode;
            
            //           serialPort.SlaveAddr = 1;
            //           serialPort.Open();
           // serialPort.PortClosed += ComPortClosed;
            

           
            
            oscilCount = 0;
            cicle = 0;
            
            countSys = ComsetClass.DataRecords.Count;
            configLoaded = new bool [countSys];
            scopeConfigLoader = new ScopeConfigLoader[countSys];
            scopeConfig = new ScopeConfig[countSys];
            
            scopeStatusesLoader = new ScopeStatusesLoader[countSys];
            
            loadScopeNum = new int[countSys]; 
            loadedScopeNum = new int[countSys];
            scopeLoader = new ScopeLoader[countSys];
            ScopeSysType.InitScopeSysType();
            loadScopeTimer = new System.Windows.Forms.Timer();
            loadScopeTimer.Interval = 1000;
            
            loadScopeTimer.Tick += new EventHandler(loadScopeTimer_Tick);

            scopePath = new string[countSys];
            for (int i = 0; i < countSys; i++)
            {

                scopePath[i] = @"\" + @ComsetClass.DataRecords[i].Name + @"\";


            }

            foreach (DataRecord dr in ComsetClass.DataRecords)
            {
                dr.Number = GetFilesCount(ComsetClass.ArhivePath + @"\" + dr.Name + @"\");
                dataGridView1.Rows.Add(dr.Address, dr.Name, dr.Number, dr.NumberNew, dr.Progress);
            }
            dataGridView1.ClearSelection();
            connectBtn_Click(null, null);

        }

        

        string CalcApplPath()
        {
            string str = Application.ExecutablePath;
            string str2 = "";
            int i = str.Length;
            char ch = (char)0;

            do
            {
                ch = str[i - 1];
                if (ch != 0x5C) { i = i - 1; }
            } while ((ch != 0x5C) && (i > 0));

            str2 = str.Substring(0, i);
            return (str2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
            Process proc = new Process();
            proc.StartInfo.FileName = CalcApplPath() + "ScopeViewFast.exe";
            if(scopePathFull !="")
                proc.StartInfo.Arguments = "\""+ scopePathFull + "\"";
            proc.Start();
            oscilCount = 0;
            button3.Text = ParameterName(31);
            button3.BackColor = SystemColors.Control;
            scopePathFull = "";
            */

            FormExit frmEx = new FormExit();
            DialogResult dlgRes = frmEx.ShowDialog();
            switch(dlgRes)
            {
                case DialogResult.Yes:
                    Application.Exit();
                    break;
                case DialogResult.No:
                    System.Diagnostics.Process.Start("shutdown", "/s /t 0");
                    break;
                case DialogResult.Abort:
                    this.WindowState = FormWindowState.Minimized;
                    break;
            }
           
            // Close();
        }

        //********************************* ЗАГРУЗКА ОСЦИЛЛОГОГРАММ ****************************************************************//
        //**************************************************************************************************************************//
        bool[] configLoaded ;
        ScopeConfigLoader[] scopeConfigLoader;
        ScopeConfig[] scopeConfig;
        bool nowLoadTimeStamps = false;//{ false, false, false, false, false, false };
        ScopeStatusesLoader[] scopeStatusesLoader;
        System.Windows.Forms.Timer loadScopeTimer;
        int[] loadScopeNum;
        int[] loadedScopeNum;
        ScopeLoader[] scopeLoader;
        string[] scopePath;
        string scopePathFull = "";
        int cicle = 0;
        
        bool loadingConfig = false;
        int oscilCount = 0;
        int countSys=0;

        void LoadConfig()
        {
           
             
                    loadingConfig = true;
                    
                    scopeConfigLoader[cicle] = new ScopeConfigLoader(_dataProvider, ComsetClass.DataRecords[cicle].Address);
                    scopeConfigLoader[cicle].OnLoadingComplete += new ScopeConfigLoader.LoadingCompleter(scopeConfigLoader_OnLoadingComplete);
          

        }
        void scopeConfigLoader_OnLoadingComplete(bool LoadingOk)
        {
            changeCellColor(cicle, LoadingOk);


            if (LoadingOk)
            {
                //MessageBox.Show("Загрузка параметров осцилогрофа" + cicle2, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                scopeConfig[cicle] = scopeConfigLoader[cicle].ScopeConfig;
                configLoaded[cicle] = true;

            }
            loadingConfig = false;
            cicle++;
        }

        //*********************** ЗАГРУЗКА СТАТУСОВ ОСЦИЛЛОГРАММ *************************************************//
        //********************************************************************************************************//
        private void loadScopeTimer_Tick(object sender, EventArgs e)
        {
            if (_dataProvider.IsOpen)
            {

                if (cicle > countSys - 1)
                    cicle = 0;

                
               

       /*         MessageBox.Show("Загрузка конфигов:" + configLoaded[0] + configLoaded[1] + 
                    configLoaded[2] + configLoaded[3] + configLoaded[4] + configLoaded[5] ,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                */
                if (!nowLoadTimeStamps && !loadingConfig)
                {
                    if( configLoaded[cicle])
                         LoadStatusesAsynch();
                    else
                        LoadConfig(); 

                }
                

            }
        }

        void LoadStatusesAsynch()
        {
            
            nowLoadTimeStamps = true;
            
            scopeStatusesLoader[cicle] = new ScopeStatusesLoader(_dataProvider, ComsetClass.DataRecords[cicle].Address, scopeConfig[cicle]);
            scopeStatusesLoader[cicle].OnLoadingComplete += new ScopeStatusesLoader.LoadingCompleter(scopeStatusesLoader_OnLoadingComplete);
        }

        void scopeStatusesLoader_OnLoadingComplete(bool LoadingOk)
        {
            changeCellColor(cicle, LoadingOk);
            if (!LoadingOk)
            {
                nowLoadTimeStamps = false;
                cicle++;
                return;
            }
            else
            {


                //MessageBox.Show("Загрузка статусов успешно! количество:"+ cicle, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                for (int i = 0; i < scopeConfig[cicle].ScopeCount; i++)
                {
                    if (scopeStatusesLoader[cicle].OscilsStatus[i] == 4)
                    {

                        //MessageBox.Show("Загрузка статусов успешно! Статус:" + scopeStatusesLoader[cicle].OscilsStatus[i], "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        loadScopeNum[cicle] = i;
                        
                        scopeConfigLoader[cicle] = new ScopeConfigLoader(_dataProvider, ComsetClass.DataRecords[cicle].Address);
                        scopeConfigLoader[cicle].OnLoadingComplete += new ScopeConfigLoader.LoadingCompleter(scopeConfigLoader_OnLoadingComplete2);

                        return;
                    }
                }
            }
            nowLoadTimeStamps = false;
            cicle++;
        }
        void scopeConfigLoader_OnLoadingComplete2(bool LoadingOk)
        {

            if (!LoadingOk)
            {
                changeCellColor(cicle, LoadingOk);
                nowLoadTimeStamps = false;
                cicle++;

            }
            else
            {
                scopeConfig[cicle] = scopeConfigLoader[cicle].ScopeConfig;
                
                scopeLoader[cicle] = new ScopeLoader(_dataProvider, ComsetClass.DataRecords[cicle].Address, scopeConfig[cicle], loadScopeNum[cicle]);
                scopeLoader[cicle].OnProcessUpdate += new ScopeLoader.ProcessUpdater(scopeLoader_OnProcessUpdate);
                scopeLoader[cicle].OnLoadingComplete += new ScopeLoader.LoadingCompleter(scopeLoader_OnLoadingComplete);

            }

        }
        void scopeLoader_OnProcessUpdate(int Value)
        {
            //progressBar1.Value = Value;
            changeProgress(cicle, Value);
        }
        void scopeLoader_OnLoadingComplete(bool LoadingOk)
        {

            if (!LoadingOk)
            {
                changeCellColor(cicle, LoadingOk);
                changeProgress(cicle, 0);
                nowLoadTimeStamps = false;
                cicle++;
                return;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(ComsetClass.ArhivePath + scopePath[cicle]);
                    //MessageBox.Show("Создание директории!"+ scopePath[cicle], "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _dataProvider.SetData(ComsetClass.DataRecords[cicle].Address, (ushort)(ScopeSysType.OscilStatusAddr + loadScopeNum[cicle]), null, 0);
                string str = scopeStatusesLoader[cicle].OscilTitls[loadScopeNum[cicle]];
                //MessageBox.Show("Имя осцилограммы:" + str, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                str = str.Replace("/", "_").Replace(":", "_").Replace(".", "_");
                
                ComsetClass.DataRecords[cicle].Path = scopePathFull = ComsetClass.ArhivePath + scopePath[cicle] + str + ".txt";
                //MessageBox.Show("Имя файла:" + scopePathFull, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                oscilCount++;
                loadedScopeNum[cicle]++;
                
                /*
                Action CollorChange = () =>
                {
                    button3.BackColor = System.Drawing.Color.Lime;
                    button3.Text = ParameterName(31)+ "("+ oscilCount.ToString() + ")";
                };
                if (InvokeRequired)
                    Invoke(CollorChange);
                else
                    CollorChange();

            */
                ScopeFile scopeFile = new ScopeFile(scopePathFull, scopeLoader[cicle].DownloadedData, scopeConfig[cicle], scopeStatusesLoader[cicle].OscilTitls[loadScopeNum[cicle]]);
                scopeFile.OnCreateComplete += new ScopeFile.CreatingCompleter(scopeFile_OnCreateComplete);
            }
        }

        void scopeFile_OnCreateComplete(bool CreateOk)
        {
            //MessageBox.Show("Сохранение успешно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            changeLabelsStrip(cicle, loadedScopeNum[cicle]);
            changeProgress(cicle, 0);
            nowLoadTimeStamps = false;
            cicle++;
        }


        
        void changeProgress(int Num, int Value)
        {

            
            ComsetClass.DataRecords[Num].Progress = (int) Math.Truncate(Value / 655.35);
            dataGridView1["Progress", Num].Value = ComsetClass.DataRecords[Num].Progress;
            /*
          //MessageBox.Show("Сохранение успешно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
          ToolStripProgressBar pb;

          pb = statusStrip1.Items[Num*2 + 1] as ToolStripProgressBar;

          pb = (ToolStripProgressBar)(ADSPLibrary.Functions.FindComponent(this, "toolStripProgressBar" + (Num + 1).ToString()));
          if (pb != null)
          {
              //pb.Value = Value+1;
              pb.Value = Value;
          }

  */

        }
        void changeLabelsStrip(int Num, int Value)
        {
           
            dataGridView1["Number", Num].Value = GetFilesCount(ComsetClass.ArhivePath + scopePath[Num]); 
            dataGridView1["NumberNew", Num].Value = Value;
            /*
            //MessageBox.Show("Сохранение успешно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ToolStripStatusLabel sl;

            sl = statusStrip1.Items[Num * 2] as ToolStripStatusLabel;
            if (sl != null)
            {
                //pb.Value = Value+1;
                sl.Text = statusLabel[i].Text = "Осц.:" + GetFilesCount(scopePath[i]).ToString()+ "(" + Value.ToString() + ")";
            }
            */
        }
        void changeCellColor(int Num, bool Value)
        {
            if(Value)
                dataGridView1["Address", Num].Style.BackColor = Color.White; 
            else
                dataGridView1["Address", Num].Style.BackColor = Color.Red;
        }

        void changeCellsColor( bool Value)
        {
            for (int Num = 0; Num < dataGridView1.Rows.Count; Num++)
            {
                if (Value)
                    dataGridView1["Address", Num].Style.BackColor = Color.White;
                else
                    dataGridView1["Address", Num].Style.BackColor = Color.Red;

                dataGridView1["Progress", Num].Value = 0;
            }
        }
        int GetFilesCount(string dir_path)
        {
            int i = 0;
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(dir_path);
            if (directoryInfo.Exists)
            {

                // ищем во всех папках                
                i = directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Length;



            }
            return i;

        }
       

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ClearSelection();
            if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                if(ComsetClass.ArhiveFlag)
                    System.Diagnostics.Process.Start("explorer", ComsetClass.ArhivePath + scopePath[e.RowIndex]);
                else
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = CalcApplPath() + "ScopeViewer.exe";
                    if (ComsetClass.DataRecords[e.RowIndex].Path != "")
                        proc.StartInfo.Arguments = "\"" + ComsetClass.DataRecords[e.RowIndex].Path + "\"";
                    proc.Start();
                }
            }
        }
    }
}
