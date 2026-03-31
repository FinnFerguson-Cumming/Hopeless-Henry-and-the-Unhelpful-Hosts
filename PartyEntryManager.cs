using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyEntryManager : MonoBehaviour
{
    private DraggableLetter[] letters;
    private Vector3[] startPositions;
    private Transform[] startParents;

    private void Awake()
    {
        letters = FindObjectsOfType<DraggableLetter>();
        startPositions = new Vector3[letters.Length];
        startParents = new Transform[letters.Length];

        for (int i = 0; i < letters.Length; i++)
        {
            startPositions[i] = letters[i].transform.localPosition;
            startParents[i] = letters[i].transform.parent;
        }
    }

    public void ResetPuzzle()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].transform.SetParent(startParents[i]);
            letters[i].transform.localPosition = startPositions[i];
            letters[i].OriginalParent = startParents[i];
        }
    }
}