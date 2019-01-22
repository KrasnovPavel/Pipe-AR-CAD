package com.ismart_ar.narfu.mobilecontroller;

import java.util.List;

public class JSON{
    public String Type;
    List<Axes> Axes;

    public JSON(){

    }

    public static class Axes {

        public String Axis;
        public Double Value;

        public Axes(String Axis, Double Value) {
            this.Axis = Axis;
            this.Value = Value;
        }
    }
    public Boolean HasChild;
}