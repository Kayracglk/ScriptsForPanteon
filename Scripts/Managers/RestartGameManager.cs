using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGameManager : MonoBehaviour
{
    public static RestartGameManager instance;

    public List<IRestart> restarts = new List<IRestart>();

    private void Awake()
    {
        instance = this;
    }

    public void AddRestart(IRestart restart)
    {
        restarts.Add(restart);
    }

    public void RestartGame()
    {
        print(restarts.Count);

        for(int i = 0; i < restarts.Count; i++)
        {
            IRestart restart = restarts[i];

            restart.Restart();
        }
    }
}

public interface IRestart
{
    void Restart()
    {

    }
}
