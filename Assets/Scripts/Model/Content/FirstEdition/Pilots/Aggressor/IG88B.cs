﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Aggressor
    {
        public class IG88B : Aggressor
        {
            public IG88B() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88B",
                    6,
                    36,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.IG88BAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Red";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class IG88BAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckIG88Ability;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckIG88Ability;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckIG88Ability()
        {
            if (!IsAbilityUsed && !HostShip.IsCannotAttackSecondTime && HasCannonWeapon())
            {
                IsAbilityUsed = true;

                // Trigger must be registered just before it's resolution
                HostShip.OnCombatCheckExtraAttack += RegisterIG88BAbility;
            }
        }

        private bool HasCannonWeapon()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n.HasType(UpgradeType.Cannon) && (n as IShipWeapon) != null) > 0;
        }

        private void RegisterIG88BAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterIG88BAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseIG88BAbility);
        }

        private void UseIG88BAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsCannonShot,
                    "IG-88B",
                    "You may perform a cannon attack.",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsCannonShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            GenericSecondaryWeapon upgradeWeapon = weapon as GenericSecondaryWeapon;
            if (upgradeWeapon != null && upgradeWeapon.HasType(UpgradeType.Cannon))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must be performed from Cannon");
            }

            return result;
        }
    }
}