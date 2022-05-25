using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
	public enum UnitEnum {

		None,

		Bucket,		// human bucket brigade, transfers water from well or river

		Hydra,			// river serpent, spews water
		Djini,			// blows outwards, can blow back on fire
		Golem,			// destroys terrain, leaves road behind as fire break
		Salamander,		// burns current tile, leaving no fuel
	}
}