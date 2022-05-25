using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Model;

namespace Algorithms {


	public class AdvancePlay_v3 : AdvancePlayIf {

		PlayMap map;

		public AdvancePlay_v3(PlayMap map) {
			this.map = map;
		}

		public void Next() {

			var turn = ( map.Turn += 1 );
			Debug.Log( "ADVANCING GAME STATE = "+(++turn) );

			AddDamage( map );
			KillSpark( map );

			SmoothHeat( map );
			RaiseHeat( map );
			EvaporateHeat( map );
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
		/// Heat is added to current tile and all adjacent tiles.
		/// </summary>
		/// <param name="map"></param>
		static internal void RaiseHeat( PlayMap map ) { 
			
			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];
				var flame = tile.Flame;
				//if (flame<1) continue;		// need to do this for flame==0 as well

				var max = flame * ModelSettings.MAX_FLAME_HEAT;
				var add = ( max - tile.Heat ) * ModelSettings.APPROACH_MAX_HEAT_RATE;
				tile.Heat += add;
			}	 
		}

		/// <summary>
		/// Leads to circular expansion in uniform terrain.
		/// Expands 6 tiles every 11 turns ( not 1 every 2 turns like I thought ).
		/// </summary>
		/// <param name="map"></param>
		static internal void SmoothHeat( PlayMap map ) {

			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];
				var neighbors = GetTileHeat( map, ix-1, iy ) +
						GetTileHeat( map, ix+1, iy ) +
						GetTileHeat( map, ix, iy-1 ) +
						GetTileHeat( map, ix, iy+1 );

				//tile.Temp = ( neighbors/2f + tile.Heat ) / 2f;
				tile.Temp = neighbors/2f + tile.Heat;			// halving is effectively done with 'raise-heat' harsh approach to max
			}	

			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {
				var tile = map.Grid[ix,iy];
				tile.Heat = tile.Temp;
			}	
		}

		/// <summary>
		/// Reduce heat by turning water into steam.
		/// </summary>
		/// <param name="map"></param>
		static internal void EvaporateHeat( PlayMap map ) {

			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];
				tile.Steam = 0;

				var evaporate = Mathf.Min( tile.Heat, tile.Water );
				if ( evaporate<=0 ) continue;

				tile.Heat -= evaporate;
				tile.Water -= evaporate;
				tile.Steam = (int)evaporate;
			}	
		}

		static internal float GetTileHeat( PlayMap map, int ix, int iy ) {
			if (ix<0 || iy<0 || ix>=map.Wide || iy>=map.Tall) return 0f;
			return map.Grid[ix,iy].Heat;
		}

		/// <summary>
		/// Heat creates flame.
		/// </summary>
		/// <param name="map"></param>
		static internal void AddSpark( PlayMap map ) { 

			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {
				var tile = map.Grid[ix,iy];
				if (tile.Heat<tile.Ignite) continue;
				if ( tile.Fuel-tile.Burnt < 1 ) continue;
				tile.Flame = 1;
			}	
		}
	}
}
