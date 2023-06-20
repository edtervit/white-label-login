using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using LootLocker.Requests;

public class SubmitLeaderboardScore : MonoBehaviour
{
  public static string leaderboardKey = "crankyGHighscore";

  public static void Submit(int scoreToSubmit)
  {
    //the member id is set when the user logs in or uses guest login, if they have not done either of those then this will be empty and the request will fail.
    // we set it throughout WhiteLabelManager.cs, you can cmd/ctrl + f and look for PlayerPrefs.SetString("LLplayerId"
    string playerId = PlayerPrefs.GetString("LLplayerId");
    LootLockerSDKManager.SubmitScore(playerId, scoreToSubmit, leaderboardKey, (response) =>
    {
      if (response.success)
      {
        Debug.Log("SubmitLeaderboardScore successful");
      }
      else
      {
        Debug.LogError("SubmitLeaderboardScore failed");
        Debug.LogError("Error: " + response.Error);
      }
    });
  }
}
