using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevCommons.UI;
using System;
using UnityEngine.UI;
using System.Linq;
using DevCommons.Utility;
using ArenaZ;

public class MatchDetailsCell : Cell<GameHistoryGameDetails>
{
    [Header("User Elements")]
    [SerializeField] private Text userStatus;
    [SerializeField] private Text userCoinAmount;
    [SerializeField] private Text userCupCount;
    [SerializeField] private Text userName;
    [SerializeField] private Image userFrame;
    [SerializeField] private Image userPic;

    [Header("Opponent Elements")]
    [SerializeField] private Text opponentCupCount;
    [SerializeField] private Text opponentName;
    [SerializeField] private Image opponentFrame;
    [SerializeField] private Image opponentPic;

    [Header("Other Elements")]
    [SerializeField] private Text gameplayTime;
    [SerializeField] private Text playedAgo;

    public override void InitializeCell(GameHistoryGameDetails cellData, Action<GameHistoryGameDetails> onClickCallback = null)
    {
        base.InitializeCell(cellData, onClickCallback);
        if (cellData.gameHistoryUserDatas.Count == 2)
        {
            GameHistoryUserData t_UserData = null;
            GameHistoryUserData t_OpponentData = null;

            foreach (var item in cellData.gameHistoryUserDatas)
            {
                if (item.Id.Equals(User.UserId))
                    t_UserData = item;
                else
                    t_OpponentData = item;
            }

            if (t_UserData != null) 
            {
                EWinningStatus winningStatus = (EWinningStatus)t_UserData.GameResult;
                userStatus.text = winningStatus.ToString();
                userStatus.color = winningStatus.Equals(EWinningStatus.Defeat) ? Color.red : winningStatus.Equals(EWinningStatus.Victory) ? Color.green : winningStatus.Equals(EWinningStatus.Draw) ? Color.yellow : Color.white;

                userCoinAmount.text = t_UserData.CoinNumber.ToString();
                userCupCount.text = t_UserData.CupNumber.ToString();
                userName.text = t_UserData.Name;

                SetUserProfileImage(t_UserData.RaceName, t_UserData.ColorName);
            }

            if (t_OpponentData != null)
            {
                opponentCupCount.text = t_OpponentData.CupNumber.ToString();
                opponentName.text = t_OpponentData.Name;

                SetOpponentProfileImage(t_OpponentData.RaceName, t_OpponentData.ColorName);
            }

            gameplayTime.text = GetTimeInFormat(cellData.GameTime);
            playedAgo.text = GetTimeInFormat(cellData.LastTime, cellData.TimePeriodType);
        }
    }

    public void SetUserProfileImage(string a_Race, string a_Color)
    {
        ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), a_Race);
        EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), a_Color);

        SquareFrameData t_FrameData = DataHandler.Instance.GetSquareFrameData(t_Color);
        if (t_FrameData != null)
        {
            userFrame.sprite = t_FrameData.FramePic;
        }

        CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
        if (t_CharacterPicData != null)
        {
            userPic.sprite = t_CharacterPicData.ProfilePic;
        }
    }

    public void SetOpponentProfileImage(string a_Race, string a_Color)
    {
        ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), a_Race);
        EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), a_Color);

        SquareFrameData t_FrameData = DataHandler.Instance.GetSquareFrameData(t_Color);
        if (t_FrameData != null)
        {
            opponentFrame.sprite = t_FrameData.FramePic;
        }

        CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
        if (t_CharacterPicData != null)
        {
            opponentPic.sprite = t_CharacterPicData.ProfilePic;
        }
    }

    private string GetTimeInFormat(int seconds, string periodType)
    {
        string timeFormat = "";
        if (!string.IsNullOrEmpty(periodType) && !string.IsNullOrWhiteSpace(periodType))
        {
            if (periodType.StartsWith("Minutes"))
                timeFormat = seconds + "mm AGO";
            else if (periodType.StartsWith("Hours"))
                timeFormat = seconds + "H AGO";
            else if (periodType.StartsWith("Days"))
                timeFormat = seconds + "D AGO";
            else
                timeFormat = seconds + periodType + " AGO";
        }
        return timeFormat;
    }

    private string GetTimeInFormat(int seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"mm\:ss");
        //return time.ToString(@"hh\:mm\:ss");
    }
}
