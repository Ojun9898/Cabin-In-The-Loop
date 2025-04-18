using System;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
   #region Manager
   private BoardManager _boardManager;
   public BoardManager boardManager
   {
      get
      {
         if (_boardManager == null)
            _boardManager = FindObjectOfType<BoardManager>(true);
         
         return _boardManager;
      }
   }
   
   private UIManager _uiManager;
   public UIManager uiManager
   {
      get
      {
         if (_uiManager == null)
            _uiManager = FindObjectOfType<UIManager>(true);
         
         return _uiManager;
      }
   }

   private Spawner spawner;
   
   private PoolManager _poolManager;
   public PoolManager poolManager
   {
      get
      {
         if (_poolManager == null)
            _poolManager = FindObjectOfType<PoolManager>(true);
         
         return _poolManager;
      }
   }
   
   #endregion
   
   public enum GameType { INTRO, BUILD, PLAY, OUTRO }
   public GameType e_GameType = GameType.INTRO;
   
   void Start()
   {
      spawner = this.GetComponent<Spawner>();
      
      boardManager.CreateBoard();
   }

   void Update()
   {
      switch (e_GameType)
      {
         case GameType.INTRO:
            break;
         case GameType.BUILD:
            boardManager.RayToBoard();
            break;
         case GameType.PLAY:
            spawner.CreateMonster();
            break;
         case GameType.OUTRO:
            break;
      }
   }

   public void OnChageType(GameType gameType)
   {
      if (e_GameType != gameType)
         e_GameType = gameType;
   }
   
}