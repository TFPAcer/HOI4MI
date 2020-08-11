﻿using System;
using StateEditor.Entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StateEditor.Manager;
using HOI4MI.Forms;

namespace StateEditor.Forms
{
    public partial class MainForm : Form
    {
        private string basePath = @"A:\Files\Documents\Paradox Interactive\Hearts of Iron IV\mod\thirdreich";

        private ResultDialog resultDialog;
        private ResourceEditorForm resourceEditor;
        private StateSplitterForm stateSplitter;
        private StateEditorForm stateEditor;

        private readonly LocalisationManager localeManager;
        private readonly CountryManager countryManager;
        private readonly ResourceManager resourceManager;

        public MainForm() {
            InitializeComponent();
            Province.SetBasePath(basePath);
            State.SetBasePath(basePath);
            localeManager = new LocalisationManager(basePath);
            countryManager = new CountryManager(basePath, localeManager);
            resourceManager = new ResourceManager(basePath, countryManager);

            Reload();
        }

        private void OpenForm(Form f) {
            mainPanel.Controls.Clear();
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(f);
            mainPanel.Tag = f;
            f.BringToFront();
            f.Show();
        }

        private void Reload() {
            localeManager.ReloadLocalisation();
            Parser.SetLocalisationManager(localeManager);
            Province.ReloadAll();
            State.ReloadAll();
            countryManager.ReloadCountries();
            resourceManager.ReloadResourceMap(true, false);


            stateSplitter = new StateSplitterForm(localeManager, countryManager, resourceManager);
            resourceEditor = new ResourceEditorForm(localeManager, countryManager, resourceManager);
            stateEditor = new StateEditorForm(localeManager, countryManager, resourceManager);
            resultDialog = new ResultDialog();
            resultDialog.Location = new Point(Location.X + Width / 2, Location.Y + Height / 2);
        }

        private void writeButton_Click(object sender, EventArgs e) {

            State.Save();

            resultDialog.SetMessage(State.Status);
            resultDialog.ShowDialog();
        }

        private void reloadButton_Click(object sender, EventArgs e) {
            mainPanel.Controls.Clear();
            Reload();
        }

        #region forms
        private void resourceEditorFormButton_Click(object sender, EventArgs e) {
            OpenForm(resourceEditor);
        }
        private void stateSplitterFormButton_Click(object sender, EventArgs e) {
            OpenForm(stateSplitter);
        }

        private void stateEditorFormButton_Click(object sender, EventArgs e) {

        }
        #endregion
    }
}
