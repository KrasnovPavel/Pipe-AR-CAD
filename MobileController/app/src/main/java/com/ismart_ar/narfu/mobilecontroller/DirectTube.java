package com.ismart_ar.narfu.mobilecontroller;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.util.Log;
import android.widget.TextView;

import com.google.gson.Gson;
import java.math.BigDecimal;
import java.math.RoundingMode;

import io.github.controlwear.virtual.joystick.android.JoystickView;

public class DirectTube extends BluetoothMessengerActivity{

    double Strength;
    int switchVariable;
    JoystickView joystick;
    TextView lengthTxt;
    double startLength = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.direct_tube);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);

        Bundle arguments = getIntent().getExtras();
        double value = arguments.getDouble("value");

        lengthTxt = findViewById(R.id.length);
        joystick = findViewById(R.id.joystickView);
        startLength = startLength + value;
        lengthTxt.setText(Double.toString(startLength));

        joystick.setOnMoveListener((angle, strength) -> {

            if( angle>=0 && angle <=45) switchVariable = 0;
                else if( angle>=135 && angle <=180) switchVariable = 1;
                    else if( angle>=181 && angle <=225) switchVariable = 2;
                        else if( angle>=315 && angle <=360) switchVariable = 3;
                            else switchVariable = 4;

            switch (switchVariable)
            {
                case 0:
                    Strength = (0.01 * strength);
                    break;
                case 1:
                    Strength = ((-0.01) * strength);
                    break;
                case 2:
                    Strength = ((-0.01) * strength);
                    break;
                case 3:
                    Strength = (0.01 * strength);
                    break;
                case 4:
                    Strength = 0;
                    break;
            }
            double roundStrength = new BigDecimal(Strength).setScale(2, RoundingMode.UP).doubleValue();
            JSON.Axes axie = new JSON.Axes("Length", roundStrength);
            Gson gson = new Gson();
            Log.i("JSON", gson.toJson(axie));
            messenger.SendMessage(gson.toJson(axie));
            startLength = startLength + Strength;
            lengthTxt.setText(Double.toString(startLength));
        });
    }
}
