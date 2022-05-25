using Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms {

	public class FillFromPoints {

		internal readonly Random rand = new Random();

		internal readonly int EMPTY_VALUE = -1;

		public FillFromPoints( int wide, int tall, List<Where> pts ) {

			this.Wide = wide;
			this.Tall = tall;
			this.Points = pts;

			Grid = new int[wide,tall];
		}

		public int Wide { get; internal set; }

		public int Tall { get; internal set; }

		public List<Where> Points {  get; internal set; }

		public int[,] Grid { get; internal set; }
		
		/// <summary>
		/// Take the index of each point ( starting with zero ), and fill in the map randomly.
		/// </summary>
		public int[,] Build() {

			List<Where> edge = new List<Where>();

			// initialize grid and points in grid
			for (int ix=0;ix<Wide;ix++) for (int iy=0;iy<Tall;iy++) Grid[ix,iy] = EMPTY_VALUE;

			for (int index=0;index<Points.Count;index++) {
				var pt = Points[index];
				Grid[pt.X,pt.Y] = index;
				edge.Add( pt );
			}

			// adjacencies for copying
			List<Where> adj = new List<Where>() { new Where(0,1), new Where(0,-1), new Where(1,0), new Where(-1,0) };

			// randomly expand numbered regions
			while (edge.Count>0) {

				// shuffle lists
				var redge = edge.OrderBy(item => rand.Next()).ToList( );
				var radj = adj.OrderBy( item => rand.Next() ).ToList( );

				foreach (var pt in redge) {

					// value to copy
					int index = Grid[pt.X,pt.Y];
					
					// find one empty adjacent
					bool found = false;
					foreach (var step in radj) {
						var next = pt.Plus(step);
						if (IsInBounds(next) && Grid[next.X,next.Y]==EMPTY_VALUE) { 
							Grid[next.X,next.Y] = index;
							edge.Add( next );
							found = true;
							break;
						}
					}

					// nothing adjacent
					if (!found) {
						edge.Remove(pt);
					}
				}
			}

			// resulting grid
			return Grid;
		}

		internal bool IsInBounds(Where pt) {
			return (pt.X>=0 && pt.Y>=0 && pt.X<Wide && pt.Y<Tall);
		}
	}
}
