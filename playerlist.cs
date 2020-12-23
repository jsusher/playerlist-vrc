using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
/* 
 *  Want to check if this client is world owner?
 *  if(Networking.IsMaster)
 *  
 *  Want to see if a specific player is the world owner?
 *  VRCPlayerApi player;
 *  if(player.isMaster)
 *  
 *  VRCPlayerApi.GetPlayers()
 *  returns an array of players not sorted by player id
 *  player ids are assigned based on the order they joined
 *  if a player rejoins a world they will get a new player id instead of their old one
 *  so a lobby of twelve people could have player ids in the thousands...
 *  Which is why foreach loops are better than for loops.
 *  For loops only work on playerList, and their index is not in anyway related to their index in playerList
 *  You must check a playerList player's Id to see if it's equal to the playerId you're looking for.
 */

public class PlayerList : UdonSharpBehaviour
{
    public VRCPlayerApi[] playerList = { null, null, null, null, null, null, null, null, null, null, null, null };
    public VRCPlayerApi[] temp = { null, null, null, null, null, null, null, null, null, null, null, null }; //for use when sorting playerList
    

    //OnPlayerJoined and OnPlayerLeft are called by VRChat or Unity Networking when these things happen.
    public override void OnPlayerJoined(VRCPlayerApi player) //method is passed the joining player
    {
        UpdatePlayerList();
        SortPlayerList();
        DisplayPlayerList();
    }
    public override void OnPlayerLeft(VRCPlayerApi player) //method is passed the leaving player
    {
        UpdatePlayerList();
        SortPlayerList();
        DisplayPlayerList();
    }

    //Called by Onplayer(Join/Left)
    public void UpdatePlayerList()
    {
        VRCPlayerApi.GetPlayers(playerList); //gets the updated list of players from VRCPlayer API, and stores it in playerList.
    }

    //Called by Onplayer(Join/Left)
    public void SortPlayerList()
    {
        //order players from lowest playerId to highest playerId
        foreach (VRCPlayerApi p in playerList) //for each player in playerlist
        {
            if (p != null) //if a player is in this element
            {
                int space = 0;
                for (int i = 0; i < 12; i++) //loop through all other possible players
                {
                    if(playerList[i] != null && p.playerId > playerList[i].playerId) //if this player's id is larger than the player at index i on player list
                    {
                        space++; //increase the index we will put this player into the temp array
                    }
                }
                temp[space] = p; //put the player into the right space in playerList
            }
        }
        playerList = temp; //assign the sorted playerlist to the real playerList
    }

    public UnityEngine.UI.Text playerListDisplay; //a reference to a UI Text to display the player List
    //Called by Onplayer(Join/Left)
    public void DisplayPlayerList()
    {
        playerListDisplay.text = ""; //clears the display

        foreach (VRCPlayerApi player in playerList) //loops through all players in the playerlist
        {
            if(player != null) //if there is a player in this element of the array
            {
                playerListDisplay.text += player.displayName + "'s Id is " + player.playerId + ", and playerList space is "; //add players id, a space, and their name

                for(int i = 0; i < 12; i++) //loop through playerlist to find the playerList index of this player
                {
                    if(playerList[i] != null && playerList[i].playerId == player.playerId) //if this is a player and it is the player we are looking at in the foreach loop
                    {
                        playerListDisplay.text += i.ToString(); //add i to the display to show the index on playerList where this player is stored.
                    }
                }

                if(player.isMaster) //if this player is the world owner
                {
                    playerListDisplay.text += " is world owner."; //display that they are the world owner.
                }
                playerListDisplay.text += "\n"; //add a new line
            }
        }
    }
}
