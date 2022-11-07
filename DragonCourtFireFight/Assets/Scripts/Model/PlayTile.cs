using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
	public class PlayTile {

		public PlayTile( TileEnum type ) {
			this.Type = type;
			var attr = TileAttributes.Get(type);
			this.Fuel = attr.Fuel * ModelSettings.FUEL_MULTIPLIER;
			this.Ignite = attr.Ignite;
			this.Water = attr.Water;
		}

		public TileEnum Type { get; internal set; }

		public TileAttributes Attributes { get { return TileAttributes.Get(Type); } }

		public UnitEnum Unit { get; set; } = UnitEnum.None;

		// how much can burn in the tile
		public float Fuel { get; set; } = 0;

		// burnt reduces fuel
		public float Burnt { get; set; } = 0;

		// heat and fuel create flame
		public float Heat { get; set; } = 0;

		// used by algorithms for map advancement
		public float Temp { get; set; } = 0;

		// flame consumes fuel and creates heat: this is 'int' for player comprehension
		public int Flame { get; set; } = 0;

		// threshold to catch fire
		public float Ignite {  get; set; } = 0;

		// added reduction of heat, player managed resource
		public float Water { get; set; } = 0;

		// steam indicates that there was evaporation:  this is 'int' for player comprehension
		public int Steam { get; set; } = 0;

		// how much of the tile is on fire.
		public float Engagement { get; set; } = 0f;

		public Mob Mob { get; set; } = null;

	}
}
