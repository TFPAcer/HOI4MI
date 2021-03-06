﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HOI4MI;
using HOI4MI.Entity;
using HOI4MI.Manager;
using HOI4MI.Util;

namespace HOI4MI.Forms {
    public partial class StateEditorForm : Form {

        private readonly ResourceManager resourceManager;

        private State currentState;
        private Province currentProvince;

        private List<List<State>> stateGroups;

        public StateEditorForm(ResourceManager rm) {
            InitializeComponent();

            resourceManager = rm;

            SetDataSources();
            SetImages();
        }

        private void Reload() {
            Localisation.Reload();
            Province.ReloadAll();
            State.ReloadAll();
            Country.ReloadAll();
            resourceManager.ReloadResourceMap(true, false);
        }

        private void SetImages() {
            oilImage.BackgroundImage = Properties.Resources.oil;
            rubberImage.BackgroundImage = Properties.Resources.rubber;
            steelImage.BackgroundImage = Properties.Resources.steel;
            aluminiumImage.BackgroundImage = Properties.Resources.aluminium;
            tungstenImage.BackgroundImage = Properties.Resources.tungsten;
            chromiumImage.BackgroundImage = Properties.Resources.chromium;

            infrastructureImage.BackgroundImage = Properties.Resources.infrastructure;
            civillianImage.BackgroundImage = Properties.Resources.civillian;
            militaryImage.BackgroundImage = Properties.Resources.military;
            dockyardImage.BackgroundImage = Properties.Resources.dockyard;
            refineryImage.BackgroundImage = Properties.Resources.refinery;
            silosImage.BackgroundImage = Properties.Resources.silo;
            airbaseImage.BackgroundImage = Properties.Resources.airbase;
            antiairImage.BackgroundImage = Properties.Resources.antiair;
            reactorImage.BackgroundImage = Properties.Resources.reactor;
            radarImage.BackgroundImage = Properties.Resources.radar;
            rocketImage.BackgroundImage = Properties.Resources.rocket;

            vpImage.BackgroundImage = Properties.Resources.vp;
            fortImage.BackgroundImage = Properties.Resources.fort;
            coastalFortImage.BackgroundImage = Properties.Resources.coastalfort;
            navalBaseImage.BackgroundImage = Properties.Resources.navalbase;
        }

        private void SetDataSources() {
            stateGroups = new List<List<State>>();
            selectList.Items.Clear();
            stateCategoryInput.DataSource = Enum.GetValues(typeof(StateCategory));
            stateOwnerInput.DataSource = Country.Countries;
            stateCoreInput.DataSource = Country.Countries;
            stateImpassableInput.DataSource = Utils.BoolValues();
        }

        private void groupByInput_SelectedIndexChanged(object sender, EventArgs e) {
            stateGroups = new List<List<State>>();
            switch (groupByInput.SelectedIndex) {
                case 0: { //ID
                    int numGrps = (State.StatesUnordered.Count / 100)+1;
                    for (int i = 0; i < numGrps; i++) {
                        stateGroups.Add(State.Find(s => s.id >= i*100 && s.id < (i+1)*100).ToList());
                    }
                    selectList.DataSource = Enumerable.Range(0, numGrps).ToList();
                    break;
                }
                case 1: { //Owner
                    var statesByOwner = State.StatesUnordered.GroupBy(s => Country.Get(s.Owner));
                    List<Country> countries = new List<Country>();
                    foreach (IGrouping<Country, State> a in statesByOwner) {
                        stateGroups.Add(a.ToList());
                        countries.Add(a.Key);
                    }
                    selectList.DataSource = countries;
                    break;
                }
                case 2: { //Cores
                    var statesByCore = new Dictionary<Country, List<State>>();


                    foreach (State s in State.StatesUnordered) {
                        if (s.Cores.Count == 0) continue;
                        foreach (string t in s.Cores) {
                            Country c = Country.Get(t);
                            if (c == null) continue;

                            if (!statesByCore.ContainsKey(c)) {
                                statesByCore.Add(c, new List<State> { s });
                            }
                            else {
                                statesByCore[c].Add(s);
                            }
                        }
                    }

                    foreach (var v in statesByCore) {
                        stateGroups.Add(v.Value);
                    }
                    selectList.DataSource = statesByCore.Keys.ToList();
                    break;
                }
                case 3: { //Category
                    var statesByCategory = State.StatesUnordered.GroupBy(s => s.Category).ToList();
                    statesByCategory.OrderBy(x => x.Key);
                    List<StateCategory> categories = new List<StateCategory>();
                    foreach (IGrouping<StateCategory, State> a in statesByCategory) {
                        stateGroups.Add(a.ToList());
                        categories.Add(a.Key);
                    }
                    selectList.DataSource = categories;
                    break;
                }
                case 4: { //Infrastructure
                    var statesByInfrastructure = State.StatesUnordered.GroupBy(s => s.Infrastructure).ToList();
                    statesByInfrastructure.OrderBy(x => x.Key);
                    List<int> inf = new List<int>();
                    foreach (IGrouping<int, State> a in statesByInfrastructure) {
                        stateGroups.Add(a.ToList());
                        inf.Add(a.Key);
                    }
                    selectList.DataSource = inf;
                    break;
                }
                case 5: { //Population
                    int[] popGrps = { 0, 100000, 500000, 1000000, 2000000, 5000000, 10000000, 30000000, 50000000};
                    int numGrps = popGrps.Length;
                    for (int i = 0; i < numGrps; i++) {
                        int min = popGrps[i];
                        int max = i+1 < numGrps ? popGrps[i + 1] : int.MaxValue;
                        stateGroups.Add(State.Find(s => s.Manpower >= min && s.Manpower < max).ToList());
                    }
                    selectList.DataSource = popGrps;
                    break;
                }
            }
        }

        private void selectList_SelectedIndexChanged(object sender, EventArgs e) {
            //int startId = (int)selectList.SelectedItem;
            stateList.DataSource = stateGroups[selectList.SelectedIndex];
        }

        private void stateList_SelectedIndexChanged(object sender, EventArgs e) {
            currentState = (State)stateList.SelectedItem;
            testBox.Text = ((State)stateList.SelectedItem).ToStringVerbose();

            stateNameInput.Text = $"{currentState.LocalisedName}";
            stateManpowerInput.Value = currentState.Manpower;
            stateCategoryInput.SelectedItem = currentState.Category;
            stateImpassableInput.SelectedItem = currentState.Impassable;
            stateOwnerInput.SelectedItem = Country.Get(currentState.Owner);
            stateCoreInput.SelectedItems.Clear();
            foreach (string s in currentState.Cores) {
                Country c = Country.Get(s);
                stateCoreInput.SelectedItems.Add(c);
            }

            stateOilInput.Value = (decimal)currentState.Resources.Oil;
            stateRubberInput.Value = (decimal)currentState.Resources.Rubber;
            stateSteelInput.Value = (decimal)currentState.Resources.Steel;
            stateTungstenInput.Value = (decimal)currentState.Resources.Tungsten;
            stateAluminiumInput.Value = (decimal)currentState.Resources.Aluminium;
            stateChromiumInput.Value = (decimal)currentState.Resources.Chromium;

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

            provinceList.Items.Clear();
            foreach (Province p in currentState.Provinces) {
                provinceList.Items.Add(p);
            }
            provinceList.SelectedIndex = 0;
        }

        private void provinceList_SelectedIndexChanged(object sender, EventArgs e) {
            currentProvince = (Province)provinceList.SelectedItem;

            provinceNameInput.Text = currentProvince.LocalisedName;
            victoryPointsInput.Value = currentProvince.VictoryPoints;
            fortInput.Value = currentProvince.LandForts;
            coastalFortInput.Value = currentProvince.CoastalForts;
            navalBaseInput.Value = currentProvince.NavalBase;
        }

        private void stateSaveButton_Click(object sender, EventArgs e) {
            //old state
            Localisation.Update(currentState.LocaleKey, stateNameInput.Text, LocaleType.StateName);
            currentState.Manpower = (int)stateManpowerInput.Value;
            currentState.Category = (StateCategory)stateCategoryInput.SelectedItem;
            currentState.Impassable = (bool)stateImpassableInput.SelectedItem;
            currentState.Owner = stateOwnerInput.SelectedItem.ToString();
            foreach (object o in stateCoreInput.SelectedItems) {
                currentState.Cores.Add(o.ToString());
            }

            currentState.Resources.Oil = (double)stateOilInput.Value;
            currentState.Resources.Rubber = (double)stateRubberInput.Value;
            currentState.Resources.Steel = (double)stateSteelInput.Value;
            currentState.Resources.Tungsten = (double)stateTungstenInput.Value;
            currentState.Resources.Aluminium = (double)stateAluminiumInput.Value;
            currentState.Resources.Chromium = (double)stateChromiumInput.Value;

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

            if (currentProvince.LocaleKey != provinceNameInput.Text)
                Localisation.Update(currentProvince.LocaleKey, provinceNameInput.Text, LocaleType.VictoryPointName);
            currentProvince.VictoryPoints = (int)victoryPointsInput.Value;
            currentProvince.LandForts = (int)fortInput.Value;
            currentProvince.CoastalForts = (int)coastalFortInput.Value;
            currentProvince.NavalBase = (int)navalBaseInput.Value;
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

        private void StateEditorForm_Load(object sender, EventArgs e) {

        }

        private void testBox_TextChanged(object sender, EventArgs e) {

        }
    }
}
