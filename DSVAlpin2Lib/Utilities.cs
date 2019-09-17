﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSVAlpin2Lib
{

  public class ItemsChangedNotifier : INotifyCollectionChanged
  {
    INotifyCollectionChanged _source;

    public bool CollectionResetIfItemChanged { get; set; } = false;

    public ItemsChangedNotifier(INotifyCollectionChanged source)
    {
      _source = source;

      _source.CollectionChanged += OnCollectionChanged;

      // Populate initially
      Initialize();
    }

    ~ItemsChangedNotifier()
    {
      _source.CollectionChanged -= OnCollectionChanged;
    }


    public INotifyCollectionChanged Source { get { return _source; } }



    public delegate void ItemChangedEventHandler(object sender, PropertyChangedEventArgs e);
    /// <summary>
    /// If an item of the collection changed its properties this event triggers
    /// </summary>
    public event ItemChangedEventHandler ItemChanged;



    public event NotifyCollectionChangedEventHandler CollectionChanged;



    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
        foreach (INotifyPropertyChanged item in e.OldItems)
          item.PropertyChanged -= ItemPropertyChanged;

      if (e.NewItems != null)
        foreach (INotifyPropertyChanged item in e.NewItems)
          item.PropertyChanged += ItemPropertyChanged;

      NotifyCollectionChangedEventHandler handler = CollectionChanged;
      handler?.Invoke(sender, e);
    }

    private void Initialize()
    {
      System.Collections.IEnumerable items = _source as System.Collections.IEnumerable;
      if (items!=null)
        foreach (var item in items)
          ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
    }


    private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      ItemChangedEventHandler handler = ItemChanged;
      handler?.Invoke(sender, e);

      if (CollectionResetIfItemChanged)
      {
        NotifyCollectionChangedEventHandler handlerCC = CollectionChanged;
        handlerCC?.Invoke(sender, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
      }
    }

  }


  public class ItemsChangeObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
  {
    /// <summary>
    /// Specifies whether the collection itself shall be reset and trigger a CollectionChanged event in case an item's property changed
    /// </summary>
    public bool CollectionResetIfItemChanged { get; set; } = false;


    public delegate void ItemChangedEventHandler(object sender, PropertyChangedEventArgs e);
    /// <summary>
    /// If an item of the collection changed its properties this event triggers
    /// </summary>
    public event ItemChangedEventHandler ItemChanged;

    protected override void ClearItems()
    {
      foreach (var item in Items) item.PropertyChanged -= ItemPropertyChanged;
      base.ClearItems();
    }

    protected override void InsertItem(int index, T item)
    {
      item.PropertyChanged += ItemPropertyChanged;
      base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
      this[index].PropertyChanged -= ItemPropertyChanged;
      base.RemoveItem(index);
    }

    protected override void SetItem(int index, T item)
    {
      this[index].PropertyChanged -= ItemPropertyChanged;
      item.PropertyChanged += ItemPropertyChanged;
      base.SetItem(index, item);
    }

    void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      ItemChangedEventHandler handler = ItemChanged;
      handler?.Invoke(sender, e);

      if (CollectionResetIfItemChanged)
        OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }
  }



  public class CopyObservableCollection<T> : ObservableCollection<T>
  {
    ObservableCollection<T> _source;
    Cloner<T> _cloner;

    public delegate TC Cloner<TC>(TC source);
    public CopyObservableCollection(ObservableCollection<T> source, Cloner<T> cloner)
    {
      _source = source;
      _cloner = cloner;

      _source.CollectionChanged += OnCollectionChanged;

      FillInitially();
    }

    ~CopyObservableCollection()
    {
      _source.CollectionChanged -= OnCollectionChanged;
    }


    private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      // Clone
      throw new NotImplementedException();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // Unhook from PropertyChanged
      if (e.OldItems != null)
        foreach (T item in e.OldItems)
        {
          if (item is INotifyPropertyChanged item2)
            item2.PropertyChanged -= ItemPropertyChanged;
        }

      // Sync Lists
      int i,j;
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          i = 0;
          foreach (T item in e.NewItems)
          {
            Insert(e.NewStartingIndex+i, _cloner(item));
            i++;
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          for (i = 0; i < e.OldItems.Count; i++)
            RemoveAt(e.OldStartingIndex + i);
          break;
        case NotifyCollectionChangedAction.Replace:
          throw new NotImplementedException();
        case NotifyCollectionChangedAction.Reset:
          Clear();
          break;
        case NotifyCollectionChangedAction.Move:
          for (i = e.OldStartingIndex, j = e.NewStartingIndex; i < e.OldItems.Count; i++, j++)
            Move(i, j);
          break;
      }

      // Unhook to PropertyChanged
      if (e.NewItems != null)
        foreach (T item in e.NewItems)
        {
          if (item is INotifyPropertyChanged item2)
            item2.PropertyChanged += ItemPropertyChanged;
        }
    }

    private void FillInitially()
    {
      for(int i=0; i<_source.Count(); i++)
        Add(_cloner(_source[i]));
    }

  }

  public static class ObservableCollectionExtensions
  {
    /// <summary>
    /// Inserts a new element in a sorted collection
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    /// <typeparam name="TI"></typeparam>
    /// <param name="col"></param>
    /// <param name="item"></param>
    /// <param name="comparer"></param>
    public static void InsertSorted<TC>(this Collection<TC> collection, TC item, System.Collections.Generic.IComparer<TC> comparer)
    {
      // Find right position and insert
      int i = 0;
      for (; i < collection.Count(); ++i)
        if (comparer.Compare(item, collection.ElementAt(i)) < 0)
          break;

      // Not yet inserted, insert at the end
      collection.Insert(i, item);
    }

    public static void InsertSorted<TC>(this Collection<TC> collection, TC[] items, IComparer<TC> comparer)
    {
      foreach (TC item in items)
        collection.InsertSorted(item, comparer);

    }

    public static void Sort<TC>(this Collection<TC> collection, IComparer<TC> comparer)
    {
      for (int n = collection.Count(); n > 1; --n)
      {
        for (int i = 0; i < n - 1; ++i)
        {
          if (comparer.Compare(collection.ElementAt(i), collection.ElementAt(i + 1)) > 0)
          {
            TC temp = collection.ElementAt(i);
            collection.RemoveAt(i);
            collection.Insert(i+1, temp);
          }
        }
      }
    }


  }


  // Define other methods and classes here
  public static class Extensions
  {
    public static DateTime AddMicroseconds(this DateTime datetime, Int32 value)
    {
      return new DateTime(datetime.Ticks + value*10, datetime.Kind);
    }


    public static TimeSpan AddMicroseconds(this TimeSpan timespan, Int32 value)
    {
      return new TimeSpan(timespan.Ticks + value*10);
    }
  }
}
