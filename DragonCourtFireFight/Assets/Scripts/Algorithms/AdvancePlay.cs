using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Model;

namespace Algorithms {


	/// <summary>
	/// This time focus on ROS ( rate of spread ).
	/// </summary>
	public class AdvancePlay_v4 : AdvancePlayIf {

		PlayMap map;

		public AdvancePlay_v4(PlayMap map) {
			this.map = map;
		}

		public void Next() {

			var turn = ( map.Turn += 1 );
			Debug.Log( "ADVANCING GAME STATE = "+(++turn) );

			AdjustDamage();
			SpreadEngagement();
			FixFlame();


			Debug.Log( "ADVANCING GAME STATE [Done]" );
		}

		/// <summary>
		/// Flame creates damage
		/// </summary>
		/// <param name="map"></param>
		internal void AdjustDamage() { 
		
			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];

				var dmg = tile.Engagement;
				if (dmg<=0f) continue;

				var fuel = tile.Fuel - tile.Burnt;
				if (dmg>fuel) dmg = fuel;					// damage capped by fuel
				tile.Burnt += dmg;
			}
		}


		static readonly Where[] ORTH_STEPS = { new Where(0,-1), new Where(0,+1), new Where(-1,0), new Where(+1,0) };

		static readonly Where[] DIAG_STEPS = { new Where(-1,-1), new Where(-1,+1), new Where(+1,+1), new Where(+1,-1) };


		/// <summary>
		///  SpreadEngagement to Temp, then merge Temp to Engagement.
		/// </summary>
		internal void SpreadEngagement() {

			// clear temp
			for (int dw=0;dw<map.Wide;dw++) for (int dt=0;dt<map.Tall;dt++) {
				var tile = map.Grid[dw,dt];
				tile.Temp = 0f;
			}

			var base_spread = 1f;

			// engage to temp
			for (int dw=0;dw<map.Wide;dw++) for (int dt=0;dt<map.Tall;dt++) {

				var tile = map.Grid[dw,dt];

				var fire = tile.Engagement;
				if ( fire == 0f ) continue;

				var spread = tile.Attributes.Spread;
				var advance = base_spread * spread;
				var heat = tile.Attributes.Heat;

				// fix self
				if ( fire + advance >= 1f ) {
					tile.Temp = 1f;
				}

				// orthoganol overlap
				var overlap = tile.Engagement + ( advance * 1f ) - 1f;
				overlap = overlap / spread;		// normalize

				if (overlap>0f) foreach (var delta in ORTH_STEPS ) {
					var work = map.GetTile(dw+delta.X,dt+delta.Y);
					if (work==null || heat<work.Attributes.Ignite) continue;
					work.Temp = Merge(work.Temp,overlap*work.Attributes.Spread);
				}
					
				// diagonal overlap
				overlap = tile.Engagement + ( advance * 0.33f ) - 1f;
				overlap = overlap / spread;		// normalize
				if (overlap>0f) foreach (var delta in DIAG_STEPS ) {
					var work = map.GetTile(dw+delta.X,dt+delta.Y);
					if (work==null || heat<work.Attributes.Ignite) continue;
					work.Temp = Merge(work.Temp,overlap*work.Attributes.Spread);
				}
			}

			// merge temo to engagement.
			for (int dw=0;dw<map.Wide;dw++) for (int dt=0;dt<map.Tall;dt++) {
				var tile = map.Grid[dw,dt];
				tile.Engagement = Merge( tile.Engagement, tile.Temp );
			}

		}

		/// <summary>
		/// Heat creates flame.
		/// </summary>
		/// <param name="map"></param>
		internal void FixFlame() { 

			for (int ix=0;ix<map.Wide;ix++) for (int iy=0;iy<map.Tall;iy++) {

				var tile = map.Grid[ix,iy];

				var fire = tile.Engagement;
				var fuel = tile.Fuel - tile.Burnt;
				if (fire>fuel) tile.Engagement = fire = fuel;	// cap fire/engagement at fuel

				if (fire<=0) tile.Flame = 0;
				else if (fire<0.33f) tile.Flame = 1;
				else if (fire<0.67f) tile.Flame = 2;
				else tile.Flame = 3;
			}	
		}

		/// <summary>
		/// Combine multiple engagement values with a vague effort to remove overlap.
		/// </summary>
		internal float Merge( float a, float b) {

			if (a>1f) a = 1f;
			if (b>1f) b = 1f;

			return a + b - a*b;

		}
	}
}
