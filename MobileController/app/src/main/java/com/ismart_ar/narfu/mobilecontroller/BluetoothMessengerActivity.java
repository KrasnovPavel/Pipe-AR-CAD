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
    String typeOfTube = "bended";
    MainActivity mainAct = new MainActivity();

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
            GsonBuilder builder = new GsonBuilder();
            Gson gson = builder.create();
            JSON json = gson.fromJson(message, JSON.class);
            Log.i("JSON", "Тип: " + json.Type + " Ось: " + json.Axes.Axis + " Значение: " + json.Axes.Values);
            typeOfTube = json.Type;
            gotoTube(typeOfTube);
//            if (json.HasChild) {
//                addDirect.setEnabled(false);
//                addBended.setEnabled(false);
//            }
        }

        @Override
        public void ConnectionClosed() {
            super.ConnectionClosed();
            mainAct.state.setText("Отключено");
        }

        @Override
        public void ClientConnected() {
            super.ClientConnected();
            mainAct.state.setText("Подключено");
        }
    }

    public void gotoTube ( String typeOfTube) {
        switch(typeOfTube) {
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
                directIntent.putExtra(EXTRA_MESSAGE, "");
                startActivity(directIntent);
                break;
            case "bended":
                Intent bendedIntent = new Intent(BluetoothMessengerActivity.this, BendedTube.class);
                bendedIntent.putExtra(EXTRA_MESSAGE, "");
                startActivity(bendedIntent);
                break;
        }
    }
}
