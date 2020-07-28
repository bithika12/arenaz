using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevCommons.UI;
using System;
using UnityEngine.UI;

namespace ArenaZ
{
    public class RegionCell : Cell<CountryPicData>
    {
        [SerializeField] private Image countryImage;
        [SerializeField] private Text countryName;
        
        public override void InitializeCell(CountryPicData cellData, Action<CountryPicData> onClickCallback = null)
        {
            base.InitializeCell(cellData, onClickCallback);

            countryName.text = cellData.CountryId;
            countryImage.sprite = cellData.CountryPic;
        }
    }
}
