﻿using System.Collections;
using System.Collections.Generic;
using Ship;
using System;
using Tokens;
using SubPhases;
using Abilities.FirstEdition;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class JekPorkins : XWing
        {
            public JekPorkins() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jek Porkins",
                    7,
                    26,
                    limited: 1,
                    abilityText: "When you receive a stress token, you may remove it and roll 1 attack die. On a hit result, deal 1 facedown Damage card to this ship.",
                    abilityType: typeof(JekPorkinsAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class JekPorkinsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= CheckAbilityConditions;
        }

        private void CheckAbilityConditions(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(StressToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskToUsePilotAbility);
            }
        }

        private void AskToUsePilotAbility(object sender, EventArgs e)
        {
            AskToUseAbility(ShouldUseAbility, RemoveStressAndRollDice);
        }

        private bool ShouldUseAbility()
        {
            return HostShip.State.HullCurrent > 1;
        }

        private void RemoveStressAndRollDice(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.RemoveToken(typeof(StressToken), StartRollDiceSubphase);
        }

        private void StartRollDiceSubphase()
        {
            PerformDiceCheck(
                "Jek Porkins: Facedown damage card on hit",
                DiceKind.Attack,
                1,
                FinishAction,
                Triggers.FinishTrigger
            );
        }

        private void FinishAction()
        {
            if (DiceCheckRoll.RegularSuccesses > 0)
            {
                SufferNegativeEffect(AbilityDiceCheck.ConfirmCheck);
            }
            else
            {
                AbilityDiceCheck.ConfirmCheck();
            }
        }

        protected virtual void SufferNegativeEffect(Action callback)
        {
            HostShip.Damage.SufferFacedownDamageCard(
                new DamageSourceEventArgs()
                {
                    Source = HostShip,
                    DamageType = DamageTypes.CardAbility
                },
                callback
            );
        }
    }
}