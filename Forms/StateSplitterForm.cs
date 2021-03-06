﻿using HOI4MI.Entity;
using HOI4MI.Manager;
using HOI4MI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4MI.Forms {
    public partial class StateSplitterForm : Form {

        private readonly ResourceManager resourceManager;

        private State currentState;

        public StateSplitterForm(ResourceManager rm) {
            InitializeComponent();
            resourceManager = rm;

            SetDataSources();

        }

        private void Reload() {
            Localisation.Reload();
            Province.ReloadAll();
            State.ReloadAll();
            Country.ReloadAll();
            resourceManager.ReloadResourceMap(true, false);
            
        }

        private void SetDataSources() {
            stateList.DataSource = State.States;
            oldStateCategoryInput.DataSource = Enum.GetValues(typeof(StateCategory));
            newStateCategoryInput.DataSource = Enum.GetValues(typeof(StateCategory));
            oldStateOwnerInput.DataSource = Country.Countries;
            newStateOwnerInput.DataSource = Country.Countries;
            oldStateCoreInput.DataSource = Country.Countries;
            newStateCoreInput.DataSource = Country.Countries;
        }

        private void debugButton_Click(object sender, EventArgs e) {
            State.Write((State)stateList.SelectedItem);
        }

        private void stateList_SelectedIndexChanged(object sender, EventArgs e) {
            
            //populate province list
            provinceSelectList.Items.Clear();
            currentState = (State)stateList.SelectedItem;
            foreach (Province p in currentState.Provinces) {
                provinceSelectList.Items.Add(p);
            }

            oldStateNameInput.Text = $"{currentState.LocalisedName}";
            newStateNameInput.Text = $"STATE {State.Count+1}";

            oldStateManpowerInput.Maximum = currentState.Manpower;
            newStateManpowerInput.Maximum = currentState.Manpower;
            oldStateManpowerInput.Value = currentState.Manpower;

            oldStateCategoryInput.SelectedItem = currentState.Category;
            newStateCategoryInput.SelectedItem = currentState.Category;

            oldStateOilInput.Value = (decimal)currentState.Resources.Oil;
            oldStateRubberInput.Value = (decimal)currentState.Resources.Rubber;
            oldStateSteelInput.Value = (decimal)currentState.Resources.Steel;
            oldStateTungstenInput.Value = (decimal)currentState.Resources.Tungsten;
            oldStateAluminiumInput.Value = (decimal)currentState.Resources.Aluminium;
            oldStateChromiumInput.Value = (decimal)currentState.Resources.Chromium;

            oldStateOwnerInput.SelectedItem = Country.Get(currentState.Owner);
            newStateOwnerInput.SelectedItem = Country.Get(currentState.Owner);

            oldStateCoreInput.SelectedItems.Clear();
            newStateCoreInput.SelectedItems.Clear();
            foreach (string s in currentState.Cores) {
                Country c = Country.Get(s);
                oldStateCoreInput.SelectedItems.Add(c);
                newStateCoreInput.SelectedItems.Add(c);
            }

            oldStateInfrastructureInput.Value = currentState.Infrastructure;
            oldStateCivFactoriesInput.Value = currentState.CivillianFactories;
            oldStateMilFactoriesInput.Value = currentState.MilitaryFactories;
            oldStateDockyardsInput.Value = currentState.Dockyards;
            oldStateRefineriesInput.Value = currentState.Refineries;
            oldStateSilosInput.Value = currentState.Silos;
            oldStateReactorsInput.Value = currentState.Reactors;
            oldStateAirbaseInput.Value = currentState.Airbases;
            oldStateRadarInput.Value = currentState.Radar;
            oldStateRocketsInput.Value = currentState.Rockets;
            oldStateAntiairInput.Value = currentState.Antiair;

        }

        private void oldStateManpowerInput_ValueChanged(object sender, EventArgs e) {
            newStateManpowerInput.Value = currentState.Manpower - oldStateManpowerInput.Value;
        }

        private void newStateManpowerInput_ValueChanged(object sender, EventArgs e) {
            oldStateManpowerInput.Value = currentState.Manpower - newStateManpowerInput.Value;

        }

        private void stateSaveButton_Click(object sender, EventArgs e) {

            //TODO error checking

            int newId = State.Count + 1;
            if (!State.Create(newId)) return;
            State newState = State.Get(newId);

            //new state

            newState.Name = $"STATE_{newId}";
            Localisation.Update(newState.LocaleKey, newStateNameInput.Text, LocaleType.StateName);
            newState.FileName = $"{newId}-{newState.LocalisedName}.txt";
            newState.Manpower = (int)newStateManpowerInput.Value;
            newState.Category = (StateCategory)newStateCategoryInput.SelectedItem;
            newState.Resources.Oil = (double)newStateOilInput.Value;
            newState.Resources.Rubber = (double)newStateRubberInput.Value;
            newState.Resources.Steel = (double)newStateSteelInput.Value;
            newState.Resources.Tungsten = (double)newStateTungstenInput.Value;
            newState.Resources.Aluminium = (double)newStateAluminiumInput.Value;
            newState.Resources.Chromium = (double)newStateChromiumInput.Value;

            newState.Owner = newStateOwnerInput.SelectedItem.ToString();
            foreach (object o in newStateCoreInput.SelectedItems) {
                newState.Cores.Add(o.ToString());
            }

            newState.Infrastructure = (int)newStateInfrastructureInput.Value;
            newState.CivillianFactories = (int)newStateCivFactoriesInput.Value;
            newState.MilitaryFactories = (int)newStateMilFactoriesInput.Value;
            newState.Dockyards = (int)newStateDockyardsInput.Value;
            newState.Refineries = (int)newStateRefineriesInput.Value;
            newState.Silos = (int)newStateSilosInput.Value;
            newState.Reactors = (int)newStateReactorsInput.Value;
            newState.Airbases = (int)newStateAirbaseInput.Value;
            newState.Radar = (int)newStateRadarInput.Value;
            newState.Rockets = (int)newStateRocketsInput.Value;
            newState.Antiair = (int)newStateAntiairInput.Value;


            //old state
            Localisation.Update(currentState.LocaleKey, oldStateNameInput.Text, LocaleType.StateName);
            currentState.Manpower = (int)oldStateManpowerInput.Value;
            currentState.Category = (StateCategory)oldStateCategoryInput.SelectedItem;
            currentState.Resources.Oil = (double)oldStateOilInput.Value;
            currentState.Resources.Rubber = (double)oldStateRubberInput.Value;
            currentState.Resources.Steel = (double)oldStateSteelInput.Value;
            currentState.Resources.Tungsten = (double)oldStateTungstenInput.Value;
            currentState.Resources.Aluminium = (double)oldStateAluminiumInput.Value;
            currentState.Resources.Chromium = (double)oldStateChromiumInput.Value;

            currentState.Owner = oldStateOwnerInput.SelectedItem.ToString();
            foreach (object o in oldStateCoreInput.SelectedItems) {
                currentState.Cores.Add(o.ToString());
            }

            currentState.Infrastructure = (int)oldStateInfrastructureInput.Value;
            currentState.CivillianFactories = (int)oldStateCivFactoriesInput.Value;
            currentState.MilitaryFactories = (int)oldStateMilFactoriesInput.Value;
            currentState.Dockyards = (int)oldStateDockyardsInput.Value;
            currentState.Refineries = (int)oldStateRefineriesInput.Value;
            currentState.Silos = (int)oldStateSilosInput.Value;
            currentState.Reactors = (int)oldStateReactorsInput.Value;
            currentState.Airbases = (int)oldStateAirbaseInput.Value;
            currentState.Radar = (int)oldStateRadarInput.Value;
            currentState.Rockets = (int)oldStateRocketsInput.Value;
            currentState.Antiair = (int)oldStateAntiairInput.Value;

            //transfer provinces
            foreach (object o in provinceSelectList.CheckedItems) {
                Province p = (Province)o;
                State.TransferProvince(currentState, newState, p);
            }


            SetDataSources();

            validLabel.Text = $"New Valid: {newState.IsValid()}";

        }
    }
}
