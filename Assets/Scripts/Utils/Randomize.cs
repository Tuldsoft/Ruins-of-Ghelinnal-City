using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Provides methods to randomize either a list or an array of type T. Uses the UnityEngine RNG.
public static class Randomize<T>
{
    
    // Output the incoming array as an array in random order
    public static T[] Array(T[] objs)
    {
        // Create a list of numbers from 0 to the length of the orig array
        List<int> origIndexes = new List<int>();
        for (int i = 0; i < objs.Length; i++)
        {
            origIndexes.Add(i);
        }

        // a list to store index numbers, 0 to length
        List<int> randIndexes = new List<int>(objs.Length);

        // create an index for the list of index numbers, adds the index
        // to the list of indexes, and removes that number from the list of available indices

        int randOrig = Random.Range(0, origIndexes.Count);
        randIndexes.Add(origIndexes[randOrig]);
        origIndexes.RemoveAt(randOrig);

        // until the randomized list is full, keep filling it by
        // randomly drawing from the available options. Reduce the options
        // as we go so that we never randomly pick something already picked.

        while (randIndexes.Count < randIndexes.Capacity)
        {
            randOrig = Random.Range(0, origIndexes.Count);
            randIndexes.Add(origIndexes[randOrig]);
            origIndexes.RemoveAt(randOrig);
        }

        // build the new array using randomized indices
        T[] outputArray = new T[objs.Length];

        for (int i = 0; i < randIndexes.Count; i++)
        {
            outputArray[i] = objs[randIndexes[i]];
        }

        return outputArray;
    }

    // output the incoming list as an array in random order
    public static List<T> List(List<T> objs)
    {
        // create a sequential list of integers from 0 to the count of the orig list
        List<int> origIndexes = new List<int>();
        for (int i = 0; i < objs.Count; i++)
        {
            origIndexes.Add(i);
        }

        // a list to store index numbers, 0 to count
        List<int> randIndexes = new List<int>(objs.Count);

        // create an index for the list of index numbers, adds the index to the list
        // of indexes, and removes that number from the list of available indices

        int randOrig = Random.Range(0, origIndexes.Count);
        randIndexes.Add(origIndexes[randOrig]);
        origIndexes.RemoveAt(randOrig);

        // until the randomized list is full, keep filling it by
        // randomly drawing from the available options. Reduce the options
        // as we go so that we never randomly pick something already picked.

        while (randIndexes.Count < randIndexes.Capacity)
        {
            randOrig = Random.Range(0, origIndexes.Count);
            if (!randIndexes.Contains(origIndexes[randOrig]))
            {
                randIndexes.Add(origIndexes[randOrig]);
                origIndexes.RemoveAt(randOrig);
            }

        }

        // build the new list using randomized indices
        List<T> outputList = new List<T>();

        for (int i = 0; i < randIndexes.Count; i++)
        {
            outputList.Add(objs[randIndexes[i]]);
        }

        return outputList;
    }
}
