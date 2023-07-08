using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static List<T> Randomize<T>(this List<T> list)
    {
        List<T> temp = new List<T>(list);
        List<T> result = new List<T>();
        int size = list.Count;
        for (int i = 0; i < size; i++)
        {
            T choice = temp.Random();
            result.Add(choice);
            temp.Remove(choice);
        }
        return result;
    }

    public static T Random<T>(this List<T> list)
    {
        // Use Unity's random number generator so that we can mess with random seeds and stuff
        if (list.Count == 0) throw new System.Exception("Cannot get a random element from a list that has no elements");
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Chooses an element from the list randomly using weights for the random choice. Each weight must be a value between 0 and 1.
    /// </summary>
    public static T WeightedRandom<T>(this List<T> list, List<float> weights)
    {
        if (weights.Count != list.Count) throw new System.Exception("List of weights must be the same length as the list to get a random element from");

        List<int> indices = new List<int>();
        float totalWeight = 0f;
        for (int i = 0; i < weights.Count; i++)
        {
            indices.Add(i);
            totalWeight += weights[i];
        }


        List<float> newWeights = new List<float>();
        List<T> newList = new List<T>();
        List<int> randomIndices = indices.Randomize();
        foreach (int index in randomIndices)
        {
            newWeights.Add(weights[index]);
            newList.Add(list[index]);
        }


        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float total = 0f;
        for (int i = 0; i < newWeights.Count; i++)
        {
            if (total <= rand && rand < total + newWeights[i]) return newList[i];
            total += newWeights[i];
        }
        return newList[newList.Count - 1];

        //List<T> rlist = new List<T>();
        //for (int i = 0; i < weights.Count; i++)
        //    for (int j = 0; j < (int)((1.0f - weights[i]) * 100f); i++) rlist.Add(list[i]);
        //return rlist.Random();
    }

    public static string GetString<T>(this List<T> list)
    {
        return string.Format("List({0})", string.Join(", ", list));
    }

    /// <summary>
    /// Sort the list using the given indices
    /// </summary>
    public static void SortBy<T>(this List<T> list, List<int> indices)
    {
        if (indices.Count != list.Count) throw new System.ArgumentException("The given list of indices must have the same length as the list itself");
        List<T> sorted = new List<T>();
        foreach (int i in indices)
            sorted.Add(list[i]);
        list = sorted;
    }

    /// <summary>
    /// Sort the list by the given values
    /// </summary>
    public static void SortBy<T>(this List<T> list, List<float> values)
    {
        if (values.Count != list.Count) throw new System.ArgumentException("The given list of values must have the same length as the list itself");
        list.SortBy(values.GetSortedIndices());
    }


    /// <summary>
    /// Get a List<int> of the indices which could be used to sort the list
    /// </summary>
    public static List<int> GetSortedIndices<T>(this List<T> list)
    {
        List<T> sorted = new List<T>(list);
        sorted.Sort();
        List<int> indices = new List<int>();
        foreach (T item in sorted) indices.Add(list.IndexOf(item));
        return indices;
    }

    /// <summary>
    /// The usual List.Remove method uses System.Object.Equals method to determine which object in the list should be removed. You can
    /// use this method instead if you want to use the System.ReferenceEquals method instead of System.Object.Equals.
    /// </summary>
    public static bool RemoveByReference<T>(this List<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (ReferenceEquals(list[i], value))
            {
                list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// The usual List.Contains method uses System.Object.Equals method to determine which object in the list should be removed. You can
    /// use this method instead if you want to use the System.ReferenceEquals method instead of System.Object.Equals.
    /// </summary>
    public static bool ContainsByReference<T>(this List<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (ReferenceEquals(list[i], value)) return true;
        }
        return false;
    }


    // -----
    // The following are from https://stackoverflow.com/a/30579982/4954083

    /// <summary>
    /// A faster List.Remove method when order doesn't matter. O(1)
    /// </summary>
    public static void RemoveBySwap<T>(this List<T> list, int index)
    {
        // O(1) 
        list[index] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
    }

    /// <summary>
    /// A faster List.Remove method when order doesn't matter. O(n)
    /// </summary>
    public static void RemoveBySwap<T>(this List<T> list, T item)
    {
        int index = list.IndexOf(item);
        RemoveBySwap(list, index);
    }

    /// <summary>
    /// A faster List.Remove method when order doesn't matter. O(n)
    /// </summary>
    public static void RemoveBySwap<T>(this List<T> list, Predicate<T> predicate)
    {
        int index = list.FindIndex(predicate);
        RemoveBySwap(list, index);
    }
    // -----



}
