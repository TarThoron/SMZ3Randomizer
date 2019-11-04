﻿using System.Collections.Generic;
using static Randomizer.SMZ3.SMLogic;

namespace Randomizer.SMZ3.Regions.SuperMetroid {

    class CrateriaWest : SMRegion {

        public override string Name => "Crateria West";
        public override string Area => "Crateria";

        public CrateriaWest(World world, Config config) : base(world, config) {
            Locations = new List<Location> {
                new Location(this, 8, 0xC78432, LocationType.Visible, "Energy Tank, Terminator"),
                new Location(this, 5, 0xC78264, LocationType.Visible, "Energy Tank, Gauntlet", Logic switch {
                    Casual => items => CanEnterAndLeaveGauntlet(items) && items.HasEnergyReserves(1),
                    _ => items => CanEnterAndLeaveGauntlet(items)
                }),
                new Location(this, 9, 0xC78464, LocationType.Visible, "Missile (Crateria gauntlet right)", Logic switch {
                    Casual => items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages() && items.HasEnergyReserves(2),
                    _ => items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages()
                }),
                new Location(this, 10, 0xC7846A, LocationType.Visible, "Missile (Crateria gauntlet left)", Logic switch {
                    Casual => items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages() && items.HasEnergyReserves(2),
                    _ => items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages()
                })
            };
        }

        public override bool CanEnter(Progression items) {
            return items.CanDestroyBombWalls() || items.SpeedBooster;
        }

        private bool CanEnterAndLeaveGauntlet(Progression items) {
            return Logic switch {
                Casual =>
                    items.Morph && (items.CanFly() || items.SpeedBooster) && (
                        items.CanIbj() ||
                        items.CanUsePowerBombs() && items.PowerBombs >= 2 ||
                        items.ScrewAttack),
                _ =>
                    items.Morph && (items.Bombs || items.PowerBombs >= 2) ||
                    items.ScrewAttack ||
                    items.SpeedBooster && items.CanUsePowerBombs() && items.HasEnergyCapacity(2)
            };
        }

    }

}