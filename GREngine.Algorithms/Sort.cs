namespace GREngine.Algorithms;

using System;
using System.Collections.Generic;
using System.Linq;

public static class Sort
{
    /// <summary>
    /// Takes two sorted lists and combines them into a larger sorted list (all in ascending order)
    /// </summary>
    /// <param name="a">List 1</param>
    /// <param name="b">List 2</param>
    /// <typeparam name="T">The type of both lists</typeparam>
    /// <returns>A sorted list with all elements combined</returns>
    public static IList<T> MergeSortedLists<T>(IList<T> a, IList<T> b) where T : IComparable<T>
    {
        if (a.Count > b.Count)
            return MergeSortedLists_Large_Small<T>(a, b);
        else
            return MergeSortedLists_Large_Small<T>(b, a);
    }
    private static IList<T> MergeSortedLists_Large_Small<T>(IList<T> a, IList<T> b) where T : IComparable<T>
    {
        // Algorithm from: Sujith Karivelil's answer on https://stackoverflow.com/questions/37780578/merging-two-lists-which-are-already-sorted-in-c-sharp
        int largeArrayCount = a.Count;
        int currentBIndex = 0;
        List<T> finalResult = new List<T>();

        for (int i = 0; i < largeArrayCount; i++)
        {
            if (i < b.Count)
            {
                if (a[i].CompareTo(b[i]) >= 0)
                {
                    // Add All elements of B Which is smaller than current element of A
                    while (b[i].CompareTo(a[currentBIndex]) <= 0)
                    {
                        finalResult.Add(b[currentBIndex++]);
                    }
                }
                else
                {
                    finalResult.Add(a[i]);
                }
            }
            else
            {
                // No more elements in b so no need for checking
                finalResult.Add(a[i]);
            }
        }

        return finalResult;
    }
}
