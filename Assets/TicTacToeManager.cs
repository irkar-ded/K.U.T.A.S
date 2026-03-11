using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeManager : MonoBehaviour
{
    public enum Winner
    {
        Player,
        Enemy,
        None
    }
    [System.Serializable]
    public class Grid
    {
        public int[] cells;
        public Grid(int[] cellsNew)=>cells = cellsNew;
    }
    [Header("System")]
    bool gameIsEnd = false;
    [SerializeField] Winner whatsTurn = Winner.Player;
    [SerializeField] Grid currentGrid = new Grid(new []
        {
            0,0,0,
            0,0,0,
            0,0,0
        });
    Grid[] patternsWin =
    {
        new Grid(new []
        {
            1,1,1,
            0,0,0,
            0,0,0
        }),
        new Grid(new []
        {
            0,0,0,
            1,1,1,
            0,0,0
        }),
        new Grid(new []
        {
            0,0,0,
            0,0,0,
            1,1,1
        }),
        new Grid(new []
        {
            1,0,0,
            1,0,0,
            1,0,0
        }),
        new Grid(new []
        {
            0,1,0,
            0,1,0,
            0,1,0
        }),
        new Grid(new []
        {
            0,0,1,
            0,0,1,
            0,0,1
        }),
        new Grid(new []
        {
            0,0,1,
            0,1,0,
            1,0,0
        }),
        new Grid(new []
        {
            1,0,0,
            0,1,0,
            0,0,1
        }),
    };
    [Header("UI")]
    [SerializeField] GameObject prefabCell;
    [SerializeField] Transform panelGrid;
    List<TextMeshProUGUI> textButtons = new List<TextMeshProUGUI>();
    void Start()=>CreateGrid();
    public void CreateGrid()
    {
        for(int i = 0; i < currentGrid.cells.Length; i++)
        {
            int idCell = i;
            Button tmpCell = Instantiate(prefabCell,panelGrid).GetComponent<Button>();
            tmpCell.onClick.AddListener(() => SelectCell(idCell));
            textButtons.Add(tmpCell.GetComponentInChildren<TextMeshProUGUI>());
        }
        RefreshCells();
    }
    public void SelectCell(int idCell)
    {
        if(gameIsEnd)
            return;
        if(currentGrid.cells[idCell] == 0)
        {
            switch (whatsTurn)
            {
                case Winner.Player:
                    currentGrid.cells[idCell] = 1;
                break;
                case Winner.Enemy:
                    currentGrid.cells[idCell] = -1;
                break;
            }
        }
        RefreshCells();
        if(CheckWin() != Winner.None)
            gameIsEnd = true;
        print(CheckWin().ToString());
    }
    public void RefreshCells()
    {
        for(int i = 0; i < currentGrid.cells.Length; i++)
        {
            switch (currentGrid.cells[i])
            {
                case 0:
                    textButtons[i].text = "-";
                break;
                case 1:
                    textButtons[i].text = "X";
                break;
                case -1:
                    textButtons[i].text = "O";
                break;
            }
        }
    }
    public Winner CheckWin()
    {
        Winner currentWinner = Winner.None;
        for(int l = 0; l < 2; l++)
        {
            for(int i = 0;i < patternsWin.Length; i++)
            {
                currentWinner = l == 0 ? Winner.Player : Winner.Enemy;
                int countToWin = 0;
                for(int j = 0;j < patternsWin[i].cells.Length; j++)
                {
                    if(patternsWin[i].cells[j] == 0)
                        continue;
                    if(currentGrid.cells[j] == (currentWinner == Winner.Player ? -1 : 1))
                    {
                        currentWinner = Winner.None;
                        break;
                    }
                    if(currentGrid.cells[j] == (currentWinner == Winner.Player ? 1 : -1))
                        countToWin++;
                }
                if(currentWinner != Winner.None && countToWin >= 3)
                    return currentWinner;
            }
        }
        return Winner.None;
    }
}
