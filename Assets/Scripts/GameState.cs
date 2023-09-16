using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that keeps track of game state elements
public class GameState : MonoBehaviour, IGameStatePlayerView
{
    [field: SerializeField]
    public CardDeck cardDeck {get; set;}
    [field: SerializeField]
    public Player[] Players {get; set;}
    [field: SerializeField]
    public Territory[] territories {get; set;}

    [field: SerializeField]
    public int currentPlayerNo{get; private set;}
    [field: SerializeField]
    public TurnStage turnStage {get; set;}
    public UIManager uiManager {get; set;}

    public static readonly int[] ContinentValues = {2, 3, 7, 5, 5, 2};
    public static int[] ContinentCount = {0, 0, 0, 0, 0, 0};

    public bool allTerritoriesClaimed {get; set;} = false;

    public int cardSetRewardStage {get; set;} = 0;

    public void Setup(PlayerData[] playerData)
    {
        uiManager = gameObject.GetComponent<UIManager>();
        cardDeck = gameObject.GetComponent<CardDeck>();
        SetupPlayers(playerData);
        SetupTerritories();
        cardDeck.SetupDeck();
    }
    public void Setup()
    {
        PlayerData[] players = new PlayerData[2];
        players[0] = new PlayerData("Vince", PlayerType.Human, ColorPreset.Green);
        players[1] = new PlayerData("JohnGPT", PlayerType.PassiveAI, ColorPreset.Pink);
        uiManager = gameObject.GetComponent<UIManager>();
        cardDeck = gameObject.GetComponent<CardDeck>();
        SetupPlayers(players);
        SetupTerritories();
        cardDeck.SetupDeck();
    }
    public void SetupPlayers(PlayerData[] players)
    {
        Players = new Player[players.Length];
        GameObject playerObject, playerObjectInstance;
        for(int i = 0; i < players.Length; i++)
        {
            switch(players[i].playerType){
                case PlayerType.Human:
                    playerObject = (GameObject)Resources.Load("prefabs/HumanPlayer", typeof(GameObject));
                    playerObjectInstance = Instantiate(playerObject, new Vector3(), Quaternion.identity);
                    Players[i] = playerObjectInstance.GetComponent<HumanPlayer>();
                    break;
                case PlayerType.PassiveAI:
                    playerObject = (GameObject)Resources.Load("prefabs/PassiveAIPlayer", typeof(GameObject));
                    playerObjectInstance = Instantiate(playerObject, new Vector3(), Quaternion.identity);
                    Players[i] = playerObjectInstance.GetComponent<PassiveAIPlayer>();
                    break;
                default:
                    break;
            }
            Players[i].Setup(this, players[i]);
        }
        currentPlayerNo = 0;
        turnStage = TurnStage.Setup;
    }

    public void SetupTerritories()
    {
        GameObject[] territoryObjects = GameObject.FindGameObjectsWithTag("Territory");
        territories = new Territory[territoryObjects.Length];
        for(int i = 0; i < territories.Length; i++)
        {
            territories[i] = territoryObjects[i].GetComponent<Territory>();
            territories[i].Setup();

            territories[i].SetOwner(null);
            territories[i].TroopCount = 0;

            ContinentCount[(int) territories[i].Continent]++;
        }
        
    }

    // Function called by the player scripts when they end their turn.
    public void EndTurn()
    {
        currentPlayerNo++;
        if(currentPlayerNo >= Players.Length) currentPlayerNo = 0;
        if(turnStage == TurnStage.Setup)
        {
            if(!allTerritoriesClaimed)
            {
                bool endOfClaimingPhase = true;
                foreach(Territory t in territories)
                {
                    if(t.Owner == null)
                    {
                        endOfClaimingPhase = false;
                        break;
                    }
                }
                if(endOfClaimingPhase)
                {
                    allTerritoriesClaimed = true;
                }
            }
        }
        else
        {
            turnStage = TurnStage.Deploy;
        }
        Players[currentPlayerNo].StartTurn();
    }

    public void OnPlayerLoss(Player player)
    {
        if(player.GetOwnedTerritories().Count != 0) return;
        Debug.Log(player.GetData().playerName + " has lost!");
        if(player.IsMyTurn())
        {
            player.EndTurn();
        }
        List<Player> newPlayers = new();
        int loserNo = 0;
        for(int i = 0; i < Players.Length; i++)
        {
            if(Players[i] != player)
            {
                newPlayers.Add(Players[i]);
            }
            else
            {
                loserNo = i;
            }
        }
        Players = newPlayers.ToArray();
        if(currentPlayerNo >= loserNo)
        {
            currentPlayerNo--;
        }
        if(Players.Length == 1)
        {
            Debug.Log(Players[0].GetData().playerName + " Has won!");
        }
    }
    
    // This says "0 references", but it's used by the UI button to end stage! Don't delete!
    public void EndTurnStage()
    {
        CurrentPlayer().EndTurnStage();
    }
    public Player CurrentPlayer()
    {
        return Players[currentPlayerNo];
    }
}
