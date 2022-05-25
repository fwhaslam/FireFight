using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
	public class ModelSettings {

		// maximum heat produced by flames
		static public readonly float MAX_FLAME_HEAT = 6f;

		static public readonly float APPROACH_MAX_HEAT_RATE = 0.5f;

		static public readonly int STARTING_FLAME = 1;

		static public readonly float STARTING_HEAT = STARTING_FLAME * MAX_FLAME_HEAT * APPROACH_MAX_HEAT_RATE;

		static public readonly float FUEL_MULTIPLIER = 1.5f;
	}
}
