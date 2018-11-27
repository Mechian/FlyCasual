﻿using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using System;

namespace UpgradesList.SecondEdition
{
    public class CrackShot : GenericUpgrade
    {
        public CrackShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Crack Shot",
                UpgradeType.Elite,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.CrackShotAbility),
                charges: 1,
                seImageNumber: 1
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CrackShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += CrackShotDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= CrackShotDiceModification;
        }

        private void CrackShotDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SecondEdition.CrackShotDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = this.HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}


namespace ActionsList.SecondEdition
{
    //While you perform a primary weapon attack, if the defender is in your bullseye firing arc, before the neutralize results step, you may spend one charge to cancel one evade result.
    public class CrackShotDiceModification : ActionsList.CrackShotDiceModification
    {

        public CrackShotDiceModification() : base()
        {
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Source.Charges > 0 && Combat.Attacker == Host && Combat.ChosenWeapon is PrimaryWeaponClass && Combat.ShotInfo.InArcByType(Arcs.ArcTypes.Bullseye))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Source.SpendCharge();
            callBack();
        }

    }

}