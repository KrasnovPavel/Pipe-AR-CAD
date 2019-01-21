package com.ismart_ar.narfu.mobilecontroller;

import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class Empty extends BluetoothMessengerActivity {

    Button createTube;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.empty);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);

        createTube = findViewById(R.id.createTube);
    }

    public void onCreateTube(View view) {
        messenger.SendMessage("{\"Button\" : \"" + createTube.getText().toString() + "\"}");
    }
}
