using System.Xml;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Text;

[System.Serializable]

public class TranslationKeyGenerator : Editor
{
    private string[] alpb, num;
    void Start()
    {
        // num = new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        // alpb = new string[26] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        Debug.Log(GenerateKey()); // Just for the sake of a simple test
    }
    /*
     * nodeCount of 3 and nodeCharCount of 5 will produce something like this
     * yu67h-7uyh8-i8uy6
     */
    public string GenerateKey()
    {
        num = new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        alpb = new string[26] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        int nodeCount = 1;
        int nodeCharCount = UnityEngine.Random.Range(4, 12);
        //Shuffle our arrays first so that every time we get a random key
        ShuffleArray<string>(num);
        ShuffleArray<string>(alpb);
        nodeCount = Mathf.Clamp(nodeCount, 2, 5);
        nodeCharCount = Mathf.Clamp(nodeCharCount, 3, 5);
        int numIndex = 0, alpIndex = 0, insertInt = 0;
        StringBuilder sB = new StringBuilder();
        for (int i = 1; i <= nodeCount; i++)
        {
            for (int j = 0; j < nodeCharCount; j++)
            {
                insertInt = UnityEngine.Random.Range(0, 2); // 0 means we will insert an alphabet in our key code and 1 means we will go with a number
                if (insertInt == 0)
                {
                    int r = UnityEngine.Random.Range(0, 20);
                    
                    if (r >= 10)
                    {
                        Char[] c = alpb[alpIndex].ToCharArray();
                        c[0] = Char.ToUpper(c[0]);
                        var upChar = c[0].ToString();
                        alpb[alpIndex] = upChar;
                        sB.Append(alpb[alpIndex]);
                    }
                    else
                    {
                        sB.Append(alpb[alpIndex]);
                    }

                    alpIndex++;
                    if (alpIndex == alpb.Length)
                    {
                        alpIndex = 0;
                    }
                }
                else
                {
                    sB.Append(num[numIndex]);
                    numIndex++;
                    if (numIndex == num.Length)
                    {
                        numIndex = 0;
                    }
                }
            }
            // if (i < nodeCount)
            // {
            //     sB.Append("-");
            // }
        }
        return sB.ToString();
    }

    static void ShuffleArray<T>(T[] arr)
    { // This will not create a new array but return the original but shuffled array
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }
}