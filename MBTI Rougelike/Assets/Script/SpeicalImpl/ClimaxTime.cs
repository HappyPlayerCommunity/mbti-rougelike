using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimaxTime : PersonalitySpecialImplementation
{
    EsfpNoteManager esfpNoteManager;

    public override void ExecuteSpecialImplementation(Personality personality)
    {
        esfpNoteManager = personality.GetComponentInChildren<EsfpNoteManager>();


    }
}
