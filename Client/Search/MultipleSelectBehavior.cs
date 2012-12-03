﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Subsonic8.MenuItem;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Subsonic8.Search
{
    public class MultipleSelectBehavior
    {
        #region SelectedItems Attached Property

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(object),
            typeof(MultipleSelectBehavior),
            new PropertyMetadata(new ObservableCollection<MenuItemViewModel>(), AttchedPropertyChanged));

        public static void SetSelectedItems(DependencyObject obj, ObservableCollection<MenuItemViewModel> selectedItems)
        {
            obj.SetValue(SelectedItemsProperty, selectedItems);
        }

        public static ObservableCollection<MenuItemViewModel> GetSelectedItems(DependencyObject obj)
        {
            return (ObservableCollection<MenuItemViewModel>)obj.GetValue(SelectedItemsProperty);
        }

        private static void AttchedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var observableCollection = eventArgs.NewValue as INotifyCollectionChanged;
            var gridView = (GridView)dependencyObject;

            var sourceChangedHandler = SetupSourceCollectionChangedEventHandler(dependencyObject, observableCollection);
            gridView.SelectionChanged += OnSelectionChanged;
            SetupOnUnloadedHandler(dependencyObject, observableCollection, sourceChangedHandler);
        }

        #endregion

        private static void SetupOnUnloadedHandler(DependencyObject dependencyObject, INotifyCollectionChanged observableCollection,
                                                   NotifyCollectionChangedEventHandler sourceChangedHandler)
        {
            RoutedEventHandler unloadedEventHandler = null;
            unloadedEventHandler = (sender, args) =>
                                       {
                                           observableCollection.CollectionChanged -= sourceChangedHandler;
                                           ((GridView)sender).SelectionChanged -= OnSelectionChanged;
                                           ((GridView)sender).Unloaded -= unloadedEventHandler;
                                       };
            ((GridView)dependencyObject).Unloaded += unloadedEventHandler;
        }

        private static NotifyCollectionChangedEventHandler SetupSourceCollectionChangedEventHandler(DependencyObject dependencyObject,
                                                                                          INotifyCollectionChanged observableCollection)
        {
            NotifyCollectionChangedEventHandler sourceChangedHandler = (s, ev) => 
                SelectedItemsChangedFromSource(dependencyObject, s as ObservableCollection<MenuItemViewModel>);
            observableCollection.CollectionChanged += sourceChangedHandler;

            return sourceChangedHandler;
        }

        private static void SelectedItemsChangedFromSource(DependencyObject dependencyObject, IEnumerable<MenuItemViewModel> source)
        {
            var selectedItems = ((GridView)dependencyObject).SelectedItems;
            if (source != null)
            {
                var toRemove = selectedItems.Where(i => !source.Contains(i)).ToList();
                foreach (var item in toRemove)
                {
                    selectedItems.Remove(item);
                }

                foreach (var item in source.Where(i => !selectedItems.Contains(i)))
                {
                    selectedItems.Add(item);
                }
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = GetSelectedItems((DependencyObject)sender);
            foreach (var item in e.RemovedItems.Where(selectedItems.Contains))
            {
                selectedItems.Remove((MenuItemViewModel)item);
            }

            foreach (var item in e.AddedItems.Where(item => !selectedItems.Contains(item)))
            {
                selectedItems.Add((MenuItemViewModel)item);
            }
        }
    }
}