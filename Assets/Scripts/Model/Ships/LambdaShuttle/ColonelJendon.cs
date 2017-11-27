﻿using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class ColonelJendon : LambdaShuttle
        {
            public ColonelJendon() : base()
            {
                PilotName = "Colonel Jendon";
                PilotSkill = 6;
                Cost = 26;
                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.ColonelJendonAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class ColonelJendonAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnCombatPhaseStart += RegisterColonelJendonAbility;
        }

        private void RegisterColonelJendonAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, StartSubphaseForColonelJendonAbility);            
        }
        
        private void StartSubphaseForColonelJendonAbility(object sender, System.EventArgs e)
        {
            if (Host.Owner.Ships.Count > 1 && Host.HasToken(typeof(Tokens.BlueTargetLockToken), '*'))
            {
                var pilotAbilityDecision = (ColonelJendonDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(ColonelJendonDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                pilotAbilityDecision.InfoText = "Use Colonel Jendon's ability?";

                var blueTargetLocks = Host.GetAssignedTokens()
                   .Where(t => t is Tokens.BlueTargetLockToken)
                   .Select(x => (Tokens.BlueTargetLockToken)x)
                   .OrderBy(y => y.Letter)
                   .ToList();

                pilotAbilityDecision.AddDecision("No", DontUseColonelJendonAbility);

                blueTargetLocks.ForEach(l => {
                    var name = "Target Lock " + l.Letter;
                    pilotAbilityDecision.AddDecision(name, delegate { UseColonelJendonAbility(l.Letter); });
                    pilotAbilityDecision.AddTooltip(name, l.OtherTokenOwner.ImageUrl);
                });

                pilotAbilityDecision.DefaultDecision = "No";

                pilotAbilityDecision.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        
        private void UseColonelJendonAbility(char letter)
        {
            Tooltips.EndTooltip();

            SelectTargetForAbility(
                SelectColonelJendonAbilityTarget,
                new List<TargetTypes> { TargetTypes.OtherFriendly },
                new UnityEngine.Vector2(1, 1)
            );
        }


        private void SelectColonelJendonAbilityTarget()
        {
            if (TargetShip.HasToken(typeof(Tokens.BlueTargetLockToken), '*'))
            {
                Messages.ShowErrorToHuman("Only ships without blue target lock tokens can be selected");
                return;
            }

            MovementTemplates.ReturnRangeRuler();

            var token = Host.GetToken(typeof(Tokens.BlueTargetLockToken), '*') as Tokens.BlueTargetLockToken;

            Host.ReassignTargetLockToken(
                typeof(Tokens.BlueTargetLockToken),
                token.Letter,
                TargetShip,
                delegate{
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    DecisionSubPhase.ConfirmDecision();
                });
        }

        private void DontUseColonelJendonAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private class ColonelJendonDecisionSubPhase : DecisionSubPhase
        {
            public override void SkipButton()
            {
                ConfirmDecision();
            }
        }
    }
}
