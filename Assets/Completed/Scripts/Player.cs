using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using System.Collections.Generic;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;				//Number of points to add to player food points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text foodText;						//UI Text to display current player food total.
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
        public int team;                          //player 1 or 2
		
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;							//Used to store player food points total during level.
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.

        private int moveRange = 100;
        private int stepsLeft = 10;
        private Text stepsLeftText;
        private Button endAction;
        public bool myTurn;
        private bool movingPhase;
        private bool attackPhase;

        public GameManager GM;


        /*private bool canMove = false;
        private bool moveSelection = false;
        private bool firstClick = false;
        private List<GameObject> validMoves=new List<GameObject>();
        private List<Vector3> validPositions = new List<Vector3>();*/



        //Start overrides the Start function of MovingObject
        protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			//Get the current food point total stored in GameManager.instance between levels.
			food = GameManager.instance.playerFoodPoints;
			
            stepsLeftText = GameObject.Find("StepsText").GetComponent<Text>();

            endAction = GameObject.Find("EndTurn").GetComponent<Button>();

            GM = GameObject.FindObjectOfType<GameManager>();

            myTurn = false;
            movingPhase = true;
            attackPhase = true;

            if (team == 0)
            {
                this.myTurn = true;
                GM.player0.Add(this);
            }
            else GM.player1.Add(this);

			//Call the Start function of the MovingObject base class.
			base.Start ();
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerFoodPoints = food;
		}
		
		void OnMouseDown()
        {
            if (myTurn)//   &&  (GM.curPlayer==null|| GM.curPlayer == this))
            {
                Debug.Log(this.GetType());
                GM.setCurTeam(team);
                validMoves = showValidTiles(stepsLeft);
                validAttack = showValidAttack();
                if (!moveSelection)
                {
                    Debug.Log("move phase");
                    if (stepsLeft > 0)
                    {
                        
                        canMove = true;
                        firstClick = true;
                    }
                    moveSelection = true;
                }
                else
                    moveSelection = false;
            }
        }

        public void endPlayerTurn()
        {
            Debug.Log("heihei");
            myTurn = false; 
            GM.curTeam = ~team;
        }

        private List<GameObject> showValidTiles(int stepsLeft)
        {
            GameObject[] floors;
            List<GameObject> validFloors=new List<GameObject>();
            floors = GameObject.FindGameObjectsWithTag("Floor");
            int curX = (int)transform.position.x;
            int curY = (int)transform.position.y;

            for (int i = 0; i < floors.Length; i++)
            {
                int fX = (int)floors[i].transform.position.x;
                int fY = (int)floors[i].transform.position.y;
                int xdif = Mathf.Abs(fX - curX);
                int ydif = Mathf.Abs(fY - curY);
                if (xdif + ydif <= stepsLeft && xdif + ydif != 0)
                {
                    
                    Vector3 rayOrigin = new Vector3(10, 10, -100);
                    Vector3 rayDes = new Vector3(fX, fY, 0.1f);
                    Vector3 rayDir = (rayDes - rayOrigin).normalized;
                    Ray ray = new Ray(rayOrigin,rayDir);
                    Debug.Log(rayOrigin + " " + rayDes + " " + Physics.Raycast(ray));
                    if (!Physics.Raycast(ray)) { 
                        SpriteRenderer renderer = floors[i].GetComponent<SpriteRenderer>();
                        renderer.color = Color.green;
                        validFloors.Add(floors[i]);
                        validPositions.Add(floors[i].transform.position);
                    }
                    else
                    {
                        Debug.Log("wall at (" + fX + "," + fY + ")");
                    }
                }

            }
            return validFloors;
        }

        private List<GameObject> showValidAttack()
        {
            List<GameObject> valid = new List<GameObject>();
            int curX = (int)transform.position.x;
            int curY = (int)transform.position.y;
            Vector3 cur = new Vector3(10, 10, -100);
            List<Vector3> list = new List<Vector3>();
            list.Add(new Vector3(curX, curY + 1,0.1f));
            list.Add(new Vector3(curX, curY - 1,0.1f));
            list.Add(new Vector3(curX-1, curY,0.1f));
            list.Add(new Vector3(curX+1, curY,0.1f));
            for (int i = 0; i < list.Count; i++)
            {
                RaycastHit hit;
                Ray ray = new Ray(cur, (list[i] - cur).normalized);
                if (Physics.Raycast(ray,out hit))
                {
                    GameObject target = hit.collider.gameObject;
                   
                    if (target.tag == "Player")
                    {
                        Player current = target.gameObject.GetComponent<Player>();
                        if (this.team != current.team)
                        {
                            valid.Add(hit.collider.gameObject);
                            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
                            sr.color = new Color(0f, 0f, 0f, 1f);
                        }
                    }
                }
            }
            return valid;
        }

        private void resetValidTiles()
        {
            for (int i = 0; i< validMoves.Count; i++){
                SpriteRenderer renderer = validMoves[i].GetComponent<SpriteRenderer>();
                renderer.color = new Color(1f, 1f, 1f, 1f);
            }
            for (int i = 0; i < validAttack.Count; i++)
            {
                SpriteRenderer renderer = validAttack[i].GetComponent<SpriteRenderer>();
                renderer.color = new Color(1f, 1f, 1f, 1f);
            }
            validMoves.Clear();
            validPositions.Clear();
            validAttack.Clear();
        }

		private void Update ()
		{
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn)
                return;
			
			int desX = -100;  	//Used to store the horizontal move direction.
			int desY = -100;		//Used to store the vertical move direction.
			

            if (canMove&&!firstClick)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Pressed left click.");
                    Vector3 posVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    desX = Mathf.RoundToInt(posVec.x);
                    desY = Mathf.RoundToInt(posVec.y);                   
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                firstClick = false;

            }

            //Check if we have a non-zero value for horizontal or vertical
            if (desX != -100 && desY != -100)
            {
                if (!attack(desX,desY)) {
                    Debug.Log("attackfail");
                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                    AttemptMove(desX, desY);
                }
			}
		}
		
        private bool attack(int x,int y)
        {
            bool valid = false;
            GameObject target=null;
            Vector3 des = new Vector3(x, y, 0);

            for (int i = 0; i < validAttack.Count; i++)
            {
                if (des == validAttack[i].transform.position) {
                    valid = true;
                    target = validAttack[i];
                    break;
                }
            }

            if (valid)
            {
                Destroy(target);
                resetValidTiles();
                endPlayerTurn();
            }

            return valid;
        }


		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove (int xDir, int yDir)
		{
            //If Move returns true, meaning Player was able to move into an empty space.
            int stepsTaken= Move(xDir, yDir, validPositions);
            if (stepsTaken > 0)
            {
                stepsLeft -= stepsTaken;
                stepsLeftText.text = "Moves remaining: " + stepsLeft;
            }
            
            resetValidTiles();
            canMove = false;
            moveSelection = false;


			
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			//GameManager.instance.playersTurn = false;
		}
		
		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
			//Set hitWall to equal the component passed in as a parameter.
			Wall hitWall = component as Wall;
			
			//Call the DamageWall function of the Wall we are hitting.
			hitWall.DamageWall (wallDamage);
			
			//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
			animator.SetTrigger ("playerChop");
		}
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("Restart", restartLevelDelay);
				
				//Disable the player object since level is over.
				enabled = false;
			}
			
			//Check if the tag of the trigger collided with is Food.
			else if(other.tag == "Food")
			{
				//Add pointsPerFood to the players current food total.
				food += pointsPerFood;
				
				//Update foodText to represent current total and notify player that they gained points
				foodText.text = "+" + pointsPerFood + " Food: " + food;
				
				//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//Disable the food object the player collided with.
				other.gameObject.SetActive (false);
			}
			
			//Check if the tag of the trigger collided with is Soda.
			else if(other.tag == "Soda")
			{
				//Add pointsPerSoda to players food points total
				food += pointsPerSoda;
				
				//Update foodText to represent current total and notify player that they gained points
				foodText.text = "+" + pointsPerSoda + " Food: " + food;
				
				//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//Disable the soda object the player collided with.
				other.gameObject.SetActive (false);
			}
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game.
			Application.LoadLevel (Application.loadedLevel);
		}
		
		
		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");
			
			//Subtract lost food points from the players total.
			food -= loss;
			
			//Update the food display with the new total.
			foodText.text = "-"+ loss + " Food: " + food;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (food <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}
		}
	}
}

