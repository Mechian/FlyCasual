﻿using Ship;
using System.Collections.Generic;
using System;
using SubPhases;

namespace Ship
{
    namespace HWK290
    {
        public class RoarkGarnet : HWK290
        {
            public RoarkGarnet() : base()
            {
                PilotName = "Roark Garnet";
                PilotSkill = 4;
                Cost = 19;

                faction = Faction.Rebel;

                PilotAbilities.Add(new PilotAbilitiesNamespace.RoarkGarnetAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class RoarkGarnetAbility : GenericPilotAbility, IModifyPilotSkill
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnCombatPhaseStart += RegisterAbility;
            Host.OnDestroyed += RemoveAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        private void RemoveAbility(GenericShip ship)
        {
            Phases.OnCombatPhaseStart -= RegisterAbility;
        }

        private void Ability(object sender, EventArgs e)
        {
            if (Host.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget, 
                    new List<TargetTypes> { TargetTypes.OtherFriendly }, 
                    new UnityEngine.Vector2(1, 3), 
                    false);                
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectAbilityTarget()
        {
            TargetShip.AddPilotSkillModifier(this);
            Phases.OnEndPhaseStart += RemovePilotSkillModifieer;
            SelectShipSubPhase.FinishSelection();
        }

        private void RemovePilotSkillModifieer()
        {
            TargetShip.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 12;
        }
    }
}
