using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Model;

public class TileEnumToSprite : MonoBehaviour {
	
	public Sprite Field;
	public Sprite Grass;
	public Sprite Shrubs;
	public Sprite Woods;
	public Sprite Forest;
	public Sprite Stone;

	public Sprite Cottage;
	public Sprite Estate;
	public Sprite Wall;
	public Sprite Well;

	public Sprite River;
	public Sprite Bridge;
	public Sprite Road;
	public Sprite Street;

	public Sprite Get( TileEnum tile ) {
		switch (tile) {

			case TileEnum.Field: return Field;
			case TileEnum.Grass: return Grass;
			case TileEnum.Shrubs: return Shrubs;
			case TileEnum.Woods: return Woods;
			case TileEnum.Forest: return Forest;
			case TileEnum.Stone: return Stone;

			case TileEnum.Cottage: return Cottage;
			case TileEnum.Estate: return Estate;
			case TileEnum.Wall: return Wall;
			case TileEnum.Well: return Well;

			case TileEnum.River: return River;
			case TileEnum.Bridge: return Bridge;
			case TileEnum.Road: return Road;
			case TileEnum.Street: return Street;

			default: return null;
		}
	}
}
