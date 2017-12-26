using Assets.Scripts;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemArray
{
    
    private Item[,] matrix = new Item[Globals.Rows, Globals.Columns];

    public Item this[int row, int column]
    {
        get
        {
            return matrix[row, column];
        }
        set
        {
            matrix[row, column] = value;
        }
    }

    public void GetRandomRowColumn(out int row, out int column)
    {
        do
        {
            row = random.Next(0, Globals.Rows);
            column = random.Next(0, Globals.Columns);
        } while (matrix[row, column] != null);
    }

    public List<ItemMovementDetails> MoveHorizontal(HorizontalMovement horizontalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<ItemMovementDetails>();

        
        int relativeColumn = horizontalMovement == HorizontalMovement.Left ? -1 : 1;
        
        var columnNumbers = Enumerable.Range(0, Globals.Columns);

        
        if (horizontalMovement == HorizontalMovement.Right)
        {
            columnNumbers = columnNumbers.Reverse();
        }

        for (int row = Globals.Rows - 1; row >= 0; row--)
        {  
            foreach (int column in columnNumbers)
            {
                
                if (matrix[row, column] == null) continue;

                
                ItemMovementDetails imd = AreTheseTwoItemsSame(row, column, row, column + relativeColumn);
                if (imd != null)
                {
                    
                    movementDetails.Add(imd);
                    
                    continue;
                }

                
                int columnFirstNullItem = -1;

                
                int numberOfItemsToTake = horizontalMovement == HorizontalMovement.Left
                ? column : Globals.Columns - column;

                bool emptyItemFound = false;

                
                foreach (var tempColumnFirstNullItem in columnNumbers.Take(numberOfItemsToTake))
                {
                    
                    columnFirstNullItem = tempColumnFirstNullItem;
                    if (matrix[row, columnFirstNullItem] == null)
                    {
                        emptyItemFound = true;
                        break;
                    }
                }

               
                if (!emptyItemFound)
                {
                    continue;
                }


                ItemMovementDetails newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne
                (row, row, row, column, columnFirstNullItem, columnFirstNullItem + relativeColumn);

                movementDetails.Add(newImd);


            }
        }
        return movementDetails;
    }



    public List<ItemMovementDetails> MoveVertical(VerticalMovement verticalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<ItemMovementDetails>();

        int relativeRow = verticalMovement == VerticalMovement.Bottom ? -1 : 1;
        var rowNumbers = Enumerable.Range(0, Globals.Rows);

        if (verticalMovement == VerticalMovement.Top)
        {
            rowNumbers = rowNumbers.Reverse();
        }

        for (int column = 0; column < Globals.Columns; column++)
        {
            foreach (int row in rowNumbers)
            {
                
                if (matrix[row, column] == null) continue;

                
                ItemMovementDetails imd = AreTheseTwoItemsSame(row, column, row + relativeRow, column);
                if (imd != null)
                {
                    movementDetails.Add(imd);

                    continue;
                }

                
                int rowFirstNullItem = -1;

                int numberOfItemsToTake = verticalMovement == VerticalMovement.Bottom
                ? row : Globals.Rows - row;


                bool emptyItemFound = false;

                foreach (var tempRowFirstNullItem in rowNumbers.Take(numberOfItemsToTake))
                {
                    rowFirstNullItem = tempRowFirstNullItem;
                    if (matrix[rowFirstNullItem, column] == null)
                    {
                        emptyItemFound = true;
                        break;
                    }
                }

                if (!emptyItemFound)
                {
                    continue;
                }

                ItemMovementDetails newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne(row, rowFirstNullItem, rowFirstNullItem + relativeRow, column, column, column);

                movementDetails.Add(newImd);
            }
        }
        return movementDetails;
    }

    private ItemMovementDetails MoveItemToNullPositionAndCheckIfSameWithNextOne
(int oldRow, int newRow, int itemToCheckRow, int oldColumn, int newColumn, int itemToCheckColumn)
    {
       
        matrix[newRow, newColumn] = matrix[oldRow, oldColumn];
        matrix[oldRow, oldColumn] = null;

        
        ItemMovementDetails imd2 = AreTheseTwoItemsSame(newRow, newColumn, itemToCheckRow,
            itemToCheckColumn);
        if (imd2 != null)
        {
            return imd2;
        }
        else
        {
            return
                new ItemMovementDetails(newRow, newColumn, matrix[newRow, newColumn].GO, null);

        }
    }

    private ItemMovementDetails AreTheseTwoItemsSame(
        int originalRow, int originalColumn, int toCheckRow, int toCheckColumn)
    {
        if (toCheckRow < 0 || toCheckColumn < 0 || toCheckRow >= Globals.Rows || toCheckColumn >= Globals.Columns)
            return null;


        if (matrix[originalRow, originalColumn] != null && matrix[toCheckRow, toCheckColumn] != null
                && matrix[originalRow, originalColumn].Value == matrix[toCheckRow, toCheckColumn].Value
                && !matrix[toCheckRow, toCheckColumn].WasJustDuplicated)
        {
            
            matrix[toCheckRow, toCheckColumn].Value *= 2;
            matrix[toCheckRow, toCheckColumn].WasJustDuplicated = true;
            
            var GOToAnimateScaleCopy = matrix[originalRow, originalColumn].GO;
            
            matrix[originalRow, originalColumn] = null;
            return new ItemMovementDetails(toCheckRow, toCheckColumn, matrix[toCheckRow, toCheckColumn].GO, GOToAnimateScaleCopy);

        }
        else
        {
            return null;
        }
    }

    private void ResetWasJustDuplicatedValues()
    {
        for (int row = 0; row < Globals.Rows; row++)
            for (int column = 0; column < Globals.Columns; column++)
            {
                if (matrix[row, column] != null && matrix[row, column].WasJustDuplicated)
                    matrix[row, column].WasJustDuplicated = false;
            }
    }

    private System.Random random = new System.Random();
}

public enum HorizontalMovement { Left, Right };
public enum VerticalMovement { Top, Bottom };
