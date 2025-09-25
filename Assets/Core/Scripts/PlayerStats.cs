using System;
using System.Collections.Generic;
using Unity;
using UnityEngine.UIElements;

public class PlayerStats
{
    public EventHandler<PointChangeArgs> OnPointChange;
    public class PointChangeArgs : EventArgs
    {
        public int OldAmount;
        public int NewAmount;
    }

    private static PlayerStats instance;
    public static PlayerStats Instance { get { if (instance is null) instance = new PlayerStats(); return instance; } }

    public Dictionary<AnimalData, PointData> Overview = new Dictionary<AnimalData, PointData>();
    public int TotalPoints
    {
        get
        {
            int points = 0;
            foreach (PointData pointData in Overview.Values)
            {
                points += pointData.Points;
            }
            return points;
        }
    }
}

public class PointData
{
    private int points;
    public int Points
    {
        get => points; private set
        {
            PlayerStats.Instance.OnPointChange?.Invoke(this, new PlayerStats.PointChangeArgs
            {
                OldAmount = points,
                NewAmount = value
            });
            points = value;
            if (PointLabel is not null) PointLabel.text = $"{Points} / {MaxPoints}";
        }
    }
    public int MaxPoints { get; private set; }

    public Label PointLabel { get; private set; }

    public void AddPoint()
    {
        Points = Math.Clamp(Points + 1, 0, MaxPoints);
    }

    public void RemovePoint()
    {
        Points = Math.Clamp(Points - 1, 0, MaxPoints);
    }

    public void SetMaxPoints(int maxPoints)
    {
        MaxPoints = maxPoints;
    }

    internal void SetLabel(UnityEngine.UIElements.Label label)
    {
        this.PointLabel = label;
        if (PointLabel is not null) PointLabel.text = $"{Points} / {MaxPoints}";
    }
}
