using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "New Note NoteData")]
public class NoteData : ScriptableObject
{
    public int bpm = 100;
    public List<float> noteDropTiming = Enumerable.Repeat(-1f, 100).ToList();
}
