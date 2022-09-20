using System.Collections;
using UnityEngine;

namespace SalatBot.Scripts.Utils.CsvReader
{
	public class CSVTest : MonoBehaviour
	{
		void Awake()
		{
			StartCoroutine(TestRoutine());
		}

		private IEnumerator TestRoutine()
		{
			var dataList = CSVReader.Read("BinanceHistory/BTCUSDT/BTCUSDT-1h-2021-04");

			yield return dataList;
			
			foreach (var data in dataList)
			{
				Debug.Log($"OpenTime  {data["OpenTime"]}\n" +
				          $"HighPrice {data["OpenPrice"]}\n" +
				          $"LowPrice {data["LowPrice"]}\n" +
				          $"ClosePrice {data["ClosePrice"]}");
			}
		}
	}
}