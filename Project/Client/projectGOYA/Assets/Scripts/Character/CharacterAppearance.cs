   // -----------------------------------------------------------------------------------------
// using classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// -----------------------------------------------------------------------------------------
// player movement class
public class CharacterAppearance : MonoBehaviour
{
    // static public members
    public static CharacterAppearance instance;

    // -----------------------------------------------------------------------------------------
    // public members
    public Transform tf;
    public Vector2 movement;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // The name of the sprite sheet to use
    public string SpriteSheetName;

    // -----------------------------------------------------------------------------------------
    // private members
    private Vector2 previousPosition;

    // The name of the currently loaded sprite sheet
    private string LoadedSpriteSheetName;

    // The dictionary containing all the sliced up sprites in the sprite sheet
    private Dictionary<string, Sprite> spriteSheet;

    private bool bNpc = false;

    // -----------------------------------------------------------------------------------------
    // awake method to initialisation
    void Awake()
    {
        instance = this;
        previousPosition = tf.position;
        //velocity = rb.velocity;
        this.LoadSpriteSheet();
        animator.SetFloat("speed", 0);
        animator.SetInteger("orientation", 4);
        animator.SetBool("sleep",false);
        bNpc = SpriteSheetName.Contains("np");
    }
    // -----------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

    }
    // -----------------------------------------------------------------------------------------
    // fixed update methode
    void FixedUpdate()
    {
        movement.x = tf.position.x - previousPosition.x;
        movement.y = tf.position.y - previousPosition.y;

        previousPosition = tf.position;

        if (bNpc)
        {
            aniNpcUpdate();
        }
        else
        {
            animationUpdate();
        }

    }

        // Runs after the animation has done its work
    private void LateUpdate()
    {
        // Check if the sprite sheet name has changed (possibly manually in the inspector)
        if (this.LoadedSpriteSheetName != this.SpriteSheetName)
        {
            // Load the new sprite sheet
            this.LoadSpriteSheet();
        }

        // Swap out the sprite to be rendered by its name
        // Important: The name of the sprite must be the same!
        this.spriteRenderer.sprite = this.spriteSheet[this.spriteRenderer.sprite.name];
    }

    // -----------------------------------------------------------------------------------------
    // Set the animation parameters
    public void animationUpdate()
    {
        animator.SetFloat("speed", Mathf.Abs(movement.x) + Mathf.Abs(movement.y));
        
        if (movement.y > 0)
            animator.SetInteger("orientation", 0);
        if (movement.y < 0)
            animator.SetInteger("orientation", 4);
        if (movement.x > 0)
            animator.SetInteger("orientation", 6);
        if (movement.x < 0)
            animator.SetInteger("orientation", 2);
        if(movement == Vector2.zero && !animator.GetBool("sleep"))
            animator.SetInteger("orientation", 4);
    }

    public void aniNpcUpdate()
    {
        animator.SetFloat("speed", Mathf.Abs(movement.x) + Mathf.Abs(movement.y));
        
        if (movement.x > 0)
            animator.SetInteger("orientation", 6);
        if (movement.x < 0)
            animator.SetInteger("orientation", 2);
        
        if (movement.y > 0)
            animator.SetInteger("orientation", 0);
        if (movement.y < 0)
            animator.SetInteger("orientation", 4);

    }

    public Vector3 GetDirection()
    {

        switch (spriteRenderer.sprite.name)
        {
            case "spritesheet_0":
            case "spritesheet_1":
            case "spritesheet_2":
                return Vector3.down; 
            case "spritesheet_3":
            case "spritesheet_4":
            case "spritesheet_5":
                return Vector3.left;   
            case "spritesheet_6":
            case "spritesheet_7":
            case "spritesheet_8":
                return Vector3.right;   
            case "spritesheet_9":
            case "spritesheet_10":
            case "spritesheet_11":
                return Vector3.up;
            default:
                return Vector3.down; 
        }
        
    }
    
    

    // -----------------------------------------------------------------------------------------
    // Loads the sprites from a sprite sheet
    private void LoadSpriteSheet()
    {
        // Load the sprites from a sprite sheet file (png). 
        // Note: The file specified must exist in a folder named Resources
        string spritesheetfolder = "Characters/";
        string spritesheetfilepath = spritesheetfolder + this.SpriteSheetName + "/spritesheet";
        var sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);
        if (sprites.Count() == 0)
        {
            spritesheetfilepath = spritesheetfolder + "chara_01/spritesheet";
            sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);
        }

        this.spriteSheet = sprites.ToDictionary(x => x.name, x => x);
        

        // Remember the name of the sprite sheet in case it is changed later
        this.LoadedSpriteSheetName = this.SpriteSheetName;
    }

    public void SetSleep(bool bSleep)
    {
        animator.SetBool("sleep",bSleep);
    }
}
