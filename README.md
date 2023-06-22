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
	-  File > build settings > open scene folder in project, drag the scenes you want into “scenes in build” in the build settings. Put the login at the top so it appears before the game
	- Edit > project settings > Lootlocker SDK > input your api key and domain key 
	- Open the scripts folder > open WhiteLabelManager.cs and update the private string gameSceneName at the top with the name of your main menu scene, or leave it as to use the default menu
		- If using the default menu open the scripts folder and find MainMenuScript > update the playGameSceneName string (can also update loginSceneName and leaderboardSceneName if you're not using the default scenes.)
	- To get the leaderboard to work 
		- Enable Game API writes in the loot locker web console for your leaderboard (https://console.lootlocker.com/leaderboards > edit > enable game API writes) 
		- Open scripts folder and find the LeaderboardScipt  Script > updated the strings at the top with your main menu scene name and loot locker leaderboard key (leaderboard key can be found at https://console.lootlocker.com/leaderboards > click edit on the leaderboard you want to use > key) 
		- Open SubmitLeaderboardScore script > update leaderboardKey string > add the script to a game object in the scene you want to submit the score > anywhere in your code write SubmitLeaderboardScore.Submit(score) and it should submit the score
		
WARNINGS
This system isn’t very secure since we’re submitting the scores directly from the game, this can lead to people learning how to submit fake scores. To avoid this it’s best to only submit scores from a server: https://ref.lootlocker.com/server-api/#leaderboards