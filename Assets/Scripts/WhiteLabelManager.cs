using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using LootLocker.Requests;

public class WhiteLabelManager : MonoBehaviour
{
  // Input fields
  [Header("New User")]
  public TMP_InputField newUserEmailInputField;
  public TMP_InputField newUserPasswordInputField;
  public TMP_InputField nickNameInputField;

  [Header("Existing User")]
  public TMP_InputField existingUserEmailInputField;
  public TMP_InputField existingUserPasswordInputField;
  public CanvasAnimator loginCanvasAnimator;

  [Header("Reset password")]
  public TMP_InputField resetPasswordInputField;

  [Header("RememberMe")]
  // Components for enabling auto login
  public Toggle rememberMeToggle;
  public Animator rememberMeAnimator;
  private int rememberMe;

  [Header("Button animators")]
  public Animator autoLoginButtonAnimator;
  public Animator backButtonAnimator;
  public Animator newUserButtonAnimator;
  public Animator resetPasswordButtonAnimator;


  //create screen
  public Animator createButtonAnimator;
  public Animator createBackButtonAnimator;
  public Animator createPasswordInputFieldAnimator;
  public Animator createEmailInputFieldAnimator;

  //reset screen 
  public Animator resetEmailInputFieldAnimator;
  public Animator resetBackButtonAnimator;
  public CanvasAnimator resetCanvasAnimator;

  //login screen
  public Animator loginEmailInputFieldAnimator;
  public Animator loginPasswordInputFieldAnimator;
  public Animator loginBackButtonAnimator;
  public Animator loginButtonAnimator;
  public Animator loginRememberMeAnimator;

  // error screen
  public Animator errorScreenAnimator;

  //game screen
  public Animator gameLogoutButtonAnimator;
  public CanvasAnimator gameCanvasAnimator;

  //newName screen
  public Animator newNickNameInputFieldAnimator;
  public Animator newNickNameLogOutButtonAnimator;
  public Animator newNickNameCreateButtonAnimator;
  public CanvasAnimator setDisplayNameCanvasAnimator;


  [Header("New Player Name")]
  public TMP_InputField newPlayerNameInputField;


  [Header("Player name")]
  public TextMeshProUGUI playerNameText;
  public Animator playerNameTextAnimator;

  [Header("Error Handling")]
  public TextMeshProUGUI errorText;
  public GameObject errorPanel;


  // Called when pressing "LOGIN" on the login-page
  public void Login()
  {
    string email = existingUserEmailInputField.text;
    string password = existingUserPasswordInputField.text;

    void isError(string error)
    {
      if (error.Contains("message"))
      {
        showErrorMessage(extractMessageFromLootLockerError(error));
      }

      if (!error.Contains("message"))
      {
        showErrorMessage("Error logging in");
      }
      loginButtonAnimator.SetTrigger("Error");
      loginRememberMeAnimator.SetTrigger("Show");
      loginEmailInputFieldAnimator.SetTrigger("Show");
      loginPasswordInputFieldAnimator.SetTrigger("Show");
      loginBackButtonAnimator.SetTrigger("Show");
      return;
    }

    LootLockerSDKManager.WhiteLabelLogin(email, password, Convert.ToBoolean(rememberMe), response =>
    {
      if (!response.success)
      {
        // Error
        isError(response.Error);
        Debug.Log("error while logging in");
        return;
      }
      else
      {
        Debug.Log("Player was logged in succesfully");
      }

      // Is the account verified?
      if (response.VerifiedAt == null)
      {
        // Stop here if you want to require your players to verify their email before continuing
      }

      LootLockerSDKManager.StartWhiteLabelSession((response) =>
          {
            if (!response.success)
            {
              // Error
              // Animate the buttons
              isError(response.Error);
              return;
            }
            else
            {
              // Session was succesfully started;
              // animate the buttons
              loginButtonAnimator.SetTrigger("LoggedIn");
              loginButtonAnimator.SetTrigger("Hide");
              gameLogoutButtonAnimator.SetTrigger("Show");
              Debug.Log("session started successfully");
              // Write the current players name to the screen
              CheckIfPlayerHasName(response.public_uid);
              //   SetPlayerNameToGameScreen();
            }
          });
    });
  }

  //checks if user has set a display name, if not forces them to set one
  public void CheckIfPlayerHasName(string publicUID)
  {
    string playerName;
    LootLockerSDKManager.GetPlayerName((response) =>
    {
      if (response.success)
      {
        playerName = response.name;
        //if the players name is the same as their publicUID, they have not set a display name
        //This could happen if the display name was taken when they created their account
        //since we made it a fallback to set their display name to their publicUID if their name is taken
        if (playerName == "" || playerName.ToLower() == publicUID.ToLower())
        {
          // Player does not have a name, force them to set one
          Debug.Log("Player has not set a display name");
          //show the set display name screen
          setDisplayNameCanvasAnimator.CallAppearOnAllAnimators();
        }
        else
        {
          // Player has a name, continue
          Debug.Log("Player has a name: " + response.name);
          playerName = response.name;
          SetPlayerNameToGameScreen();
        }
      }
    });


  }

  public void updatePlayerName()
  {
    newNickNameCreateButtonAnimator.SetTrigger("UpdateName");
    newNickNameLogOutButtonAnimator.SetTrigger("Hide");
    newNickNameInputFieldAnimator.SetTrigger("Hide");

    string newPlayerName = newPlayerNameInputField.text;
    if (newPlayerName == "")
    {
      showErrorMessage("Please enter a display name");
      return;
    }

    void isError(string error)
    {
      if (error.Contains("message"))
      {
        string message = extractMessageFromLootLockerError(error);
        if (message.Contains("UNIQUE")) showErrorMessage("Display name already taken");
        else
        {
          showErrorMessage(message);
        }
      }

      if (!error.Contains("message"))
      {
        showErrorMessage("Error setting display name");
      }
      newNickNameCreateButtonAnimator.ResetTrigger("UpdateName");
      newNickNameLogOutButtonAnimator.SetTrigger("Show");
      newNickNameInputFieldAnimator.SetTrigger("Show");
      newNickNameCreateButtonAnimator.SetTrigger("Error");

      return;
    }
    // Set the players name
    LootLockerSDKManager.SetPlayerName(newPlayerName, (response) =>
    {
      if (!response.success)
      {
        isError(response.Error);
        return;
      }

      setDisplayNameCanvasAnimator.CallDisappearOnAllAnimators();
      newNickNameCreateButtonAnimator.SetTrigger("Hide");
      // Write the players name to the screen
      SetPlayerNameToGameScreen();
    });
  }

  // Write the players name to the screen
  void SetPlayerNameToGameScreen()
  {
    LootLockerSDKManager.GetPlayerName((response) =>
    {
      if (response.success)
      {
        playerNameTextAnimator.ResetTrigger("Hide");
        playerNameTextAnimator.SetTrigger("Show");
        playerNameText.text = response.name;
      }
    });
  }

  // Show an error message on the screen
  public void showErrorMessage(string message, int showTime = 3)
  {
    //set active
    errorPanel.SetActive(true);
    errorText.text = message.ToUpper();
    errorScreenAnimator.SetTrigger("Show");
    //wait for 3 seconds and hide the error panel
    Invoke("hideErrorMessage", showTime);
  }

  private void hideErrorMessage()
  {
    errorScreenAnimator.SetTrigger("Hide");
  }

  public void logout()
  {
    //remove the auto remember
    PlayerPrefs.SetInt("rememberMe", 0);
    rememberMeToggle.isOn = false;
    rememberMe = 0;

    createButtonAnimator.SetTrigger("Hide");
    createButtonAnimator.ResetTrigger("CreateAccount");
    createButtonAnimator.ResetTrigger("Login");
    createButtonAnimator.ResetTrigger("ResetPassword");

    gameCanvasAnimator.CallDisappearOnAllAnimators();


    existingUserEmailInputField.text = "";
    existingUserPasswordInputField.text = "";


    //end the session
    LootLockerSessionRequest sessionRequest = new LootLockerSessionRequest();
    LootLocker.LootLockerAPIManager.EndSession(sessionRequest, (response) =>
      {
        if (!response.success)
        {
          showErrorMessage("Error logging out");
          return;
        }
        Debug.Log("Logged Out");
      });

  }

  // Called when pressing "CREATE" on new user screen
  public void NewUser()
  {
    string email = newUserEmailInputField.text;
    string password = newUserPasswordInputField.text;
    // string newNickName = nickNameInputField.text;


    if (email.Length < 1 || password.Length < 1)
    {
      showErrorMessage("Please fill in all fields");
      return;
    }

    //if password is shorter than 8 characters display an error
    if (password.Length < 8)
    {
      showErrorMessage("Password must be at least 8 characters long");
      return;
    }

    void isError(string error)
    {
      if (error.Contains("message"))
      {
        showErrorMessage(extractMessageFromLootLockerError(error));
      }

      if (!error.Contains("message"))
      {
        showErrorMessage("Error creating account");
      }
      createButtonAnimator.SetTrigger("Error");

      createBackButtonAnimator.SetTrigger("Show");
      createPasswordInputFieldAnimator.SetTrigger("Show");
      createEmailInputFieldAnimator.SetTrigger("Show");
      return;
    }


    //if passes all above checks, create the account
    Debug.Log("Creating account");
    createButtonAnimator.SetTrigger("CreateAccount");
    createBackButtonAnimator.SetTrigger("Hide");
    createPasswordInputFieldAnimator.SetTrigger("Hide");
    createEmailInputFieldAnimator.SetTrigger("Hide");


    LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
    {
      if (!response.success)
      {
        isError(response.Error);
        return;
      }
      else
      {
        // Succesful response
        // Log in player to set name
        // Login the player
        LootLockerSDKManager.WhiteLabelLogin(email, password, false, response =>
            {
              if (!response.success)
              {
                isError(response.Error);
                return;
              }
              // Start session
              LootLockerSDKManager.StartWhiteLabelSession((response) =>
                  {
                    if (!response.success)
                    {
                      isError(response.Error);
                      return;
                    }
                    string publicUID = response.public_uid;
                    // Set nickname to be public UID 
                    string newNickName = response.public_uid;
                    // Set new nickname for player
                    LootLockerSDKManager.SetPlayerName(newNickName, (response) =>
                        {
                          if (!response.success)
                          {
                            showErrorMessage("Your account was created but your display name was already taken, you'll be asked to set it when you log in.", 5);
                            // Set public UID as name if setting nickname failed
                            LootLockerSDKManager.SetPlayerName(publicUID, (response) =>
                                {
                                  if (!response.success)
                                  {
                                    showErrorMessage("Your account was created but your display name was already taken, you'll be asked to set it when you log in.", 5);
                                  }
                                });
                          }

                          // End this session
                          LootLockerSessionRequest sessionRequest = new LootLockerSessionRequest();
                          LootLocker.LootLockerAPIManager.EndSession(sessionRequest, (response) =>
                            {
                              if (!response.success)
                              {
                                showErrorMessage("Account created but error ending session");
                                return;
                              }
                              Debug.Log("Account Created");
                              newUserButtonAnimator.SetTrigger("AccountCreated");
                              backButtonAnimator.SetTrigger("Show");
                              // New user, turn off remember me
                              rememberMeToggle.isOn = false;
                            });
                        });
                  });
            });
      }
    });
  }

  // Start is called before the first frame update
  public void Start()
  {
    // See if we should log in the player automatically
    rememberMe = PlayerPrefs.GetInt("rememberMe", 0);
    if (rememberMe == 0)
    {
      rememberMeToggle.isOn = false;
    }
    else
    {
      rememberMeToggle.isOn = true;
    }
  }

  // Called when changing the value on the toggle
  public void ToggleRememberMe()
  {
    bool rememberMeBool = rememberMeToggle.isOn;
    rememberMe = Convert.ToInt32(rememberMeBool);

    // Animate button
    if (rememberMeBool == true)
    {
      rememberMeAnimator.SetTrigger("On");
    }
    else
    {
      rememberMeAnimator.SetTrigger("Off");
    }
    PlayerPrefs.SetInt("rememberMe", rememberMe);
  }

  public void AutoLogin()
  {
    // Does the user want to automatically log in?
    if (Convert.ToBoolean(rememberMe) == true)
    {
      Debug.Log("Auto login");
      // Hide the buttons on the login screen
      existingUserEmailInputField.GetComponent<Animator>().ResetTrigger("Show");
      existingUserEmailInputField.GetComponent<Animator>().SetTrigger("Hide");
      existingUserEmailInputField.GetComponent<Animator>().ResetTrigger("Show");
      existingUserPasswordInputField.GetComponent<Animator>().SetTrigger("Hide");
      loginBackButtonAnimator.ResetTrigger("Show");
      loginBackButtonAnimator.SetTrigger("Hide");

      // Start to spin the login button
      loginButtonAnimator.ResetTrigger("Hide");
      loginButtonAnimator.SetTrigger("Hide");
      //   loginButtonAnimator.SetTrigger("Login");

      LootLockerSDKManager.CheckWhiteLabelSession(response =>
      {
        if (response == false)
        {
          // Session was not valid, show error animation
          // and show back button
          loginButtonAnimator.SetTrigger("Error");
          backButtonAnimator.SetTrigger("Show");

          // set the remember me bool to false here, so that the next time the player press login
          // they will get to the login screen
          rememberMeToggle.isOn = false;
        }
        else
        {
          // Session is valid, start game session
          LootLockerSDKManager.StartWhiteLabelSession((response) =>
                {
                  if (response.success)
                  {
                    // It was succeful, log in
                    loginButtonAnimator.SetTrigger("Hide");
                    backButtonAnimator.SetTrigger("Show");
                    // Write the current players name to the screen
                    CheckIfPlayerHasName(response.public_uid);
                    // SetPlayerNameToGameScreen();
                  }
                  else
                  {
                    // Error
                    // Animate the buttons
                    loginButtonAnimator.SetTrigger("Error");
                    backButtonAnimator.SetTrigger("Show");

                    Debug.Log("error starting LootLocker session");
                    // set the remember me bool to false here, so that the next time the player press login
                    // they will get to the login screen
                    rememberMeToggle.isOn = false;

                    return;
                  }

                });

        }

      });
    }
    else if (Convert.ToBoolean(rememberMe) == false)
    {
      Debug.Log("Auto login is off");
      // Continue as usual
      loginCanvasAnimator.CallAppearOnAllAnimators();
      //   loginButtonAnimator.ResetTrigger("Show");
      loginButtonAnimator.SetTrigger("Show");
    }
  }

  public void PasswordReset()
  {
    string email = resetPasswordInputField.text;
    LootLockerSDKManager.WhiteLabelRequestPassword(email, (response) =>
    {
      if (!response.success)
      {
        Debug.Log("error requesting password reset");
        //get the message from the error and dsiplay it 

        if (response.Error.Contains("message"))
        {
          showErrorMessage(extractMessageFromLootLockerError(response.Error));
        }


        if (!response.Error.Contains("message"))
        {
          showErrorMessage("Error requesting password reset");
        }


        resetPasswordButtonAnimator.SetTrigger("Error");

        // make the buttons show again 
        resetBackButtonAnimator.SetTrigger("Show");
        resetEmailInputFieldAnimator.SetTrigger("Show");

        return;
      }

      Debug.Log("requested password reset successfully");
      resetEmailInputFieldAnimator.SetTrigger("Hide");
      resetPasswordButtonAnimator.SetTrigger("Done");
      resetBackButtonAnimator.SetTrigger("Show");
    });
  }

  public void ResendVerificationEmail()
  {
    int playerID = 0;
    LootLockerSDKManager.WhiteLabelRequestVerification(playerID, (response) =>
    {
      if (response.success)
      {
        // Email was sent!
      }
    });
  }

  private string extractMessageFromLootLockerError(string rawError)
  {
    //find in the string "message":" and split the string there
    int first = rawError.IndexOf("\"message\":\"") + "\"message\":\"".Length;
    int last = rawError.LastIndexOf("\"message\":\"");
    // removes "message":" and everything before it from the string
    string str2 = rawError.Substring(first, rawError.Length - first);

    int end = str2.IndexOf("\"");
    // finds the closing " and removes everything after it from the string 
    string res = str2.Substring(0, end);
    res = res.ToUpper();
    return res;
  }
}
