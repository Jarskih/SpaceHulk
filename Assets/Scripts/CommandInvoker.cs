using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    private static Queue<ICommand> commandBuffer;

    private static List<ICommand> commandHistory;
    private static List<ICommand> currentPlayerHistory;
    private static int counter;

    private void Awake()
    {
        commandBuffer = new Queue<ICommand>();
        commandHistory = new List<ICommand>();
        currentPlayerHistory = new List<ICommand>();
    }

    public static void AddCommand(ICommand command)
    {
        if (counter < currentPlayerHistory.Count)
        {
            while (currentPlayerHistory.Count > counter)
            {
                currentPlayerHistory.RemoveAt(counter);
            }
        }
        
        commandBuffer.Enqueue(command);
    }
    
    void Update()
    {
        if (commandBuffer.Count > 0)
        {
            ICommand c = commandBuffer.Dequeue();
            c.Execute();
            commandHistory.Add(c);
            currentPlayerHistory.Add(c);
            counter++;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (counter > 0)
                {
                    counter--;
                    currentPlayerHistory[counter].Undo();
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if (counter < commandBuffer.Count)
                {
                    currentPlayerHistory[counter].Execute();
                    counter++;
                }
            }
        }
    }

    public void Undo()
    {
        if (counter > 0) {
            counter--;
            currentPlayerHistory[counter].Undo();
        }
    }

    public void Redo()
    {
        if (counter < currentPlayerHistory.Count)
        {
            currentPlayerHistory[counter].Execute();
            counter++;
        }
    }

    public static void ResetHistory()
    {
        counter = 0;
        currentPlayerHistory.Clear();
    }

    public static int GetPlayerHistory()
    {
        return currentPlayerHistory.Count;
    }

    public static bool CanUndo()
    {
        return currentPlayerHistory.Count > 0 && counter > 0;
    }

    public static bool CanRedo()
    {
        return currentPlayerHistory.Count > 0 && counter < currentPlayerHistory.Count;
    }
}
