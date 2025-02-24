using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResource
{
    private string _name;
    private int _currentAmount;

    public GameResource(string name, int initialAmount)
    {
        _name = name;
        _currentAmount = initialAmount;
    }

    public void addAmount(int value)
    {
        _currentAmount += value;
        if (_currentAmount < 0 )
        {
            _currentAmount = 0;
        }
    }

    public string getName
    {
        get => _name;
    }
    public int getCurrentAmount
    {
        get => _currentAmount;
    }
}
