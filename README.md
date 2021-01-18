# Tetris-Clone-Game

### About the Project
This project is a 2D Tetris game built in Unity using C# and ASP.NET.  
The GUI consists of three scenes: the game board interface, a main menu, and game over screen.  

### Controls
* Left arrow: Move a tetromino one space left
* Right arrow: Move a tetromino one space right
* Down arrow: Move a tetromino one space down
* Up arrow: Rotate a tetromino 90°
* Spacebar: Hold a tetromino
  
### Features 
* Preview: A preview of the next three tetrominos is displayed on screen.
* Sound: Sound effects for various tetromino movements and line clearing, as well as background music are provided.
* Score: The player's current score is showned at the top of the game board. More points are awarded if lines are cleared and tetrominos are placed fast. 
* Levels: The player may choose the level on the main menu, the higher the level, the faster the tetrominos fall, the more points are gained for each tetromino landed. The game also increases in difficulty as the user plays.
* Hold: The player may choose to hold/save a tetromino. However, they can only swap in/out a tetromino a maximum of once per piece.

### Demo
  
Note: The executable file TetrisClone.exe is found in src > Builds.  
  
#### Part 1: Game Start  
![Game Start](images/GameStart.gif)
  
#### Part 2: Holding a Tetromino
![Hold](images/Hold.gif)
  
#### Part 3: Swap a Tetromino and Level Up
Every 10 lines cleared leads to leveling up and increasing the speed of the fall of tetrominos.  
![Swap and Level Up](images/SwapAndLevelUp.gif)
  
#### Part 4: Game Over and Play Again
![Game Over and Play Again](images/GameOverAndPlayAgain.gif)

#### Part 5: Choosing Level 10
There is a noticeable difference in speed between level 1 and level 10
![Level 10](images/Level10.gif)
