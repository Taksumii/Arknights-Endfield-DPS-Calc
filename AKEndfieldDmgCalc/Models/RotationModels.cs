using System;
using System.Collections.Generic;

namespace AKEndfieldDmgCalc.Models
{
    public enum ActionKind
    {
        Wait,
        BasicAttackChain,
        BattleSkill,
        ComboSkill,
        Ultimate,
        SwitchControlled 
    }

    public sealed class TeamSetup
    {
        public string[] OperatorIds { get; init; } = Array.Empty<string>(); // length 4 expected
        public int ControlledIndex { get; set; } = 0;
        public int[] SelectedPotentialTier { get; init; } = Array.Empty<int>(); 
    }

    public sealed class TeamResources
    {
        public double TeamSp { get; set; }
        public double TeamSpMax { get; set; } = 300;
    }

    public sealed class OperatorRuntime
    {
        public string OperatorId { get; init; } = "";
        public double UltEnergy { get; set; }
        public double UltEnergyMax { get; set; } = 300;
        public HashSet<string> ActiveStates { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class RotationAction
    {
        public ActionKind Kind { get; init; }
        public int ActorIndex { get; init; } 
        public double TimeSeconds { get; init; } // scheduled start time

        public double? WaitDurationSeconds { get; init; } // for Wait
        public int? TargetsHit { get; init; } // for gains depending on enemies hit
        public int? SwitchToIndex { get; init; } // for SwitchControlled
    }
}
