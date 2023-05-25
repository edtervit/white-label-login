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

 	- copy package 
	-  File > build settings > open scene foled in project, drag the scene into “scenes in build” in the build settings. Put it at the top so it appears before the game
	- Edit > project settings > Lootlocker SDK > input your api key and domain key 
	- Open the scripts folder > open WhiteLabelManager > Find the function PlayGame() > change the “GameScene” to the name of the scene you want to load when the user presses play
	- To get the leaderboard to work 
		- Enable Game API writes in the loot locker web console for your leaderboard (https://console.lootlocker.com/leaderboards > edit > enable game API writes) 
		- Open WhiteLabelManager Script > ctrl/cmd + f > search for leaderboardKey > update this with the key of your leaderboard (leaderboard key can be found at https://console.lootlocker.com/leaderboards > click edit on the leaderboard you want to use > key) 
		- Open SubmitLeaderboardScore script > update leaderboardKey string with the key of your leaderboard > add the script to a game object in the scene you want to submit the score > anywhere in your code write SubmitLeaderboardScore.Submit(score) and it should submit the score
		
WARNINGS
This system isn’t very secure since we’re submitting the scores directly from the game, this can lead to people learning how to submit fake scores. To avoid this it’s best to only submit scores from a server: https://ref.lootlocker.com/server-api/#leaderboards