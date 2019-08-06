using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    private static Queue<ICommand> commandBuffer;

    private static List<ICommand> commandHistory;
    private static int counter;

    private void Awake()
    {
        commandBuffer = new Queue<ICommand>();
        commandHistory = new List<ICommand>();
    }

    public static void AddCommand(ICommand command)
    {
        commandBuffer.Enqueue(command);
    }
    
    void Update()
    {
        if (commandBuffer.Count > 0)
        {
            ICommand c = commandBuffer.Dequeue();
            c.Execute();
            commandHistory.Add(c);
            counter++;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (counter > 0)
                {
                    counter--;
                    commandHistory[counter].Undo();
                    commandHistory.RemoveAt(counter);
                }
            }
        }
    }   
}
