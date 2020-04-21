﻿/*
 *  Copyright (C) 2019 - 2020 by Sven Flossmann
 *  
 *  This file is part of Race Horology.
 *
 *  Race Horology is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  any later version.
 * 
 *  Race Horology is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with Race Horology.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  Diese Datei ist Teil von Race Horology.
 *
 *  Race Horology ist Freie Software: Sie können es unter den Bedingungen
 *  der GNU Affero General Public License, wie von der Free Software Foundation,
 *  Version 3 der Lizenz oder (nach Ihrer Wahl) jeder neueren
 *  veröffentlichten Version, weiter verteilen und/oder modifizieren.
 *
 *  Race Horology wird in der Hoffnung, dass es nützlich sein wird, aber
 *  OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite
 *  Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
 *  Siehe die GNU Affero General Public License für weitere Details.
 *
 *  Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem
 *  Programm erhalten haben. Wenn nicht, siehe <https://www.gnu.org/licenses/>.
 * 
 */

using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceHorologyLib
{

  public class ImportReader
  {
    DataSet _dataSet;

    static private string[] txtExtensions = { ".csv", ".tsv", ".txt" };

    public ImportReader(string path)
    {
      IExcelDataReader reader;

      if (txtExtensions.Contains(System.IO.Path.GetExtension(path).ToLower()))
      {
        // CSV File
        var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        reader = ExcelReaderFactory.CreateCsvReader(stream);
        _dataSet = reader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });
      }
      else
      {
        // Excel
        var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        reader = ExcelReaderFactory.CreateReader(stream);
        _dataSet = reader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });
      }

      Columns = extractFields(_dataSet);
    }

    public DataSet Data { get { return _dataSet; } }

    public List<string> Columns { get; set; }

    protected static List<string> extractFields(DataSet ds)
    {
      List<string> ret = new List<string>();
      foreach (DataColumn col in ds.Tables[0].Columns)
      {
        ret.Add(col.ColumnName);
      }
      return ret;
    }
  }


  public class Mapping
  {
    public class MappingEntry
    {
      public string Key { get; set; }
      public string Value { get; set; }
    }


    ObservableCollection<MappingEntry> _mapping;
    List<string> _availableFields;
    List<string> _requiredFields;

    public Mapping(IEnumerable<string> requiredFields, IEnumerable<string> availableFields)
    {
      _availableFields = new List<string>();
      _availableFields.Add("---");
      _availableFields.AddRange(availableFields);
      
      _requiredFields = requiredFields.ToList();

      _mapping = new ObservableCollection<MappingEntry>();

      initMapping();
    }


    public void Assign(string requiredField, string providedField)
    {
      var mapEntry = _mapping.FirstOrDefault(m => m.Key == requiredField);
      if (mapEntry != null)
      {
        mapEntry.Value = providedField;
      }
      else
        _mapping.Add(new MappingEntry { Key = requiredField, Value = providedField });
    }


    public string MappedField(string requiredField)
    {
      var mapEntry = _mapping.First(m => m.Key == requiredField);
      return mapEntry.Value;
    }

    /// <summary>
    /// The mapping from required field => configured field
    /// </summary>
    public ObservableCollection<MappingEntry> MappingList { get { return _mapping; } private set { _mapping = value; } }

    /// <summary>
    /// Returns the available fields (used by the UI to populate the ComboBox)
    /// </summary>
    public List<string> AvailableFields { get { return _availableFields; } }

    /// <summary>
    /// Initially populates the mapping
    /// </summary>
    void initMapping()
    {
      foreach (var v in _requiredFields)
      {
        Assign(v, guessMappedField(v));
      }
    }

    /// <summary>
    /// Tries to guess which field form the _availableFields maps best to the required field
    /// </summary>
    virtual protected string guessMappedField(string reqField, double threshold = 0.7)
    {
      double maxV = 0;
      int selI = 0;
      for (int i = 0; i < _availableFields.Count; i++)
      {
        foreach (var s in synonyms(reqField))
        {
          double val = StringComparison.ComparisonMetrics.Similarity(s, _availableFields[i],
            StringComparison.Enums.StringComparisonOption.UseHammingDistance | StringComparison.Enums.StringComparisonOption.UseRatcliffObershelpSimilarity
            );
          if (val > maxV)
          {
            selI = i;
            maxV = val;
          }
        }
      }

      if (maxV > threshold)
        return _availableFields[selI];

      return null;
    }

    /// <summary>
    /// Returns potential synonyms for the given field
    /// </summary>
    virtual protected List<string> synonyms(string field)
    {
      return new List<string> { field };
    }
  }

  /// <summary>
  /// Pre-configured mapping for participant mapping (participant import)
  /// </summary>
  public class ParticipantMapping : Mapping
  {
    /// <summary>
    /// Map defining the required fields and potential available fields
    /// </summary>
    static Dictionary<string, List<string>> _requiredField = new Dictionary<string, List<string>>
    {
      { "Name", new List<string>{ "Name" } },
      { "Firstname", new List<string>{"Vorname"} },
      { "Sex", new List<string>{"Geschlecht", "Kategorie"} },
      { "Year", new List<string>{"Geburtsjahr", "Jahr", "Jahrgang", "JG" } },
      { "Club", new List<string>{"Club", "Verein"} },
      { "Nation", new List<string>{"Nation", "Verband"} },
      { "Code", new List<string>{"DSV-Id", "Code"} },
      { "SvId", new List<string>{"SvId", "SkiverbandId"} }
    };

    public ParticipantMapping(List<string> availableFields) : base(_requiredField.Keys, availableFields)
    { 
    }

    protected override List<string> synonyms(string field)
    {
      return _requiredField[field];
    }

  }


  /// <summary>
  /// Pre-configured mapping for race mapping (race import)
  /// </summary>
  public class RaceMapping : Mapping
  {
    /// <summary>
    /// Map defining the required fields and potential available fields
    /// </summary>
    static Dictionary<string, List<string>> _requiredField = new Dictionary<string, List<string>>
    {
      { "Name", new List<string>{ "Name" } },
      { "Firstname", new List<string>{"Vorname"} },
      { "Sex", new List<string>{"Geschlecht", "Kategorie"} },
      { "Year", new List<string>{"Geburtsjahr", "Jahr", "Jahrgang", "JG" } },
      { "Club", new List<string>{"Club", "Verein"} },
      { "Nation", new List<string>{"Nation", "Verband"} },
      { "Code", new List<string>{"DSV-Id", "Code"} },
      { "SvId", new List<string>{"SvId", "SkiverbandId"} },
      { "Points", new List<string>{"Points", "Punkte"} },
      { "StartNumber", new List<string>{"start number", "Startnummer", "SN"} },
    };

    public RaceMapping(List<string> availableFields) : base(_requiredField.Keys, availableFields)
    {
    }

    protected override List<string> synonyms(string field)
    {
      return _requiredField[field];
    }

  }




  public class Import
  {
    DataSet _importDataSet;
    IList<Participant> _particpants;
    Mapping _mapping;

    public Import(DataSet ds, IList<Participant> particpants, Mapping mapping)
    {
      _importDataSet = ds;
      _particpants = particpants;
      _mapping = mapping;
    }


    public void DoImport()
    {
      var rows = _importDataSet.Tables[0].Rows;


      foreach(DataRow row in rows)
      {
        try
        {
          importRow(row);
        }
        catch(Exception)
        { }
      }
    }


    public Participant importRow(DataRow row) 
    {
      Participant partImported = null;

      Participant partCreated = createParticipant(row);

      Participant partExisting = findExistingParticpant(partCreated);

      if (partExisting != null)
        partImported = updateParticipant(partExisting, partCreated);
      else
        partImported = insertParticpant(partCreated);

      return partImported;
    }


    Participant createParticipant(DataRow row)
    {
      Participant p = new Participant
      {
        Name = getNameComaSeparated(getValue<string>(row, "Name")),
        Firstname = getFirstNameComaSeparated(getValue<string>(row, "Firstname")),
        Sex = getValue<string>(row, "Sex"),
        Club = getValue<string>(row, "Club"),
        Nation = getValue<string>(row, "Nation"),
        SvId = getValue<string>(row, "SvId"),
        Code = getValue<string>(row, "Code"),
        Year = uint.Parse(getValue<string>(row, "Year"))
      };

      return p;
    }

    T getValue<T>(DataRow row, string field)
    {
      var columnName = _mapping.MappedField(field);

      if (columnName != null)
        if (row.Table.Columns.Contains(columnName))
          return row.Field<T>(_mapping.MappedField(field));

      return default;
    }

    string getNameComaSeparated(string name)
    {
      string res;
      var nameParts = name.Split(',');
      if (nameParts.Length > 1)
        res = nameParts[0];
      else
        res = name;

      return res.Trim();
    }

    string getFirstNameComaSeparated(string name)
    {
      string res;
      var nameParts = name.Split(',');
      if (nameParts.Length > 1)
        res = nameParts[nameParts.Length - 1];
      else
        res = name;

      return res.Trim();
    }

    bool sameParticpant(Participant p1, Participant p2)
    {
      if (!string.IsNullOrEmpty(p1.SvId) && !string.IsNullOrEmpty(p2.SvId))
        return p1.SvId == p2.SvId;

      if (!string.IsNullOrEmpty(p1.Code) && !string.IsNullOrEmpty(p2.Code))
        return p1.Code == p2.Code;

      return p1.Fullname == p2.Fullname;

    }

    Participant findExistingParticpant(Participant partImp)
    {
      var pFound = _particpants.FirstOrDefault(p => sameParticpant(p, partImp));
      return pFound;
    }

    Participant updateParticipant(Participant partExisting, Participant partImp)
    {
      partExisting.Assign(partImp);
      return partExisting;
    }

    Participant insertParticpant(Participant partImp)
    {
      _particpants.Add(partImp);
      
      return partImp;
    }
  }


  public class RaceImport
  {
    DataSet _importDataSet;
    Race _race;
    Mapping _mapping;

    public RaceImport(DataSet ds, Race race, Mapping mapping)
    {
      _importDataSet = ds;
      _race = race;
      _mapping = mapping;
    }


    public void DoImport()
    {
      // 1. Normaler Import
      Import particpantImport = new Import(_importDataSet, _race.GetDataModel().GetParticipants(), _mapping);

      // 2. Punkteabgleich für ein Rennen (eg DSV Liste) 
      var rows = _importDataSet.Tables[0].Rows;
      foreach (DataRow row in rows)
      {
        try
        {
          Participant importedParticipant = particpantImport.importRow(row);

          double points = getPoints(row);
          uint sn = getStartNumber(row);

          _race.AddParticipant(importedParticipant);
        }
        catch (Exception)
        { }
      }
    }


    double getPoints(DataRow row) 
    {
      return -1;
    }


    uint getStartNumber(DataRow row)
    {
      return 0;
    }


  }

}
