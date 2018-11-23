﻿using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class BobaFettScum : Firespray31
        {
            public BobaFettScum() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    8,
                    39,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettScumAbility)
                );

                ModelInfo.SkinName = "Boba Fett";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BobaFettScumAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                GetNumberOfEnemyShipsAtRange1
            );
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) || (Combat.AttackStep == CombatStep.Defence))
            {
                if (GetNumberOfEnemyShipsAtRange1() > 0) result = true;
            }
            return result;
        }

        private int GetNumberOfEnemyShipsAtRange1()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Enemy).Count;
        }

        private int GetDiceModificationPriority()
        {
            return 90;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}