using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Model;

namespace Algorithms {


	/// <summary>
	/// First draft, heat was additive from adjacent tiles.
	/// </summary>
	public class AdvancePlay_v1 : AdvancePlayIf {

		static int turn = 0;

		static readonly float AIR_COOL = 0.3f;		// natural heat reduction
		static readonly int FLAME_LIMIT = 3;		// cap on flame size
		static readonly float SPARK_FACTOR = 3.5f;	// need more heat to spark

				PlayMap map;

		public AdvancePlay_v1(PlayMap map) {
			this.map = map;
		}

		public void Next() {

			Debug.Log( "ADVANCING GAME STATE = "+(++turn) );

			AddDamage( map );
			KillSpark( map );
			CoolMap( map );
			AddHeat( map );
			AddSpark( map );

			Debug.Log( "ADVANCING GAME STATE [Done]" );
		}

		/// <summary>
		/// Flame creates damage
		/// </summary>
		/// <param name="map"></param>
		static internal void AddDamage( PlayMap map ) { 
		
			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];
				if (tile.Flame<1) continue;

				var dmg = tile.Burnt + tile.Flame;
				if (dmg>tile.Fuel) dmg = tile.Fuel;
				tile.Burnt = dmg;
			}
		}

		/// <summary>
		/// Lack of fuel kills flames.
		/// </summary>
		/// <param name="map"></param>
		static internal void KillSpark( PlayMap map ) { 
				
			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];
				if (tile.Flame<1) continue;

				var fuel = (int)(tile.Fuel - tile.Burnt);
				if (tile.Flame>fuel) tile.Flame = fuel;
			}	
		}

		/// <summary>
		/// Water and air will cool tiles.
		/// </summary>
		/// <param name="map"></param>
		static internal void CoolMap( PlayMap map ) {
			
			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];

				var heat = tile.Heat;
				heat -= tile.Water;		// player effects
				heat *= (1f-AIR_COOL);	// natural heat decay

				tile.Heat = heat;
			}	 
		}

		/// <summary>
		/// Heat is added to current tile and all adjacent tiles.
		/// </summary>
		/// <param name="map"></param>
		static internal void AddHeat( PlayMap map ) { 
			
			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];
				var flame = tile.Flame;
				if (flame<1) continue;

				// diagonal
				var spark = flame;
				AddTileHeat( map, spark, ix-1, iy-1 );
				AddTileHeat( map, spark, ix-1, iy+1 );
				AddTileHeat( map, spark, ix+1, iy-1 );
				AddTileHeat( map, spark, ix+1, iy+1 );

				// orthoganol
				spark = 2*flame;
				AddTileHeat( map, spark, ix, iy-1 );
				AddTileHeat( map, spark, ix-1, iy );
				AddTileHeat( map, spark, ix, iy+1 );
				AddTileHeat( map, spark, ix+1, iy );

				// current
				tile.Heat += 3*spark;
			}	 
		}

		static internal void AddTileHeat( PlayMap map, float add, int ix, int iy ) {
			if (ix<0 || iy<0 || ix>=map.Wide || iy>=map.Tall) return;
			map.Grid[ix,iy].Heat += add;
		}

		/// <summary>
		/// Heat creates flame.
		/// </summary>
		/// <param name="map"></param>
		static internal void AddSpark( PlayMap map ) { 

			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];

				int spark = (int)( tile.Heat / ( SPARK_FACTOR * tile.Ignite ) );
				if (spark>FLAME_LIMIT) spark = FLAME_LIMIT;
				if (spark>tile.Flame) tile.Flame = spark;
			}	
		}
	}
}
