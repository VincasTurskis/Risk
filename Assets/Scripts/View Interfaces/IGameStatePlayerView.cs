
public interface IGameStatePlayerView
{
    public bool is2PlayerGame{get;}
    public Player[] Players();
    public Territory[] territories();
    public int currentPlayerNo();
    public TurnStage turnStage();
    public UIManager uiManager {get;}
    public bool allTerritoriesClaimed {get;}
    public int cardSetRewardStage();
    public Player CurrentPlayer();
}