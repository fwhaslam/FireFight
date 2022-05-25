using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {

	public class Where {

		public Where() {
			this.X = 0;
			this.Y = 0;
		}
		public Where(int x,int y) {
			this.X = x;
			this.Y = y;
		}

		override public string ToString() {
			return "Where["+X+","+Y+"]";
		}

		public int X { get; set; }

		public int Y { get; set; }

		public Where Plus( Where delta ) {
			return new Where( X+delta.X, Y+delta.Y );
		}
	}
}
