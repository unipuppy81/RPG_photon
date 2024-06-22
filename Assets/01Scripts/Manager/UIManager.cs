using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    public GameObject _InventoryPanel;
    public GameObject _EqipmentPanel;
    public GameObject _ShopPanel;

    bool activeInventory = false;
    bool activeShop = false;


    private void Start()
    {
        _InventoryPanel.SetActive(activeInventory);
        _EqipmentPanel.SetActive(activeInventory);
        _ShopPanel.SetActive(activeShop);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            _InventoryPanel.SetActive(activeInventory);
            _EqipmentPanel.SetActive(activeInventory);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            activeShop = !activeShop;
            _ShopPanel.SetActive(activeShop);
        }
    }
}
