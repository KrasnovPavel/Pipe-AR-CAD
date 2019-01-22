package com.ismart_ar.narfu.mobilecontroller;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class StartTube extends BluetoothMessengerActivity {

    Button btnMove;
    Button btnIncrease;
    Button btnDecrease;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.start_tube);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);

        Intent startIntent = getIntent();
        String message = startIntent.getStringExtra(MainActivity.EXTRA_MESSAGE);

        btnMove = findViewById(R.id.btnMove);
        btnIncrease = findViewById(R.id.btnIncrease);
        btnDecrease = findViewById(R.id.btnDecrease);
    }

    public void onMove(View view) {
        messenger.SendMessage("{\"Button\" : \"" + btnMove.getText().toString() + "\"}");
    }

    public void onIncrease(View view) {
        messenger.SendMessage("{\"Button\" : \"" + btnIncrease.getText().toString() + "\"}");
    }

    public void onDecrease(View view) {
        messenger.SendMessage("{\"Button\" : \"" + btnDecrease.getText().toString() + "\"}");
    }
}
