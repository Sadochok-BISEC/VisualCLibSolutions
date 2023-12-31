﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using COM_WindApplication.com;
using Excel = Microsoft.Office.Interop.Excel;

// COM Application
// The main class of the program describes the controls and their interaction with the user
// 
// Ilya Bisec - 13.11.23

namespace COM_WindApplication
{
    public partial class MainForm : Form
    {
        private String thisName = "COM Application";

        private Dictionary<string, List<string>> countryRegions = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> defaultCountryRegions;


        ComExcel comExcel = new ComExcel();


        public MainForm()
        {
            InitializeComponent();
        }

        #region Word Page
        // Creating document
        private void btn_CreateDoc_Click(object sender, EventArgs e)
        {
            var res = Properties.Settings.Default;

            ComWord comWord = new ComWord(tb_RecipientName.Text.ToString(), tb_ProjectName.Text.ToString(), tb_DepartmentName.Text.ToString(), tb_CompanyName.Text.ToString(),
                tb_SenderName.Text.ToString(), res.newTemplateDocPath, res.newDefaultFilePath);

            comWord.createTemplate();
        }

        // Demonstration of the document template for the user, how the document will look and which fields we change
        private void pnl_Preview_Paint(object sender, PaintEventArgs e)
        {
            var res = Properties.Settings.Default;

            if (tab_COM.SelectedTab == tab_COM.TabPages["tbp_Word"])
            {
                showProccessInfo("Word");

                // Showing privew proccess
                previewHandlerHost.Open(res.demoTemplate);
            }
        }

        // Checking the result of a previously created document
        private void btn_CheckResult_Click(object sender, EventArgs e)
        {
            var res = Properties.Settings.Default;
            var createdLastFile = res.newDefaultFilePath + "\\" + tb_RecipientName.Text + ".docx";

            if (tab_COM.SelectedTab == tab_COM.TabPages["tbp_Word"])
            {
                showProccessInfo("Word");

                // Showing privew proccess
                previewHandlerHost.Open(createdLastFile);
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            exitApplication();
        }
        #endregion

        #region Excel page
        // Demonstration of the document template for the user, how the document will look and which fields we change
        private void pnl_PreviewExcel_Paint(object sender, PaintEventArgs e)
        {
            var res = Properties.Settings.Default;

            if (tab_COM.SelectedTab == tab_COM.TabPages["tbp_Excel"])
            {
                showProccessInfo("Excel");

                // Showing privew proccess
                previreHandleHostExcel.Open(res.demoExcelTemplate);
            }
        }

        private void btn_CreateExcelTable_Click(object sender, EventArgs e)
        {
            comExcel.createTable();
        }

        private void btn_AddNote_Click(object sender, EventArgs e)
        {
            if (chekb_TurnOffComboboxDictionary.Checked)
            {
                comExcel.addNote(tb_NameCountry.Text, tb_NameRegion.Text, tb_MonthName.Text, tb_MonthTemperature.Text);
            }
            else
            {
                comExcel.addNote(cmb_NameCountry.SelectedItem.ToString(), cmb_NameRegion.SelectedItem.ToString(),
                    cmb_MonthName.SelectedItem.ToString(), tb_MonthTemperature.Text);
            }

                clearFields();
        }

        private void btn_TemperatureAverage_Click(object sender, EventArgs e)
        {
            comExcel.calculateAverageTemperature();
        }

        private void btn_CreateHistogram_Click(object sender, EventArgs e)
        {
            comExcel.createHistogram();
        }

        // Checking the result of a previously created document
        private void btn_CheckExcelResult_Click(object sender, EventArgs e)
        {
            var res = Properties.Settings.Default;
            var createdLastFile = res.newDefaultFilePath + "\\" + "TemperatureDocument" + ".xlsx";

            if (tab_COM.SelectedTab == tab_COM.TabPages["tbp_Excel"])
            {
                showProccessInfo("Excel");

                // Showing privew proccess
                previreHandleHostExcel.Open(createdLastFile);
            }
        }

        private void cmb_NameCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCountry = cmb_NameCountry.SelectedItem.ToString();

            // Getting a list of regions for the selected country from the dictionary
            List<string> temp_regions = countryRegions[selectedCountry];

            // Filling region combobox regions
            cmb_NameRegion.Items.Clear();
            foreach (string region in temp_regions)
            { cmb_NameRegion.Items.Add(region); }


        }

        // If manual input is disabled(chekb_TurnOffComboboxDictionary.UnChecked)
        // List of cities and countries to choose from in combobox
        // If the default file is not found, use the template with the default cities
        public void loadListOfCitiesToCombobox()
        {
            var res = Properties.Settings.Default;

            try
            {
                if (File.Exists(res.defaultCityFilePath))
                {
                    string[] lines = File.ReadAllLines(res.defaultCityFilePath);

                    // We iterate through each line in the file
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        string country = parts[0].Trim();
                        string cities = parts[1].Trim();

                        // We divide the list of cities into separate cities
                        string[] cityList = cities.Split(',');

                        // Creating a new list of cities and adding it to the dictionary
                        List<string> cityCollection = new List<string>();
                        foreach (string city in cityList) { cityCollection.Add(city.Trim()); }

                        countryRegions.Add(country, cityCollection);
                    }

                    // Filling combobox countries value
                    foreach (string countryName in countryRegions.Keys)
                        cmb_NameCountry.Items.Add(countryName);
                }
                else
                {
                    defaultCountryRegions = new Dictionary<string, List<string>>
                    {
                        {"Россия", new List<string>{"Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Казань"}},
                        {"Украина", new List<string>{"Киев", "Харьков", "Одесса", "Днепр", "Донецк", "Запорожье"}},
                        {"Беларусь", new List<string>{ "Минск", "Борисов", "Солигорск", "Молодечно", "Жодино", "Слуцк"}},
                        {"Польша", new List<string>{ "Варшава", "Краков", "Люблин", "Белосток", "Торун", "Рыбник"}},
                        {"Сербия", new List<string>{ "Белград", "Вог", "Кикинда", "Крагувац", "Лесковац", "Лозница"}},

                    };

                    countryRegions = defaultCountryRegions;

                    // Filling combobox countries value
                    foreach (string countryName in defaultCountryRegions.Keys)
                        cmb_NameCountry.Items.Add(countryName);
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString(), "Cities dictionary not availeble, using manual input please", MessageBoxButtons.OK); }
        }

        // Cleans the application fields (except for the month), the temperature field 
        // is always cleaned after making entries in the Excel spreadsheet.
        private void clearFields()
        {
            tb_MonthTemperature.Text = "";

            if (chekb_ClearExcelFiledsAfterAddNote.Checked)
            {
                if (chekb_TurnOffComboboxDictionary.Checked)
                {
                    tb_NameCountry.Text = "";
                    tb_NameRegion.Text = "";
                    tb_MonthName.Text = "";
                }
                else
                {
                    cmb_NameCountry.Text = "";
                    cmb_NameRegion.Text = "";
                    //cmb_MonthName.Text = "";
                }
            }
        }

        private void btn_ExitExelApp_Click(object sender, EventArgs e)
        {
            exitApplication();
        }

        #endregion

        // Main form load
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Gets paths from resources
            var res = Properties.Settings.Default;

            tb_TemplateDocPath.Text = res.newTemplateDocPath;
            tb_NewDefFilePath.Text = res.newDefaultFilePath;
            tb_DemoTemplate.Text = res.demoTemplate;
            tb_DemoExcelTemplate.Text = res.demoExcelTemplate;
            tb_PathToTxtCities.Text = res.defaultCityFilePath;

            chekb_TurnOffComboboxDictionary.Checked = res.manualInput;
            chekb_ClearExcelFiledsAfterAddNote.Checked = res.clearExcelFields;

            loadListOfCitiesToCombobox();
        }


        #region Settings page
        // If Checked, opening advanced settings
        private void chekb_AdvancedSettings_CheckedChanged(object sender, EventArgs e)
        {
            var res = Properties.Settings.Default;

            if (chekb_AdvancedSettings.Checked)
            {
                lb_DemoTemplate.Visible = true;
                tb_DemoTemplate.Visible = true;
                btn_DemoTemplate.Visible = true;

                lb_DemoExcelTemplate.Visible = true;
                tb_DemoExcelTemplate.Visible = true;
                btn_DemoExcelTemplate.Visible = true;

                lb_PathToTxtCities.Visible = true;
                tb_PathToTxtCities.Visible = true;
                btn_PathToTxtCities.Visible = true;

                chekb_ClearExcelFiledsAfterAddNote.Visible = true;

                res.advanceSettings = true;
            }
            else
            {
                lb_DemoTemplate.Visible = false;
                tb_DemoTemplate.Visible = false;
                btn_DemoTemplate.Visible = false;

                lb_DemoExcelTemplate.Visible = false;
                tb_DemoExcelTemplate.Visible = false;
                btn_DemoExcelTemplate.Visible = false;

                lb_PathToTxtCities.Visible = false;
                tb_PathToTxtCities.Visible = false;
                btn_PathToTxtCities.Visible = false;

                chekb_ClearExcelFiledsAfterAddNote.Visible = false;

                res.advanceSettings = false;
            }
        }

        // If checked, disable the use of dictionaries and enable manual input
        private void chekb_TurnOffComboboxDictionary_CheckedChanged(object sender, EventArgs e)
        {
            var res = Properties.Settings.Default;

            if (chekb_TurnOffComboboxDictionary.Checked)
            {
                cmb_MonthName.Visible = false;
                cmb_NameRegion.Visible = false;
                cmb_NameCountry.Visible = false;

                tb_MonthName.Visible = true;
                tb_NameRegion.Visible = true;
                tb_NameCountry.Visible = true;

                res.manualInput = true;
            }
            else
            {
                cmb_MonthName.Visible = true;
                cmb_NameRegion.Visible = true;
                cmb_NameCountry.Visible = true;

                tb_MonthName.Visible = false;
                tb_NameRegion.Visible = false;
                tb_NameCountry.Visible = false;

                res.manualInput = false;
            }
        }

        //  If checked, cleaning excel fields after add note
        private void chekb_ClearExcelFiledsAfterAddNote_CheckedChanged(object sender, EventArgs e)
        {
            var res = Properties.Settings.Default;

            if (chekb_ClearExcelFiledsAfterAddNote.Checked)
                res.clearExcelFields = true;
            else
                res.clearExcelFields = false;
        }

        // Sets new default resources path
        private void btn_SaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.newTemplateDocPath = tb_TemplateDocPath.Text;
            Properties.Settings.Default.newDefaultFilePath = tb_NewDefFilePath.Text;
            Properties.Settings.Default.demoTemplate = tb_DemoTemplate.Text;
            Properties.Settings.Default.demoExcelTemplate = tb_DemoExcelTemplate.Text;

            Properties.Settings.Default.defaultCityFilePath = tb_PathToTxtCities.Text;

            Properties.Settings.Default.manualInput = chekb_TurnOffComboboxDictionary.Checked;
            Properties.Settings.Default.advanceSettings = chekb_AdvancedSettings.Checked;


            Properties.Settings.Default.Save();
        }

        // Changing the default template doc path
        private void btn_TemplateDocPath_Click(object sender, EventArgs e)
        {
            changePathForFiles(tb_TemplateDocPath);
        }

        // Changing the default demonstrate template doc path
        private void btn_DemoTemplate_Click(object sender, EventArgs e)
        {
            changePathForFiles(tb_DemoTemplate);
        }

        // Changing the default demonstrate template excel path
        private void btn_DemoExcelTemplate_Click(object sender, EventArgs e)
        {
            changePathForFiles(tb_DemoExcelTemplate);
        }

        // Changing the default path for new documents 
        private void btn_NewDefFilePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();

            try
            {
                if (browserDialog.ShowDialog() == DialogResult.OK)
                {
                    tb_NewDefFilePath.Text = browserDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Template file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Changing the default text dictionary path
        private void btn_PathToTxtCities_Click(object sender, EventArgs e)
        {
            changePathForFiles(tb_PathToTxtCities);
        }
        #endregion

        #region Additional functions
        // Showing the applicaton name + the process name
        private void showProccessInfo(string preoccess_name)
        {
            if (chekb_AdvancedSettings.Checked)
            {
                this.Text = thisName + "Current proccess: " + preoccess_name + ": " + previewHandlerHost.CurrentPreviewHandler.ToString();
            }
        }

        // Chanching default path for doc files
        private void changePathForFiles(TextBox changed_path)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    changed_path.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Template file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // True exit from application with to release resources
        private void exitApplication()
        {
            previewHandlerHost.UnloadPreviewHandler();
            previreHandleHostExcel.UnloadPreviewHandler();
            this.Close();
            System.Windows.Forms.Application.Exit();
        }

        #endregion

    }
}
