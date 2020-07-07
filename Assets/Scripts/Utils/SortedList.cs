using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A sorted list
/// </summary>
public class SortedList<T> where T:IComparable
{
    List<T> items = new List<T>();

    // used in Add method
    List<T> tempList = new List<T>();
	
    #region Constructors

    /// <summary>
    /// No argument constructor
    /// </summary>
    public SortedList()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of items in the list
    /// </summary>
    /// <value>number of items in the list</value>
    public int Count
    {
        get { return items.Count; }
    }
	
    /// <summary>
    /// Gets the item in the array at the given index
    /// This property allows access using [ and ]
    /// </summary>
    /// <param name="index">index of item</param>
    /// <returns>item at the given index</returns>
    public T this[int index]
    {
        get { return items[index]; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the given item to the list
    /// </summary>
    /// <param name="item">item</param>
    public void Add(T item)
    {
        // run the loop only if the list isn't empty
        if (items.Count != 0)
        {
            // initialize a new tempList
            tempList = new List<T>();

            // use foundIt when the new item has been inserted
            bool foundIt = false;
            foreach (T listItem in items)
            {
                if (foundIt || listItem.CompareTo(item) < 0)
                {
                    // we are before or after where the new item is placed
                    tempList.Add(listItem);
                }
                else
                {
                    // we are at the place to add the new item
                    tempList.Add(item);
                    tempList.Add(listItem);
                    // only add the new item one time, then just finish populating tempList
                    foundIt = true;
                }
            }
            // if placement for the new item has not occured, add it to the end of the list
            if (!foundIt) { tempList.Add(item); }
            
            // empty the items list
            items.Clear();

            // fill the items list with the newly constructed tempList
            items = tempList;
        }
        else
        {
            // if the list is empty, just add the new item
            items.Add(item);
        }
    }

    /// <summary>
    /// Removes the item at the given index from the list
    /// </summary>
    /// <param name="index">index</param>
    public void RemoveAt(int index)
    {
        // add your implementation below
        items.RemoveAt(index);

    }

    /// <summary>
    /// Determines the index of the given item using binary search
    /// </summary>
    /// <param name="item">the item to find</param>
    /// <returns>the index of the item or -1 if it's not found</returns>
    public int IndexOf(T item)
    {
        int lowerBound = 0;
        int upperBound = items.Count - 1;
        int location = -1;

        // loop until found value or exhausted array
        while ((location == -1) &&
            (lowerBound <= upperBound))
        {
            // find the middle
            int middleLocation = lowerBound + (upperBound - lowerBound) / 2;
            T middleValue = items[middleLocation];

            // check for match
            if (middleValue.CompareTo(item) == 0)
            {
                location = middleLocation;
            }
            else
            {
                // split data set to search appropriate side
                if (middleValue.CompareTo(item) > 0)
                {
                    upperBound = middleLocation - 1;
                }
                else
                {
                    lowerBound = middleLocation + 1;
                }
            }
        }
        return location;
    }

    /// <summary>
    /// Sorts the list
    /// </summary>
    public void Sort()
    {
        items.Sort();
    }

    #endregion
}
