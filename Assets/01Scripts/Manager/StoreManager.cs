using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class StoreManager : MonoBehaviour
{
    public List<GameObject> ExplainPanel;

    public List<GameObject> check;

    public GameObject ConsumePanel;
    public GameObject EquipPanel;

    public void SelectConsume()
    {
        ConsumePanel.SetActive(true);
        EquipPanel.SetActive(false);
    }

    public void SelectEquipment()
    {
        ConsumePanel.SetActive(false);
        EquipPanel.SetActive(true);
    }

    private void ShowPanel(string panelName, int checkIndex)
    {
        foreach (GameObject t in ExplainPanel)
        {
            t.SetActive(t.name == panelName);
        }

        for (int i = 0; i < check.Count; i++)
        {
            check[i].SetActive(i == checkIndex);
        }
    }

    // Consume
    public void Potion()
    {
        ShowPanel("PotionPanel", 0);
    }

    public void Meat()
    {
        ShowPanel("MeatPanel", 1);
    }

    // Equipment
    public void NormalSword()
    {
        ShowPanel("NormalSwordPanel", 2);
    }

    public void NormalPant()
    {
        ShowPanel("NormalPantsPanel", 3);
    }

    public void NormalHelmet()
    {
        ShowPanel("NormalHelmetPanel", 4);
    }

    public void NormalGloves()
    {
        ShowPanel("NormalGlovesPanel", 5);
    }

    public void NormalChest()
    {
        ShowPanel("NormalChestPanel", 6);
    }

    public void NormalShoes()
    {
        ShowPanel("NormalBootsPanel", 7);
    }

    // Knight
    public void KnightSword()
    {
        ShowPanel("KnightSwordPanel", 8);
    }

    public void KnightPant()
    {
        ShowPanel("KnightPantsPanel", 9);
    }

    public void KnightHelmet()
    {
        ShowPanel("KnightHelmetPanel", 10);
    }

    public void KnightGloves()
    {
        ShowPanel("KnightGlovesPanel", 11);
    }

    public void KnightChest()
    {
        ShowPanel("KnightChestPanel", 12);
    }

    public void KnightShoes()
    {
        ShowPanel("KnightBootsPanel", 13);
    }




    /*
    /// <summary>
    /// Consume
    /// </summary>

    public void Potion()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "PotionPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 0)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }

    public void Meat()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "MeatPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }
        }

        for(int i = 0; i< check.Count; i++)
        {
            if(i == 1)
            {
                check[i].SetActive(true);
            }
            else
            {
                check[i].SetActive(false);
            }
        }
    }






    /// <summary>
    /// Equipment
    /// </summary>
    // Normal
    public void NormalSword()
    {
        foreach(GameObject t in ExplainPanel)
        {
            if(t.name == "NormalSwordPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 2)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }

    public void NormalPant()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "NormalPantsPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 3)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }

    public void NormalHelmet()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "NormalHelmetPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 4)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }
    public void NormalGloves()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "NormalGlovesPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 5)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }
    public void NormalChest()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "NormalChestPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 6)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }
    public void NormalShoes()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "NormalBootsPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 7)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }


    //  Knight
    public void KnightSword()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "KnightSwordPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 8)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }
    public void KnightPant()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "KnightPantsPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 9)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }
    public void KnightHelmet()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "KnightHelmetPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 10)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }

    public void KnightGloves()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "KnightGlovesPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 11)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }

    public void KnightChest()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "KnightChestPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 12)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }

    public void KnightShoes()
    {
        foreach (GameObject t in ExplainPanel)
        {
            if (t.name == "KnightBootsPanel")
            {
                t.SetActive(true);
            }
            else
            {
                t.SetActive(false);
            }

            for (int i = 0; i < check.Count; i++)
            {
                if (i == 13)
                {
                    check[i].SetActive(true);
                }
                else
                {
                    check[i].SetActive(false);
                }
            }
        }
    }
    */
}
