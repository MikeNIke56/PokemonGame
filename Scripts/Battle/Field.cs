using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public Conditions Weather { get; set; }
    public int WeatherDuration { get; set; }

    public void SetWeather(ConditionsID conditionsID)
    {
        Weather = ConditionsDB.Conditions[conditionsID];
        Weather.Id = conditionsID;
        Weather.OnStart?.Invoke(null);
    }
}
