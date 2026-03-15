using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToeManager : MonoBehaviour
{
    public class MinimaxMove
    {
        public int idMove;
        public int costMove;
        public MinimaxMove(int idMove,int costMove)
        {
            this.idMove = idMove;
            this.costMove = costMove;
        }
    }
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
    [SerializeField] bool isDebug;
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
    void Update()
    {
        if(whatsTurn == Winner.Enemy && isDebug)
            MoveEnemy();
    }
    public void SetTicTacToe(bool isPlay)=>mainPanelTicTacToe.SetActive(isPlay);
    public void CreateGrid()
    {
        for(int i = 0; i < currentGrid.cells.Count; i++)
        {
            int idCell = i;
            Button tmpCell = Instantiate(prefabCell,panelGrid).GetComponent<Button>();
            if(isDebug)
                tmpCell.onClick.AddListener(() => PlayCell(idCell));
            else
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
                    if(isDebug)
                        whatsTurn = Winner.Enemy;
                break;
                case Winner.Enemy:
                    currentGrid.cells[idCell] = -1;
                    if(isDebug)
                        whatsTurn = Winner.Player;
                break;
            }
        }
        RefreshCells();
        if(CheckWin() != Winner.None)
            gameIsEnd = true;
        print(CheckWin().ToString());
    }
    public void MoveEnemy()
    {
        List<int> currentEmptyCells = FreeCells();
            int maxValue = int.MinValue;
            int idMove = 0;
            for(int i = 0;i < currentEmptyCells.Count; i++)
            {
                currentGrid.cells[currentEmptyCells[i]] = -1;
                int value = Minimax(0,false);
                currentGrid.cells[currentEmptyCells[i]] = 0;
                if(value > maxValue)
                {
                    maxValue = value;
                    idMove = currentEmptyCells[i];
                }
            }
            PlayCell(idMove);
    }
    public void RefreshCells()
    {
        for(int i = 0; i < currentGrid.cells.Count; i++)
            textButtons[i].text = GetSymbolCell(currentGrid.cells[i]);
    }
    public string GetSymbolCell(int value)
    {
        switch (value)
        {
            case 0:
                return "-";
            case 1:
                return "X";
            case -1:
                return "O";
        }
        return "-";
    }
    public Winner CheckWin()
    {
        Winner currentWinner = Winner.Tie;
        for(int i = 0;i < currentGrid.cells.Count; i++)
        {
            if(currentGrid.cells[i] == 0)
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
    int Minimax(int depth,bool maximizingPlayer)
    {
        Winner winner = CheckWin();
        if(winner != Winner.None)
        {
            switch (winner)
            {
                case Winner.Enemy:
                    return 1;
                case Winner.Player:
                    return -1;
                case Winner.Tie:
                    return 0;
            }
        }
        int maxValue;
        List<int> canMove = FreeCells();
        if (maximizingPlayer)
        {
            maxValue = int.MinValue;
            for(int i = 0; i < canMove.Count; i++)
            {
                currentGrid.cells[canMove[i]] = -1;
                int value = Minimax(depth + 1,false);
                currentGrid.cells[canMove[i]] = 0;
                maxValue = Mathf.Max(value,maxValue);
            }
        }
        else
        {
            maxValue = int.MaxValue;
            for(int i = 0; i < canMove.Count; i++)
            {
                currentGrid.cells[canMove[i]] = 1;
                int value = Minimax(depth + 1,true);
                currentGrid.cells[canMove[i]] = 0;
                maxValue = Mathf.Min(value,maxValue);
            }
        }
        return maxValue;
    }
    public List<int> FreeCells()
    {
        List<int> idsCellsCanGo = new List<int>();
        for(int i = 0;i < currentGrid.cells.Count; i++)
        {
            if(currentGrid.cells[i] == 0)
                idsCellsCanGo.Add(i);
        }
        return idsCellsCanGo;
    }
}
