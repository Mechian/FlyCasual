﻿using Upgrade;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class TansariiPointVeteran : M3AInterceptor
        {
            public TansariiPointVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tansarii Point Veteran",
                    5,
                    17
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Serissu";
            }
        }
    }
}
