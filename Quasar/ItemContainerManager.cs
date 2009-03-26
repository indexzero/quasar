/**************************************************************
 * Copyright (c) 2008 Josh Wagoner, Charlie Robbins
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
**************************************************************/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace Quasar
{
    public class ItemContainerManager
    {
		#region Fields 

        private Dictionary<DependencyObject, object> _containerToItem;
        private ItemCollection _itemCollection;
        private Dictionary<object, DependencyObject> _itemToContainer;

		#endregion Fields 


        public ItemContainerManager(ItemsControl itemsControl)
        {
            _itemCollection = itemsControl.Items;

            _containerToItem = new Dictionary<DependencyObject, object>();
            _itemToContainer = new Dictionary<object, DependencyObject>();
        }

		#region Methods 


        public void AddContainer(DependencyObject container, object item)
        {
            // Store the container to item and item to container
            // Relationships for quick/easy lookup
            _containerToItem.Add(container, item);
            _itemToContainer.Add(item, container);
        }

        public DependencyObject ContainerFromIndex(int index)
        {
            // Get the item from the index
            if (index >= 0 && index < _itemCollection.Count)
            {
                object item = _itemCollection[index];

                return ContainerFromItem(item);
            }

            return null;
        }

        public DependencyObject ContainerFromItem(object item)
        {
            if (item == null)
                return null;

            if (_itemToContainer.ContainsKey(item))
            {
                return _itemToContainer[item];
            }
            return null;
        }

        public ReadOnlyCollection<DependencyObject> GetContainerList()
        {
            List<DependencyObject> containers = new List<DependencyObject>(_containerToItem.Keys);

            return new ReadOnlyCollection<DependencyObject>(containers);
        }

        public int IndexFromContainer(DependencyObject container)
        {
            if (container == null)
                return -1;

            // Get the item for the container
            object item = ItemFromContainer(container);

            if (item != null)
            {
                return _itemCollection.IndexOf(item);
            }

            return -1;
        }

        public bool IsFirstContainer(DependencyObject container)
        {
            if (container == null)
                return false;

            // Get the item for the container
            object item = ItemFromContainer(container);

            if (item != null)
            {
                return _itemCollection.IndexOf(item) == 0;
            }

            return false;
        }

        public bool IsLastContainer(DependencyObject container)
        {
            if (container == null)
                return false;

            // Get the item for the container
            object item = ItemFromContainer(container);

            if (item != null)
            {
                return _itemCollection.IndexOf(item) == (_itemCollection.Count - 1);
            }

            return false;
        }

        public object ItemFromContainer(DependencyObject container)
        {
            if (container == null)
                return null;

            if (_containerToItem.ContainsKey(container))
            {
                return _containerToItem[container];
            }

            return null;
        }

        public void OnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    _containerToItem.Clear();
                    _itemToContainer.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // Remove the old items. The new items will be added via a call to AddContainer
                    RemoveItems(args.OldItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(args.OldItems);
                    break;
            }
        }


        private void RemoveItems(IList items)
        {
            foreach (object item in items)
            {
                DependencyObject container = ContainerFromItem(item);

                if (container != null && item != null)
                {
                    _containerToItem.Remove(container);
                    _itemToContainer.Remove(item);
                }
            }
        }


		#endregion Methods 
    }
}
