using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Model.TileEnum;

namespace Model {

	public class TileAttributes {

		static int FUEL_MULTIPLIER = 3;

		internal static readonly Dictionary<TileEnum,TileAttributes> map;
		

		static TileAttributes() {

			map = new Dictionary<TileEnum, TileAttributes>();

			new TileAttributes( Field, 1, 1, 2, 1f );
			new TileAttributes( Grass, 1, 1, 2, 0.7f );
			new TileAttributes( Shrubs, 2, 2, 3, 1/2f );
			new TileAttributes( Woods, 3, 3, 4, 1/3f );
			new TileAttributes( Forest, 4, 4, 5, 1/4f );

			new TileAttributes( Stone, 0, 8, 0, 0f );		// does not burn
			new TileAttributes( River, 0, 8, 0, 0f );		// does not burn

			new TileAttributes( Cottage, 3, 2, 3, 1/2f );
			new TileAttributes( Estate, 2, 3, 4, 1/3f );
			new TileAttributes( Well, 1, 5, 5, 1/4f );
			new TileAttributes( Wall, 1, 5, 5, 1/8f );

			new TileAttributes( Bridge, 1, 3, 3, 1/2f );
			new TileAttributes( Road, 0.5f, 2, 2, 1/3f );
			new TileAttributes( Street, 0.5f, 3, 2, 1/4f );
		}


		public TileAttributes( TileEnum type, float fuel, float ignite, float heat, float spread ) { 
			
			this.Type = type;
			this.Fuel = fuel * FUEL_MULTIPLIER;
			//this.Water = water;
			this.Ignite = ignite;
			this.Heat = heat;
			this.Spread = spread;

			map[type] = this;
		}

		static public TileAttributes Get( TileEnum type ) {
			return map[type];
		}


		public TileEnum Type { get; internal set; }

		public float Fuel { get; internal set; }

		public float Water { get; internal set; } = 0;

		/// <summary>
		/// Heat -> Ignite3 defines the chain of ignition.
		/// </summary>
		public float Ignite { get; internal set; }

		/// <summary>
		/// Heat -> Ignite defines the chain of ignition.
		/// </summary>
		public float Heat { get; internal set; }

		/// <summary>
		/// Rate of spread.  Usually the inverse of 'turns to advance'.
		/// So 1, 1/2, 1/3, etc.
		/// </summary>
		public float Spread { get; internal set; }
	}
}
