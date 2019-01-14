package com.ismart_ar.narfu.mobilecontroller;

public class JSON extends BluetoothMessengerActivity{
    public String Type;
    public Axies Axes;
    public Boolean HasChild;

    public static class Axies {

        public String Axis;
        public Double Values;

        public Axies(String Axis, Double Values) {
            this.Axis = Axis;
            this.Values = Values;
        }
    }
}