﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using StateEditor;
using StateEditor.Entity;
using StateEditor.Manager;
using StateEditor.Util;

namespace HOI4MI.Forms {
    public partial class StateEditorForm : Form {

        private readonly LocalisationManager localeManager;
        private readonly ResourceManager resourceManager;

        private State currentState;

        public StateEditorForm(LocalisationManager lm, ResourceManager rm) {
            InitializeComponent();

            localeManager = lm;
            resourceManager = rm;

            SetDataSources();
        }

        private void Reload() {
            localeManager.ReloadLocalisation();
            Parser.SetLocalisationManager(localeManager);
            Province.ReloadAll();
            State.ReloadAll();
            Country.ReloadAll();
            resourceManager.ReloadResourceMap(true, false);
        }

        private void SetDataSources() {
            selectList.Items.Clear();
            for (int i = 0; i < State.Count; i += 100) {
                selectList.Items.Add(i);
            }
            stateCategoryInput.DataSource = Enum.GetValues(typeof(StateCategory));
            stateOwnerInput.DataSource = Country.Countries;
            stateCoreInput.DataSource = Country.Countries;
        }

        private void countryList_SelectedIndexChanged(object sender, EventArgs e) {
            int startId = (int)selectList.SelectedItem;
            stateList.DataSource = State.Find(s => s.id >= startId && s.id < startId+100);
        }

        private void stateList_SelectedIndexChanged(object sender, EventArgs e) {
            currentState = (State)stateList.SelectedItem;
            testBox.Text = ((State)stateList.SelectedItem).ToStringVerbose();

            stateNameInput.Text = $"{currentState.LocalisedName}";

            stateManpowerInput.Value = currentState.Manpower;

            stateCategoryInput.SelectedItem = currentState.Category;

            stateOilInput.Value = (decimal)currentState.Resources.Oil;
            stateRubberInput.Value = (decimal)currentState.Resources.Rubber;
            stateSteelInput.Value = (decimal)currentState.Resources.Steel;
            stateTungstenInput.Value = (decimal)currentState.Resources.Tungsten;
            stateAluminiumInput.Value = (decimal)currentState.Resources.Aluminium;
            stateChromiumInput.Value = (decimal)currentState.Resources.Chromium;

            stateOwnerInput.SelectedItem = Country.Get(currentState.Owner);

            stateCoreInput.SelectedItems.Clear();
            foreach (string s in currentState.Cores) {
                Country c = Country.Get(s);
                stateCoreInput.SelectedItems.Add(c);
            }

            stateInfrastructureInput.Value = currentState.Infrastructure;
            stateCivFactoriesInput.Value = currentState.CivillianFactories;
            stateMilFactoriesInput.Value = currentState.MilitaryFactories;
            stateDockyardsInput.Value = currentState.Dockyards;
            stateRefineriesInput.Value = currentState.Refineries;
            stateSilosInput.Value = currentState.Silos;
            stateReactorsInput.Value = currentState.Reactors;
            stateAirbaseInput.Value = currentState.Airbases;
            stateRadarInput.Value = currentState.Radar;
            stateRocketsInput.Value = currentState.Rockets;
            stateAntiairInput.Value = currentState.Antiair;
        }

        private void stateSaveButton_Click(object sender, EventArgs e) {
            //old state
            currentState.LocalisedName = stateNameInput.Text;
            currentState.Manpower = (int)stateManpowerInput.Value;
            currentState.Category = (StateCategory)stateCategoryInput.SelectedItem;
            currentState.Resources.Oil = (double)stateOilInput.Value;
            currentState.Resources.Rubber = (double)stateRubberInput.Value;
            currentState.Resources.Steel = (double)stateSteelInput.Value;
            currentState.Resources.Tungsten = (double)stateTungstenInput.Value;
            currentState.Resources.Aluminium = (double)stateAluminiumInput.Value;
            currentState.Resources.Chromium = (double)stateChromiumInput.Value;

            currentState.Owner = stateOwnerInput.SelectedItem.ToString();
            foreach (object o in stateCoreInput.SelectedItems) {
                currentState.Cores.Add(o.ToString());
            }

            currentState.Infrastructure = (int)stateInfrastructureInput.Value;
            currentState.CivillianFactories = (int)stateCivFactoriesInput.Value;
            currentState.MilitaryFactories = (int)stateMilFactoriesInput.Value;
            currentState.Dockyards = (int)stateDockyardsInput.Value;
            currentState.Refineries = (int)stateRefineriesInput.Value;
            currentState.Silos = (int)stateSilosInput.Value;
            currentState.Reactors = (int)stateReactorsInput.Value;
            currentState.Airbases = (int)stateAirbaseInput.Value;
            currentState.Radar = (int)stateRadarInput.Value;
            currentState.Rockets = (int)stateRocketsInput.Value;
            currentState.Antiair = (int)stateAntiairInput.Value;

        }

        private void resetButton_Click(object sender, EventArgs e) {
            stateNameInput.Text = $"{currentState.LocalisedName}";

            stateManpowerInput.Value = currentState.Manpower;

            stateCategoryInput.SelectedItem = currentState.Category;

            stateOilInput.Value = (decimal)currentState.Resources.Oil;
            stateRubberInput.Value = (decimal)currentState.Resources.Rubber;
            stateSteelInput.Value = (decimal)currentState.Resources.Steel;
            stateTungstenInput.Value = (decimal)currentState.Resources.Tungsten;
            stateAluminiumInput.Value = (decimal)currentState.Resources.Aluminium;
            stateChromiumInput.Value = (decimal)currentState.Resources.Chromium;

            stateOwnerInput.SelectedItem = Country.Get(currentState.Owner);

            stateCoreInput.SelectedItems.Clear();
            foreach (string s in currentState.Cores) {
                Country c = Country.Get(s);
                stateCoreInput.SelectedItems.Add(c);
            }

            stateInfrastructureInput.Value = currentState.Infrastructure;
            stateCivFactoriesInput.Value = currentState.CivillianFactories;
            stateMilFactoriesInput.Value = currentState.MilitaryFactories;
            stateDockyardsInput.Value = currentState.Dockyards;
            stateRefineriesInput.Value = currentState.Refineries;
            stateSilosInput.Value = currentState.Silos;
            stateReactorsInput.Value = currentState.Reactors;
            stateAirbaseInput.Value = currentState.Airbases;
            stateRadarInput.Value = currentState.Radar;
            stateRocketsInput.Value = currentState.Rockets;
            stateAntiairInput.Value = currentState.Antiair;
        }
    }
}
