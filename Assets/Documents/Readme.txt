Trieris Readme

Timeline

	Previous Team:

		 - Completed code for game logic and game mechanics
		 - Game fully playable with basic UI

	Our Team:

		COMP 3900
		 - Project moved from Java to Unity in C#
		 - First version of the new redesigned UI
		 - Prototype animation

		COMP 4900
		 - Phase manager made for managing animations
		 - New animations added for sinking, capturing, catapults, and all major actions
		 - Final version of redesigned UI
		 - Bug with major consequence solved to allow for better AI behavior
				(ships were healing when they weren't supposed to, making AI ships "dogpile" on ports and not moving)
		 - Added sounds & music
		 
Code

	Major classes:
	
		GameManager
			- Creates and stores references the board, team and AI
		GameLogic
			- Handles execution of each turn and calculates each phase
			- Calculating the result of a phase populates the animation lists for phase manager
			- Checks for victory conditions
		PhaseManger
			- Handles the execution of animation for each phase
			
		Team
			- Represents a single team in the game
			- Holds lists of team's ships
			- Has methods for getting team color, sprites, name, etc
		TrierisAI
			- Responsible for calculating all AI actions and choices
			- Typically one AI attached to a team
		
		Ship
			- Responsible for all ship-related behaviors
			- Calculates movement, ramming, and actions
			
		Board
			- Holds a list of nodes, loads data from text files to populate the board
		Node
			- Responsible for node behavior and information
		Port
			- Responsible for port behavior and information
			
		InputControl
			- Responsible for camera movement, zooming, and other misc controls
		UIControl
			- Responsible for UI functions, also holds reference to currently selected ship
			
		Animation
			- These classes hold information needed to execute an animation
			- Have a resolve method which should be run as a coroutine to run the animation
			
		CombatResolution
			- Holds information to complete the resolution of in instance of combat, included animation and damage
			
Bugs

	- Occasionally a single yellow ship in particular will spin in place
		- Most likely due to a change in the pathfinding when the code was translated over from java
			- Potentially due to different implementation of the NodePath sorting
				- Original Java version used a built in java priority heap
				
Contact Info

	- The team's full contact info in available in the statement of work and other project documentation
	
		Please feel free to contact us for any questions about the code regarding further development or support
	