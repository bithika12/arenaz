using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

namespace ArenaZ
{
    public class RegionDataLoader : MonoBehaviour
    {
        [SerializeField] private RegionCell regionCellPrototype;
        [SerializeField] private Transform container;
        [SerializeField] private ScrollRect regionScrollRect;

        private List<RegionCell> activeRegionCells = new List<RegionCell>();
        private Action<CountryPicData> onSelect;

        public void PopulateRegions(Action<CountryPicData> a_OnSelect)
        {
            onSelect = a_OnSelect;
            ClearCells();

            List<CountryPicData> datas = DataHandler.Instance.CountryPicDatas;
            Debug.Log($"RegionData Count: {datas.Count}");
            for (int i = 0; i < datas.Count; i++)
            {
                GameObject t_Go = Instantiate(regionCellPrototype.gameObject, container);
                t_Go.SetActive(true);
                RegionCell t_RegionCell = t_Go.GetComponent<RegionCell>();
                t_RegionCell.InitializeCell(datas.ElementAt(i), OnSelectCountry);
                activeRegionCells.Add(t_RegionCell);
            }
            StartCoroutine(OnCompletePopulateAction());
        }

        private IEnumerator OnCompletePopulateAction()
        {
            yield return new WaitForSeconds(1.0f);
            regionScrollRect.verticalNormalizedPosition = 1;
        }

        private void OnSelectCountry(CountryPicData a_Obj)
        {
            onSelect?.Invoke(a_Obj);
        }

        private void ClearCells()
        {
            if (activeRegionCells.Any())
            {
                activeRegionCells.ForEach(x => Destroy(x.gameObject));
                activeRegionCells.Clear();
            }
        }

        public void CloseWindow()
        {
            ClearCells();
            gameObject.SetActive(false);
        }
    }
}
