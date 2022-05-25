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

			new TileAttributes( Field, 1, 1, 0 );
			new TileAttributes( Grass, 1, 1, 1 );
			new TileAttributes( Shrubs, 2, 1, 2 );
			new TileAttributes( Woods, 3, 2, 3 );
			new TileAttributes( Forest, 4, 2, 4 );
			new TileAttributes( Stone, 0, 8, 0 );

			new TileAttributes( Cottage, 4, 1, 1 );
			new TileAttributes( Estate, 2, 2, 1 );
			new TileAttributes( Well, 1, 1, 5 );
			new TileAttributes( Wall, 2, 2, 1 );

			new TileAttributes( River, 0, 8, 8 );
			new TileAttributes( Bridge, 1, 2, 5 );
			new TileAttributes( Road, 1, 2, 1 );
			new TileAttributes( Street, 1, 3, 0 );
		}


		public TileAttributes( TileEnum type, float fuel, float ignite, float water ) {  
			this.Type = type;
			this.Fuel = fuel * FUEL_MULTIPLIER;
			this.Ignite = ignite;
			this.Water = water;
			map[type] = this;
		}

		static public TileAttributes Get( TileEnum type ) {
			return map[type];
		}


		public TileEnum Type { get; internal set; }

		public float Fuel { get; internal set; }

		public float Ignite { get; internal set; }

		public float Water { get; internal set; }
	}
}
