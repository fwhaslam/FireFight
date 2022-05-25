using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {

	public class PlayMap {

		public PlayMap(int wide,int tall) {
			this.Wide = wide;
			this.Tall = tall;
			this.Grid = new PlayTile[wide,tall];
		}

		public int Turn { get; set; } = 0;

		public int Wide { get; internal set; }

		public int Tall { get; internal set; }

		public PlayTile[,] Grid { get; set; }

		public bool IsInBounds(Where spot) {
			return IsInBounds(spot.X,spot.Y);
		}
		public bool IsInBounds( int x, int y ) {
			return ( x>=0 && y>=0 && x<Wide && y<Tall);
		}

		public PlayTile GetTile(int x,int y) {
			if (!IsInBounds(x,y)) return null;
			return Grid[x,y];
		}

		public void AddSpark( int x, int y ) {

			var tile = Grid[x,y];
			
			//tile.Flame = ModelSettings.STARTING_FLAME;
			//tile.Heat = heat;
			//tile.Water = 0;

			//tile = map.GetTile(dw+1,dt);
			//if (tile!=null) tile.Heat = heat/2;
			//tile = map.GetTile(dw-1,dt);
			//if (tile!=null) tile.Heat = heat/2;
			//tile = map.GetTile(dw,dt+1);
			//if (tile!=null) tile.Heat = heat/2;
			//tile = map.GetTile(dw,dt-1);
			//if (tile!=null) tile.Heat = heat/2;


			tile.Engagement = 1f;
			tile.Flame = 1;
		}

	}
}
