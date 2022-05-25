using Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms {

	/// <summary>
	/// 
	/// Select Points in a rectangular area that are organzied as Blue Noise.
	/// Specify the area and the number of points you would like to have placed.
	/// 
	/// N = Number of points
	/// R = inner radius of annulus ( outer is R2 = 2 * R )
	/// V = Volume = Width * Height
	/// 
	/// Max(N) = V / R^2
	/// Min(N) = V / (2R)^2
	/// 
	/// R = Root( V / MAx(N) )
	/// 
	/// https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
	/// https://medium.com/@hemalatha.psna/implementation-of-poisson-disc-sampling-in-javascript-17665e406ce1
	/// 
	/// </summary>
	public class BlueNoisePoints {

		internal Random rand = new Random();

		internal readonly int MAX_TRIES = 30;

		public BlueNoisePoints(int wide, int tall, int num ) {
			this.Wide = wide;
			this.Tall = tall;
			this.Num = num;
			this.Points = new List<Where>();
		}

		public int Wide {  get; internal set; }
		public int Tall {  get; internal set; }

		public int Num { get; internal set; }

		public List<Where> Points { get; internal set; }


		/// <summary>
		/// TODO: reqrite using R-Grid to reduce cross checking
		/// </summary>
		/// <returns></returns>
		public List<Where> Build() {
			
			float V = Wide * Tall;
			var R = Math.Sqrt( V / Num );
			var R2 = 2 * R;

			List<Where> active = new List<Where>();

			// annulus radius
			var rmin = R * R;
			var rmax = R2 * R2;
			int range = (int)Math.Ceiling( R2 );
			int range2 = 2 * range;

			// first point
			var pt = PickPtInArea( 0,0,Wide,Tall );
			active.Add( pt );
			Points.Add( pt );

			var cycles = 2 * Num - 1;
			for (int cx=0;cx<cycles && active.Count>0;cx++) {

				var px = rand.Next(active.Count);
				var pick = active[ px ];

				bool found = false;
				for (int tries=0;tries<MAX_TRIES;tries++) {

					var delta = PickPtInArea( -range, -range, range2, range2 );
					int pickDistance = delta.X*delta.X + delta.Y*delta.Y;
					if (pickDistance>rmax) continue;	// outside of annulus

					var candidate = pick.Plus(delta);
					if (!IsInBounds(candidate)) continue;

					bool fail = false;
					foreach ( var check in Points) {
						var dx = check.X - candidate.X;
						var dy = check.Y - candidate.Y;
						int distance2 = dx*dx + dy*dy;
						if (distance2<rmin) {		// too close
							fail = true;
							break;
						}
					}
					if (fail) continue;

					found = true;
					active.Add( candidate );
					Points.Add( candidate );
					break;
				}

				if (!found) active.RemoveAt( px );
			}

			return Points;
		}



		internal Where PickPtInArea( int x, int y, int dx, int dy ) {
			return new Where(  x + rand.Next(dx), y + rand.Next(dy) );
		}
		
		internal bool IsInBounds( Where pt ) {
			return (pt.X>=0 && pt.Y>=0 && pt.X<Wide && pt.Y<Tall);
		}

	}
}
