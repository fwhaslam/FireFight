using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using TMPro;

using Algorithms;
using Model;

public class GameHandlerController : MonoBehaviour
{

    static public PlayMap world;
    static internal AdvancePlayIf advancer;

    static readonly int MAP_WIDE = 24;
    static readonly int MAP_TALL = 15;

    static readonly int TILE_PIXELS = 64;
    static readonly float WORLD_SCALE = 100f;

    // OFF = center of world, shift for all tiles
     static readonly float DRAW_OFFX = ( 1 - MAP_WIDE ) * TILE_PIXELS / WORLD_SCALE / 2;
    static readonly float DRAW_OFFY = ( 1 - MAP_TALL ) * TILE_PIXELS / WORLD_SCALE / 2;

    // NOTE: Tile Pivot is center
    static readonly Vector2 TILE_BASE = new Vector2( -TILE_PIXELS / WORLD_SCALE / 2, -TILE_PIXELS / WORLD_SCALE / 2 );

    // NOTE: Flame Pivot is bottom center
   static readonly Vector2 FLAME_BASE = new Vector2( 0, -TILE_PIXELS / WORLD_SCALE / 2 );


    Object[] flamesPrefab,burntPrefab;

    static public UnityEvent NextTurnEvent = new UnityEvent();

    GameObject peekabooGO;
    TMP_Text whereTMP,flameTMP,heatTMP,typeTMP;

	void Awake() {
		peekabooGO = GameObject.Find("PeekABoo");
        whereTMP = GameObject.Find("PeekABoo/WhereTMP").GetComponent<TMP_Text>();
        flameTMP = GameObject.Find("PeekABoo/FlameTMP").GetComponent<TMP_Text>();
        heatTMP = GameObject.Find("PeekABoo/HeatTMP").GetComponent<TMP_Text>();
        typeTMP = GameObject.Find("PeekABoo/TerrainTMP").GetComponent<TMP_Text>();
	}


	// Start is called before the first frame update
	void Start() {

        Debug.Log("startup!");

        flamesPrefab = new GameObject[3];
        for (int ix=0;ix<3;ix++) flamesPrefab[ix] = Resources.Load( "Fire/Flame"+(1+ix)+"_PF");
        burntPrefab = new GameObject[4];
        for (int ix=0;ix<4;ix++) burntPrefab[ix] = Resources.Load( "Fire/Burnt"+(1+ix)+"_PF");

		world = TileMapFactory.BuildVillage( MAP_WIDE, MAP_TALL );
		//world = TileMapFactory.BuildOneType(MAP_WIDE, MAP_TALL, TileEnum.Field);
		//world = TileMapFactory.BuildTwoType( MAP_WIDE, MAP_TALL, TileEnum.Grass, TileEnum.Woods );
		//world = TileMapFactory.BuildQuarters(MAP_WIDE, MAP_TALL);
		//world = TileMapFactory.BlueNoisePoints( MAP_WIDE, MAP_TALL );
		//world = TileMapFactory.FloodFillMap(MAP_WIDE, MAP_TALL);

		advancer = new AdvancePlay_v4(world);

        BuildWorldTiles();

        NextTurnEvent.AddListener( DoNextTurn );
    }

    // Update is called once per frame
    void Update() {

		if (Input.GetKeyDown("space")) {
            TriggerNextTurn();
        }

        CheckMouseOverForPopup(Input.GetMouseButton(1));
    }


    /// <summary>
    /// Place tiles using center pivot.
    /// </summary>
    void BuildWorldTiles() {

print("Drawing World "+world.Wide+" "+world.Tall);

        var tiles = gameObject.GetComponent<TileEnumToSprite>();
        var display = GameObject.Find("TileDisplay");

        for (int ix=0;ix<world.Wide;ix++) {
            for (int iy=0;iy<world.Tall;iy++) {

                var tile = world.Grid[ix,iy];
                var sprite = tiles.Get( tile.Type );

                // add tile
                GameObject go = new GameObject("Tile_"+ix+"_"+iy);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                go.transform.parent = display.transform;
                go.transform.position = TileWhereAsWorldPoint( ix, iy );
                    //new Vector2( ix * TILE_PIXELS/100f + DRAW_OFFX, iy * TILE_PIXELS/100f + DRAW_OFFY );

			}
		}

        UpdateWorldSprites();
	}

    internal void UpdateWorldSprites() {
        
print("Drawing Sprites "+world.Wide+" "+world.Tall);

        var display = GameObject.Find("SpriteDisplay");

        foreach (Transform child in display.transform) {
             GameObject.Destroy(child.gameObject);
         }

//print("Loaded Prefab = "+flamePF );

        //var offx = ( 1 - world.Wide ) * TILE_PIXELS / 100f / 2;
        //var offy = ( 0 - world.Tall ) * TILE_PIXELS / 100f / 2;

        for (int ix=0;ix<world.Wide;ix++) {
            for (int iy=0;iy<world.Tall;iy++) {

//print("Adding Tile "+ix+" "+iy);
                var tile = world.Grid[ix,iy];

                // add flame
                if (tile.Flame>0) {
Debug.Log("Adding Flame at "+ix+" "+iy+"  ["+tile.Flame+"]");
                    var sprite = flamesPrefab[ tile.Flame-1 ];
                    GameObject gof = Instantiate (sprite) as GameObject;
                    gof.name = "Fire_"+ix+"_"+iy;
                    gof.transform.parent = display.transform;
                    //gof.transform.position = new Vector2( 
                    //    ix * TILE_PIXELS/100f + DRAW_OFFX, 
                    //    iy * TILE_PIXELS/100f + DRAW_OFFX );
                               
                    gof.transform.position = TileWhereAsWorldPoint( ix, iy ) + FLAME_BASE;

				}

                // add burnt
 Debug.Log("TILE fuel="+tile.Fuel+" burnt="+tile.Burnt);
               int showBurnt = (int) ( ( tile.Burnt * 4f + 0.5f ) / tile.Fuel );
                if (showBurnt>0) {
Debug.Log("Adding Burnt at "+ix+" "+iy+"  ["+showBurnt+"]");
                    var sprite = burntPrefab[ showBurnt-1 ];
                    GameObject gof = Instantiate (sprite) as GameObject;
                    gof.name = "Burn_"+ix+"_"+iy;
                    gof.transform.parent = display.transform;
                    gof.transform.position = TileWhereAsWorldPoint( ix, iy );
				}
			}
		}
	}

    public void TriggerNextTurn() {
        NextTurnEvent.Invoke();
	}

    internal void DoNextTurn() {

Debug.Log("DO NEXT TURN");
        advancer.Next();
        UpdateWorldSprites();

        var turnText = GameObject.Find("TurnTXT").GetComponent<TMP_Text>();
        turnText.text = "Turn: "+world.Turn;
	}


    void CheckMouseOverForPopup( bool show ) {

        peekabooGO.SetActive( show );
        if (!show) return;

        Vector3 mousePos = Input.mousePosition;
//Debug.Log("MOUSE POS = "+mousePos);

        mousePos.z = Camera.main.nearClipPlane;
        var point = Camera.main.ScreenToWorldPoint(mousePos);

        Where spot = new Where();
        WorldPointAsTileWhere( point, spot );
//Debug.Log("POPUP TILE = "+spot);

        if (!world.IsInBounds(spot)) {

            peekabooGO.SetActive(false);

        }
        else {

            peekabooGO.SetActive(true);

            var tile = world.Grid[ spot.X, spot.Y ];
            //peekabooGO.transform.position = new Vector2( point.x, point.y );
            peekabooGO.transform.position = new Vector2( mousePos.x, mousePos.y );

            whereTMP.text = "At: "+spot.X+","+spot.Y;
            flameTMP.text = "Flame: "+tile.Flame;
            heatTMP.text = "Heat: "+tile.Heat.ToString("0.00");
            //typeTMP.text = "Here: "+tile.Type.ToString();
            typeTMP.text = "Water: "+tile.Water.ToString("0.00");
        }

	}

    Vector2 TileWhereAsWorldPoint(int x,int y) { 

        return new Vector2( x * TILE_PIXELS/100f + DRAW_OFFX, y * TILE_PIXELS/100f + DRAW_OFFY );

    }

    void WorldPointAsTileWhere(Vector2 pt, Where spot) {
        
        //spot.X = (int) ( ( pt.x - DRAW_OFFX ) * 100f / TILE_PIXELS );
        //spot.Y = (int) ( ( pt.y - DRAW_OFFY ) * 100f / TILE_PIXELS );
        
        spot.X = (int) ( ( pt.x - DRAW_OFFX ) * 100f / TILE_PIXELS + 0.5 );
        spot.Y = (int) ( ( pt.y - DRAW_OFFY ) * 100f / TILE_PIXELS + 0.5 );

	}

}
