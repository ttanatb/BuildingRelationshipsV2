﻿using Dialogue.Struct;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue.SO
{
    [CreateAssetMenu(fileName = "StartDialogueEvent", menuName = "br/Dialogue/StartDialogueEvent", order = 1)]
    public class StartDialogueEvent : SoCustomEvent<StartDialogueData>
    {
        
    }
}
