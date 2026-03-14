using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToeManager : MonoBehaviour
{
    public enum Winner
    {
        Player,
        Enemy,
        Tie,
        None
    }
    [System.Serializable]
    public class Grid
    {
        public List<int> cells;
        public Grid(List<int> cellsNew)=>cells = cellsNew;
    }
    [Header("System")]
    bool gameIsEnd = false;
    public Winner whatsTurn = Winner.Player;
    [SerializeField] Grid currentGrid = new Grid(new List<int>
        {
            0,0,0,
            0,0,0,
            0,0,0
        });
    Grid[] patternsWin =
    {
        new Grid(new List<int>
        {
            1,1,1,
            0,0,0,
            0,0,0
        }),
        new Grid(new List<int>
        {
            0,0,0,
            1,1,1,
            0,0,0
        }),
        new Grid(new List<int>
        {
            0,0,0,
            0,0,0,
            1,1,1
        }),
        new Grid(new List<int>
        {
            1,0,0,
            1,0,0,
            1,0,0
        }),
        new Grid(new List<int>
        {
            0,1,0,
            0,1,0,
            0,1,0
        }),
        new Grid(new List<int>
        {
            0,0,1,
            0,0,1,
            0,0,1
        }),
        new Grid(new List<int>
        {
            0,0,1,
            0,1,0,
            1,0,0
        }),
        new Grid(new List<int>
        {
            1,0,0,
            0,1,0,
            0,0,1
        }),
    };
    [Header("UI")]
    [SerializeField] GameObject mainPanelTicTacToe;
    [SerializeField] GameObject prefabCell;
    [SerializeField] Transform panelGrid;
    public UnityEvent<int> onChooseCell;
    List<TextMeshProUGUI> textButtons = new List<TextMeshProUGUI>();
    public static TicTacToeManager instance;
    void Awake()
    {
        instance = this;
        CreateGrid();
    }
    public void SetTicTacToe(bool isPlay)=>mainPanelTicTacToe.SetActive(isPlay);
    public void CreateGrid()
    {
        for(int i = 0; i < currentGrid.cells.Count; i++)
        {
            int idCell = i;
            Button tmpCell = Instantiate(prefabCell,panelGrid).GetComponent<Button>();
            tmpCell.onClick.AddListener(() => SelectCell(idCell));
            textButtons.Add(tmpCell.GetComponentInChildren<TextMeshProUGUI>());
        }
        RefreshCells();
    }
    public void SelectCell(int id)
    {
        if(gameIsEnd)
            return;
        onChooseCell.Invoke(id);
    }
    public void PlayCell(int idCell)
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
        if(CheckWin(currentGrid) != Winner.None)
            gameIsEnd = true;
        print(CheckWin(currentGrid).ToString());
    }
    public void MoveEnemy()
    {
        PlayCell(idCellToMoveAI(currentGrid));
    }
    public void RefreshCells()
    {
        for(int i = 0; i < currentGrid.cells.Count; i++)
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
    public Winner CheckWin(Grid grid)
    {
        Winner currentWinner = Winner.Tie;
        for(int i = 0;i < grid.cells.Count; i++)
        {
            if(grid.cells[i] == 0)
            {
                currentWinner = Winner.None;
                break;
            }
        }
        if(currentWinner == Winner.Tie)
            return currentWinner;
        for(int l = 0; l < 2; l++)
        {
            for(int i = 0;i < patternsWin.Length; i++)
            {
                currentWinner = l == 0 ? Winner.Player : Winner.Enemy;
                int countToWin = 0;
                for(int j = 0;j < patternsWin[i].cells.Count; j++)
                {
                    if(patternsWin[i].cells[j] == 0)
                        continue;
                    if(grid.cells[j] == (currentWinner == Winner.Player ? -1 : 1))
                    {
                        currentWinner = Winner.None;
                        break;
                    }
                    if(grid.cells[j] == (currentWinner == Winner.Player ? 1 : -1))
                        countToWin++;
                }
                if(currentWinner != Winner.None && countToWin >= 3)
                    return currentWinner;
            }
        }
        return Winner.None;
    }
    public int idCellToMoveAI(Grid grid)
    {
        List<int> idsCellsCanGo = new List<int>();
        for(int i = 0;i < grid.cells.Count; i++)
        {
            if(grid.cells[i] == 0)
                idsCellsCanGo.Add(i);
        }
        int isBetterMove = int.MinValue;
        int idBestMove = 0;
        for(int i = 0;i < idsCellsCanGo.Count; i++)
        {
            Grid gridTemp = new Grid(null);
            gridTemp.cells = new List<int>();
            gridTemp.cells.AddRange(grid.cells);
            gridTemp.cells[idsCellsCanGo[i]] = -1;
            int moveCost = Minimax(gridTemp);
            if (moveCost > isBetterMove)
            {
                isBetterMove = moveCost;
                idBestMove = idsCellsCanGo[i];
            }
        }
        return idBestMove;
    }
    int Minimax(Grid grid) 
    {
        Winner winner = CheckWin(grid);
        if (winner != Winner.None)
        {
            switch (winner)
            {
                case Winner.Player:
                    return -1;
                case Winner.Enemy:
                    return 1;
                case Winner.Tie:
                    return 0;
            }
        }
        List<int> idsCellsCanGo = new List<int>();
        for(int i = 0;i < grid.cells.Count; i++)
        {
            if(grid.cells[i] == 0)
                idsCellsCanGo.Add(i);
        }
        int isBetterMove = int.MinValue;
        for(int i = 0;i < idsCellsCanGo.Count; i++)
        {
            Grid gridTemp = new Grid(null);
            gridTemp.cells = new List<int>();
            gridTemp.cells.AddRange(grid.cells);
            gridTemp.cells[idsCellsCanGo[i]] = -1;
            int moveCost = Minimax(gridTemp);
            if (moveCost > isBetterMove)
                isBetterMove = moveCost;
        }
        return isBetterMove;
    }
}
