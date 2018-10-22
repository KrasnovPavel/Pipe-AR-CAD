using System;
using System.Collections;
using System.Collections.Generic;

public class Standart  {

    //TEMPORAL
    // Key is outer diameter in millimeters/10
    // Values are first bend radius, second bend radius, maximum angle
    private static Dictionary<int, Tuple<int, int, int>> tubesData = new Dictionary<int, Tuple<int, int, int>>()
    {
        { 250, Tuple.Create(70, 55, 180) }
    };

    public static BendedTube CreateBendedTube(int outerDiameter)
    {
        return new BendedTube(outerDiameter, tubesData[outerDiameter]);
    }
}
