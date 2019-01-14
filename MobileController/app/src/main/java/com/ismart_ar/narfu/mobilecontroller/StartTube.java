package com.ismart_ar.narfu.mobilecontroller;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class StartTube extends Activity {

    Button btnMove;
    Button btnIncrease;
    Button btnDecrease;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.start_tube);

        Intent startIntent = getIntent();
        String message = startIntent.getStringExtra(MainActivity.EXTRA_MESSAGE);

        btnMove = findViewById(R.id.btnMove);
        btnIncrease = findViewById(R.id.btnIncrease);
        btnDecrease = findViewById(R.id.btnDecrease);
    }

    public void onMove(View view) {
         //Buttons.jsonBtn(btnMove.getText().toString());
    }

    public void onIncrease(View view) {
        //Buttons.jsonBtn(btnIncrease.getText().toString());
    }

    public void onDecrease(View view) {
        //Buttons.jsonBtn(btnDecrease.getText().toString());
    }
}
