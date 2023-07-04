# white-label-login-example
A small example on white-label-login


Small improvements on the example from loot locker: 

- Simple form validation
- If response has an error, display error to user (e.g email already)
- Removed nickname from account creation as I couldn't figure out how to check for unique name nicely. 
- Nickname prompt on first login (forced)
- Added a logout button 


Draft for user installation instructions so anyone can plug and play LootLockers auth + leaderboard quickly
--- 

- Right click project window in unity, import package > custom package 
- Visit this link and add lootlocker SDK to your assets https://assetstore.unity.com/packages/tools/utilities/lootlocker-sdk-233183
- Window > package manager > paackages > my assets > Loot locker sdk > download > import 
- Edit > project settings > player > other settings > active input handling > both 
- Login to your loot locker console in the browser https://console.lootlocker.com/ > select your game in the rop right > settings on the left > api keys > copy domain key > back in unity > edit > project settings> lootlocker sdk > paste in your domain > go back to the browser and copy the game api key > paste that in as well
- File > build settings > find LootLockerLoginTemplate folder in your project > scenes > drag WhitelabelAndGuestLogin to the top of scenes in build on the build settings window > drag the example main menu and leaderboard screen if you want that as well 
- Go back to the conosole in the browser > Systems on the left hand menu > leaderboards > create leaderboard > fill in the options, making sure to fill in the Key box > copy the key to clipboard > enable game api writes > save 
- Open the scripts folder > open leaderboardScript.cs > change the private string leaderboardKey to the key you copied
- If you're using the example main menu you can skip this but if you're using your own game main menu, update the scene name of your main menu. This scene is whats loaded when someone presses the back button on the leaderboard screen. 
- Open submitleaderboardscore.cs and update the leaderboard key there as well. 
- Open MainMenuScript.cs > update private strings at the top, setting playGameSceneName to the name of the scene of your game

--- 

If you're using the example menus you should be all set up, all thats left to do is let the player upload their score to the leaderboard. 

Where in the code you do this and how is up to you but below is the steps I took to do it in my game. 
--- 

- In the game over screen I have a script, in the start function of that script I do a quick call to PlayerPrefs.GetInt("highScore") to get the highscore that we got earlier for the user. 
- I have a simple if statement to check if the score they just got was higher than their previous high score
- If it is then I call the static function       SubmitLeaderboardScore.Submit(score); and pass it the new score. This will update the leaderboard value for the user. 
- I also update the PlayerPrefs.SetInt("highScore") 

--- 

We're done!

IF NOT USING EXAMPLE MAIN MENU 
- In the start function of a script on your main menu (screen thats shown after the login) add the following function call to get the users current high score : SubmitLeaderboardScore.GetPlayerHighScore(); 
- This saves the users high score to a PlayerPref Int called "highScore" and can be retrieved by calling PlayerPrefs.GetInt("highScore");  this will return 0 if the player has no high score
- This can be changed in the SubmitLeaderboardScore.cs script

		
WARNINGS
This system isn’t very secure since we’re submitting the scores directly from the game, this can lead to people learning how to submit fake scores. To avoid this it’s best to only submit scores from a server: https://ref.lootlocker.com/server-api/#leaderboards