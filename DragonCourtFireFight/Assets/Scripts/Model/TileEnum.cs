using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
	public enum TileEnum {

		Field,
		Grass,
		Shrubs,
		Woods,
		Forest,			// dense woods
		Stone,

		Cottage,		// wood building
		Estate,			// stone building
		Wall,
		Well,

		River,
		Bridge,			// stone road over river
		Road,			// dirt road
		Street			// cobbled road
	}
}
