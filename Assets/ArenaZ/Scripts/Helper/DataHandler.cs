using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedApple;
using System.Linq;

namespace ArenaZ
{
    public class DataHandler : Singleton<DataHandler>
    {
        [SerializeField] private List<SquareFrameData> squareFrameDatas = new List<SquareFrameData>();
        [SerializeField] private List<RaceData> profilePicDatas = new List<RaceData>();
        [SerializeField] private List<RaceData> fullBodyDatas = new List<RaceData>();

        public SquareFrameData GetSquareFrameData(EColor a_ColorId)
        {
            return squareFrameDatas.Where(x => x.ColorId.Equals(a_ColorId)).First();
        }

        public CharacterPicData GetCharacterPicData(ERace a_RaceId, EColor a_ColorId)
        {
            CharacterPicData t_CharacterData = null;
            RaceData t_ProfilePicData = profilePicDatas.Where(x => x.RaceId.Equals(a_RaceId)).First();
            if (t_ProfilePicData != null)
                t_CharacterData = t_ProfilePicData.CharacterPicDatas.Where(x => x.ColorId.Equals(a_ColorId)).First();
            return t_CharacterData;
        }
    }

    [System.Serializable]
    public class SquareFrameData
    {
        public EColor ColorId;
        public Sprite FramePic;
    }

    [System.Serializable]
    public class RaceData
    {
        public ERace RaceId;
        public List<CharacterPicData> CharacterPicDatas = new List<CharacterPicData>();
    }

    [System.Serializable]
    public class CharacterPicData
    {
        public EColor ColorId;
        public Sprite ProfilePic;
    }
}
