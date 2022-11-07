using Algorithms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static Model.TileEnum;
using static System.Math;

namespace Model {

	public class TileMapFactory {

		static System.Random rand = new System.Random( (int)DateTime.Now.Ticks );

		static public PlayMap Build( ) {
			return Build( 4, 4 );
		}

		static public PlayMap Build( int wide, int tall ) {

			var map = new PlayMap(wide,tall);

			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				var type =  ( ((ix+iy)&1)==0 ? Grass : Field );
				map.Grid[ix,iy] = new PlayTile(type);
			}

			return map;
		}

		static public PlayMap BuildOneType( int wide, int tall, TileEnum type ) {

			var map = new PlayMap(wide,tall);

			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				map.Grid[ix,iy] = new PlayTile(type);
			}

			map.AddSpark( wide/2, tall/2 );

			return map;
		}

		static public PlayMap BuildTwoType( int wide, int tall, TileEnum type1, TileEnum type2 ) {

			var map = new PlayMap(wide,tall);

			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				TileEnum drop = ( ((ix+iy)&1)==0 ? type1 : type2 );
				map.Grid[ix,iy] = new PlayTile(drop);
			}

			map.AddSpark( wide/2, tall/2 );
			return map;
		}

		static public PlayMap BuildVillage( int wide, int tall ) {

			var map = new PlayMap(wide,tall);

			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				map.Grid[ix,iy] = new PlayTile( Grass );
			}

			for (int ix=0;ix<10;ix++) AddRandomBlock( map );

			AddRiver( map );
			Where center = AddRoadWithWell( map );
Debug.Log("CENTER = "+center.X+" "+center.Y );
			AddSomeCity( map, center );

			AddSpark( map );
			return map;
		}

		static readonly TileEnum[] Quarters = { Field, Shrubs, Stone, Woods };

		static public PlayMap BuildQuarters( int wide, int tall ) {

			var map = new PlayMap(wide,tall);

			var w2 = wide/2;
			var t2 = tall/2;

			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				var index = ( ix>w2 ? 2 : 0 ) + ( iy>t2 ? 1 : 0 );
				map.Grid[ix,iy] = new PlayTile( Quarters[index] );
			}

			map.AddSpark( wide/4, tall/4 );

			return map;
		}
		
		static public PlayMap BlueNoisePoints( int wide, int tall ) {

			var map = new PlayMap(wide,tall);
			var pts = new BlueNoisePoints( wide, tall, 20 ).Build();


			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				map.Grid[ix,iy] = new PlayTile( TileEnum.Field );
			}

			foreach ( var pt in pts ) {
				map.Grid[pt.X,pt.Y] = new PlayTile( TileEnum.Grass );
			}

			map.AddSpark(0, 0);

			return map;
		}

		static public PlayMap FloodFillMap( int wide, int tall ) {

			var map = new PlayMap(wide,tall);
			var pts = new BlueNoisePoints( wide, tall, 30 ).Build();
			int[,] grid = new FillFromPoints( wide, tall, pts ).Build();

			// ensure zero/zero is field
			var offset = 8 -  ( grid[0,0] % 8 );

			for (int ix=0;ix<wide;ix++) for (int iy=0;iy<tall;iy++) {
				var type = NumToType( grid[ix,iy]+offset );
				map.Grid[ix,iy] = new PlayTile( type  );
			}

			map.AddSpark(0, 0);

			return map;
		}

		static internal TileEnum NumToType( int num ) {

			switch ( num % 8 ) {

				case 0: return TileEnum.Field;
				case 1: return TileEnum.Grass;
				case 2: return TileEnum.Shrubs;
				case 3: return TileEnum.Woods;

				case 4: return TileEnum.Forest;
				case 5: return TileEnum.Cottage;
				case 6: return TileEnum.Road;
				default: return TileEnum.Stone;

			}
		}

//=====================================================================================================================

		static TileEnum[] blockTypes = {Grass,Field,Woods};

		static internal void AddRandomBlock( PlayMap map ) {

			int wide = rand.Next( map.Wide/2) + map.Wide/5;
			int tall = rand.Next( map.Tall/2) + map.Tall/5;

			int cx = rand.Next( map.Wide );
			int cy = rand.Next( map.Tall );

			int sx = cx - wide/2;
			int sy = cy - tall/2;

			TileEnum type = blockTypes[ rand.Next(blockTypes.Length) ];
			for (int dx=0;dx<wide;dx++) {
				for (int dy=0;dy<tall;dy++) {
					int dw = sx + dx;
					int dt = sy + dy;
					if (dw>=0 && dw<map.Wide && dt>=0 && dt<map.Tall) {
						map.Grid[dw,dt] = new PlayTile( type );
					}
				}
			}
		}

		static internal void AddRiver( PlayMap map ) {

			int tall = 2 + rand.Next( map.Tall-4 );

			for (int dx=0;dx<map.Wide;dx++) {
				map.Grid[ dx, tall ] = new PlayTile( River );
			}

		}

		static internal Where AddRoadWithWell( PlayMap map ) {

			int wide = 2 + rand.Next( map.Wide-4 );
			int tall = map.Tall / 2;

			// move well if river is in the way
			if (map.Grid[wide,tall].Type == River ) tall++;

			for (int dy=0;dy<map.Tall;dy++) {
				TileEnum work = Road;
				if (map.Grid[wide,dy].Type == River ) work = Bridge;
				map.Grid[ wide, dy ] = new PlayTile( work );
			}

			map.Grid[ wide, tall ] = new PlayTile( Well );

			return new Where( wide, tall );

		}
		
		// city type with emphasized ratios.
		static TileEnum[] cityTypes = {Cottage,Cottage,Cottage,Estate,Road,Road,Street };

		static internal void AddSomeCity( PlayMap map, Where center ) {

			for (int count=0;count<50;count++) {

				int dx = rand.Next(7) - 3;
				int dy = rand.Next(7) - 3;
				if (Abs(dx)==3 && Abs(dy)==3) continue;

				int dw = center.X + dx;
				int dt = center.Y + dy;
				if (dw<0 || dt<0 || dw>=map.Wide || dt>=map.Tall) continue;

				PlayTile tile = map.Grid[dw,dt];
				TileEnum type = tile.Type;
				if (type!=Grass && type!=Field && type!=Woods) continue;

				TileEnum work = cityTypes[rand.Next(cityTypes.Length)];
				map.Grid[dw,dt] = new PlayTile( work );

			}
		}

		static internal void AddSpark( PlayMap map ) {

			while (true) {

				int dw = rand.Next( map.Wide );
				int dt = rand.Next( map.Tall );
				var tile = map.Grid[dw,dt];
				if (tile.Type==River || tile.Type==Well) continue;

				map.AddSpark(dw, dt);
				return;

			}
		}

		//static internal void AddSpark( PlayMap map, int dw, int dt ) {

		//	var heat = ModelSettings.STARTING_FLAME * ModelSettings.MAX_FLAME_HEAT;

		//	var tile = map.Grid[dw,dt];

		//	//tile.Flame = ModelSettings.STARTING_FLAME;
		//	//tile.Heat = heat;
		//	//tile.Water = 0;

		//	//tile = map.GetTile(dw+1,dt);
		//	//if (tile!=null) tile.Heat = heat/2;
		//	//tile = map.GetTile(dw-1,dt);
		//	//if (tile!=null) tile.Heat = heat/2;
		//	//tile = map.GetTile(dw,dt+1);
		//	//if (tile!=null) tile.Heat = heat/2;
		//	//tile = map.GetTile(dw,dt-1);
		//	//if (tile!=null) tile.Heat = heat/2;


		//	tile.Engagement = 1f;
		//	tile.Flame = 1;
		//}
	}
}
