package com.ismart_ar.narfu.mobilecontroller;

import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;
import java.io.IOException;


public class MainActivity extends BluetoothMessengerActivity {

    private final static int REQUEST_ENABLE_BT = 1;
    public final static String EXTRA_MESSAGE = "EXTRA_MESSAGE";
    TextView state;
    BluetoothAdapter btAdapter;
    Button startServers;

    public void startServer(View view) {
        if (messenger == null) {
            try {
                messenger = new MABluetoothMessenger();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }

        try {
            messenger.StartServer();
        } catch (NullPointerException e) {
            Toast toast = Toast.makeText(getApplicationContext(), "Подожди пока включится BT!", Toast.LENGTH_SHORT);
            toast.show();
        }

        if (messenger.lookingStarted) {
            startServers.setEnabled(false);
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);

        state = findViewById(R.id.state);
        startServers = findViewById(R.id.startServers);

        btAdapter = BluetoothAdapter.getDefaultAdapter();
        if (messenger == null) {
            try {
                messenger = new MABluetoothMessenger();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        if (btAdapter.isEnabled()) {
            messenger.StartServer();
        }
        else {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        }
    }

    public void test(View view) {
        messenger.SendMessage("test");
    }
}
