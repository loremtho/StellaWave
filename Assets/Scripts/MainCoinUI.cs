using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainCoinUI : MonoBehaviour
{
    public TextMeshProUGUI coin;
    public DataJson dataJson;
    public void MainCoinText()
    {
        coin.text = string.Format("Coin : {0:n0}", dataJson.coinData.Coin);
    }
    private void Update() {
        MainCoinText();
    }
}
