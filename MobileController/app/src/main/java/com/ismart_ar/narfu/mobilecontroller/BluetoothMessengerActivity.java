package com.ismart_ar.narfu.mobilecontroller;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.util.Log;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import java.io.IOException;
import static android.provider.AlarmClock.EXTRA_MESSAGE;

public class BluetoothMessengerActivity extends Activity {
    protected static MABluetoothMessenger messenger;
    private String type = "null";
    public double Length = 0;
    public double camberAngle = 0;
    public double Angle = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    protected class MABluetoothMessenger extends BluetoothMessengerServer {
        MABluetoothMessenger() throws IOException {
            super();
        }

        @Override
        public void MessageReceived(String message) {
            message = message.substring(4, message.length());
            Log.i("", message);
            GsonBuilder builder = new GsonBuilder();
            Gson gson = builder.create();
            JSON json = gson.fromJson(message, JSON.class);
            Log.i("JSON", "Тип: " + json.Type + " Ось: " + json.Axes.get(0).Axis + " Значение: " + json.Axes.get(1).Axis);

            for (JSON.Axes axe : json.Axes) {
                switch (axe.Axis) {
                    case "Length":
                        Length = axe.Value;
                        break;
                    case "camberAngle":
                        camberAngle = axe.Value;
                        break;
                    case "Angle":
                        Angle = axe.Value;
                        break;
                }
            }


            if (!json.Type.equals(type)) {
                gotoTubes(json.Type);
            }
            type = json.Type;
        }

        @Override
        public void ConnectionClosed() {
            super.ConnectionClosed();
        }

        @Override
        public void ClientConnected() {
            super.ClientConnected();
        }
    }

    public void gotoTubes ( String type) {
        Log.i("",type);
        switch(type) {
            case "empty":
                Intent emptyIntent = new Intent(BluetoothMessengerActivity.this, Empty.class);
                emptyIntent.putExtra(EXTRA_MESSAGE,"");
                startActivity(emptyIntent);
                break;
            case "start":
                Intent startIntent = new Intent(BluetoothMessengerActivity.this, StartTube.class);
                startIntent.putExtra(EXTRA_MESSAGE,"");
                startActivity(startIntent);
                break;
            case "direct":
                Intent directIntent = new Intent(BluetoothMessengerActivity.this, DirectTube.class);
                directIntent.putExtra("value", Length);
                startActivity(directIntent);
                break;
            case "bended":
                Intent bendedIntent = new Intent(BluetoothMessengerActivity.this, BendedTube.class);
                bendedIntent.putExtra("camberAngle", camberAngle);
                bendedIntent.putExtra("Angle", Angle);
                startActivity(bendedIntent);
                break;
        }
    }
}
