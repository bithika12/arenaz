using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevCommons.UI;
using System;
using UnityEngine.UI;
using System.Linq;
using DevCommons.Utility;
using ArenaZ;

public class LeaderboardCell : Cell<LeaderboardData>
{
    [Header("User Elements")]
    [SerializeField] private Text userRank;
    [SerializeField] private Text userCupCount;
    [SerializeField] private Text userName;
    [SerializeField] private Image userFrame;
    [SerializeField] private Image userPic;
    [SerializeField] private Image userCountry;

    public override void InitializeCell(LeaderboardData cellData, Action<LeaderboardData> onClickCallback = null)
    {
        base.InitializeCell(cellData, onClickCallback);
        if (cellData != null)
        {
            userRank.text = cellData.Rank.ToString();
            userCupCount.text = cellData.CupNumber.ToString();
            userName.text = cellData.Name;

            SetUserProfileImage(cellData.RaceName, cellData.ColorName);
            SetCountryImage(cellData.CountryName);
        }
    }

    private void SetUserProfileImage(string a_Race, string a_Color)
    {
        try
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
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void SetCountryImage(string a_CountryId)
    {
        try
        {
            CountryPicData t_CountryPicData = DataHandler.Instance.GetCountryPicData(a_CountryId);
            if (t_CountryPicData != null)
            {
                userCountry.sprite = t_CountryPicData.CountryPic;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
