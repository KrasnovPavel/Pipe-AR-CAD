using System.Collections.Generic;
using UnityEngine;

public static class TubeManager
{
    public static readonly List<BaseTube> AllTubes = new List<BaseTube>();

    private static BaseTube _selectedTube;

    public static BaseTube SelectedTube
    {
        get { return _selectedTube; }
    }

    public static void AddTube(BaseTube newTube)
    {
        AllTubes.Add(newTube);
    }

    public static void ToggleTubeSelection(BaseTube selectedTube)
    {
        if (selectedTube.IsSelected)
        {
            DeselectTube();
        }
        else
        {
            SelectTube(selectedTube);
        }
    }

    public static void SelectTube(BaseTube selectedTube)
    {
        _selectedTube = selectedTube;

        foreach (BaseTube tube in AllTubes)
        {
            tube.IsSelected = tube == _selectedTube;
        }
    }

    public static void DeselectTube()
    {
        _selectedTube.IsSelected = false;
        _selectedTube = null;
    }
}
