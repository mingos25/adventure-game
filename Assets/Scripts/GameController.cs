using System;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public FollowCamera FollowCamera;

    public GameObject PlayerPrefab;
    private Player player;

    public Player Player
    {
        get { return player; }
    }
    
    public enum eGameState
    {
        PlayerTurn,
        MonsterTurn
    }
    public eGameState GameState = eGameState.PlayerTurn;

    public void GoToState(eGameState _state)
    {
        GameState = _state;
        switch (GameState)
        {
            case eGameState.MonsterTurn:
                MonsterController.Instance.MoveMonster();
                break;
        }
    }

    private void Start()
    {
        StartNewGame();
    }

    void StartNewGame()
    {
        CreatePlayer();
        DungeonController.Instance.CreateNewDungeon();
    }

    void CreatePlayer()
    {
        GameObject playerGo = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        player = playerGo.GetComponent<Player>();
        player.Reset();
        FollowCamera.Target = playerGo.transform;
    }
}
