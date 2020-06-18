using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractClass : MonoBehaviour
{
    /*Use sbyte if value is between -128 and +127 */
    protected const sbyte DEFAULT_BOARD_WIDTH = 8;
    protected const sbyte DEFAULT_BOARD_HEIGHT = 8;
    protected const sbyte DROP_LERP_SPEED = 12;
    protected const sbyte SCORE_CONSTANT = 5;
    protected const sbyte DROP_ROTATE_SLIDE_DISTANCE = 5;
    /* Float constants */
    protected const float DROP_DISTANCE = 1.2f;
    protected const float SPRITE_RADIUS = 0.6f;
    protected const float BOARD_BOTTOM = -4.3f;
    protected const float DROP_LERP_STOP_DISTANCE = 0.05f;
    protected const float DROP_ROTATE_THRESHOLD = 0.05f;
    protected const float DELAY_TO_PRODUCE_DROP = 0.025f;

    protected readonly Vector2 DROP_START_POSITION = new Vector3(0f, 6f, 0f);
    protected readonly Vector2 TILE_START_POSITION = new Vector3(-2f, -6f, 0f);
    

}
