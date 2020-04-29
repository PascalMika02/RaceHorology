﻿using RaceHorologyLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RaceHorology
{
  /// <summary>
  /// Interaction logic for CompetitionUC.xaml
  /// </summary>
  public partial class CompetitionUC : UserControl
  {
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    AppDataModel _dm;
    LiveTimingMeasurement _liveTimingMeasurement;
    TextBox _txtLiveTimingStatus;

    public CompetitionUC(AppDataModel dm, LiveTimingMeasurement liveTimingMeasurement, TextBox txtLiveTimingStatus)
    {
      _dm = dm;
      _liveTimingMeasurement = liveTimingMeasurement;
      _txtLiveTimingStatus = txtLiveTimingStatus;

      InitializeComponent();

      _liveTimingMeasurement.LiveTimingMeasurementStatusChanged += OnLiveTimingMeasurementStatusChanged;

      ConnectGUIToDataModel();
    }



    /// <summary>
    /// Connects the GUI (e.g. Data Grids, ...) to the data model
    /// </summary>
    private void ConnectGUIToDataModel()
    {
      // Connect with GUI DataGrids
      ObservableCollection<Participant> participants = _dm.GetParticipants();
      dgParticipants.ItemsSource = participants;

      foreach (var r in _dm.GetRaces())
      {
        TabItem tabRace = new TabItem { Header = r.RaceType.ToString(), Name = r.RaceType.ToString() };
        tabControlTopLevel.Items.Insert(1, tabRace);

        tabRace.FontSize = 16;

        RaceUC raceUC = new RaceUC(_dm, r, _liveTimingMeasurement, _txtLiveTimingStatus);
        tabRace.Content = raceUC;
      }
    }


    private void btnImport_Click(object sender, RoutedEventArgs e)
    {
      if (_dm?.GetParticipants() == null)
      {
        Logger.Error("Import not possible: datamodel not available");
        return;
      }
      ImportWizard importWizard = new ImportWizard(_dm);
      importWizard.Owner = Window.GetWindow(this);
      importWizard.ShowDialog();
    }


    #region Tab Management

    private void OnLiveTimingMeasurementStatusChanged(object sender, bool isRunning)
    {
      EnsureOnlyCurrentRaceCanBeSelected(isRunning);
    }

    private void EnsureOnlyCurrentRaceCanBeSelected(bool onlyCurrentRace)
    {
      foreach (TabItem tab in tabControlTopLevel.Items)
      {
        RaceUC raceUC = tab.Content as RaceUC;
        if (raceUC != null)
        {
          bool isEnabled = !onlyCurrentRace || (_dm.GetCurrentRace() == raceUC.GetRace());
          tab.IsEnabled = isEnabled;
        }
      }
    }

    private void TabControlTopLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selected = tabControlTopLevel.SelectedContent as RaceUC;
      if (selected != null)
      {
        _dm.SetCurrentRace(selected.GetRace());
        _dm.SetCurrentRaceRun(selected.GetRaceRun());
      }
    }

    #endregion

  }
}
