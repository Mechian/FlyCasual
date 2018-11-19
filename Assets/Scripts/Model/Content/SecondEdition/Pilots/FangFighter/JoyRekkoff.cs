﻿using Ship;
using SubPhases;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class JoyRekkoff : FangFighter
        {
            public JoyRekkoff() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Joy Rekkoff",
                    4,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KadSolusAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 157;
            }
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class JoyRekkoffAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.OnAttackStartAsAttacker += CheckAbility;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnAttackStartAsAttacker -= CheckAbility;
            }

            private void CheckAbility()
            {
                if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.Types.Contains(UpgradeType.Torpedo) && n.Charges > 0))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskYoUseJoyRekkoffAbility);
                }
            }

            private void AskYoUseJoyRekkoffAbility(object sender, System.EventArgs e)
            {
                if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.Types.Contains(UpgradeType.Torpedo) && n.Charges > 0))
                {
                    AskToUseAbility(AlwaysUseByDefault, UseJoyRekkoffAbility);
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

            private void UseJoyRekkoffAbility(object sender, System.EventArgs e)
            {
                GenericSecondaryWeapon torpedo = (GenericSecondaryWeapon)HostShip.UpgradeBar.GetUpgradesOnlyFaceup().FirstOrDefault(n => n.Types.Contains(UpgradeType.Torpedo) && n.Charges > 0);
                torpedo.SpendCharge();
                AssignConditionToDefender();
            }

            private void AssignConditionToDefender()
            {
                Combat.Defender.Tokens.AssignCondition(typeof(Conditions.JoyRekkoffCondition));
                DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}

namespace Conditions
{
    public class JoyRekkoffCondition : GenericToken
    {
        bool AgilityWasDecreased = false;

        public JoyRekkoffCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            TooltipType = typeof(Ship.SecondEdition.FangFighter.JoyRekkoff);

            Temporary = false;
        }

        public override void WhenAssigned()
        {
            if (Host.State.Agility != 0)
            {
                AgilityWasDecreased = true;

                Messages.ShowError("Joy Rekkoff: Agility is decreased");
                Host.ChangeAgilityBy(-1);
            }

            Host.OnAttackFinishAsDefender += RemoveJoyRekkoffAbility;
        }

        private void RemoveJoyRekkoffAbility(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            if (AgilityWasDecreased)
            {
                Messages.ShowInfo("Agility is restored");
                Host.ChangeAgilityBy(+1);
            }

            Host.OnAttackFinishAsDefender -= RemoveJoyRekkoffAbility;
        }
    }
}