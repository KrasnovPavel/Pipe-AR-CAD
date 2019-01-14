package com.ismart_ar.narfu.mobilecontroller;

import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import com.google.gson.Gson;
import android.widget.SeekBar;
import android.widget.TextView;
import com.triggertrap.seekarc.SeekArc;
import com.triggertrap.seekarc.SeekArc.OnSeekArcChangeListener;


public class BendedTube extends BluetoothMessengerActivity {

    Button changeBended;
    SeekArc mSeekArc;
    TextView seekArcProgress;
    TextView seekBarValue;
    Integer lastProgress = 0;
    Integer lastProgressSeek = 0;
    SeekBar seekBar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
      //  setContentView(R.layout.bended_tube);

        Intent bendedIntent = getIntent();
        String message = bendedIntent.getStringExtra(MainActivity.EXTRA_MESSAGE);

      //  changeBended = findViewById(R.id.changeBended);
      //  mSeekArc = findViewById(R.id.seekArc);
      //  seekArcProgress = findViewById(R.id.seekArcProgress);
        mSeekArc.setClockwise(false);
        mSeekArc.setRotation(90);
        mSeekArc.invalidate();
        mSeekArc.setOnSeekArcChangeListener(new OnSeekArcChangeListener() {

            @Override
            public void onStopTrackingTouch(SeekArc seekArc) {
            }

            @Override
            public void onStartTrackingTouch(SeekArc seekArc) {
            }

            @Override
            public void onProgressChanged(SeekArc seekArc, int progress, boolean fromUser) {
                progress = (int) (progress * 3.6);
                seekArcProgress.setText(String.valueOf(progress));
                if (lastProgress != progress) {
                    JSON.Axies axie = new JSON.Axies("Angle", (double) progress);
                    Gson gson = new Gson();
                    Log.i("JSON", gson.toJson(axie));
                }
                lastProgress = progress;
            }
        });

     //   seekBarValue = findViewById(R.id.seekBarValue);
      //  seekBar = findViewById(R.id.seekBar);
        seekBar.setProgress(0);
        seekBar.incrementProgressBy(15);
        seekBar.setProgressTintList(ColorStateList.valueOf(Color.RED));
        seekBar.setThumbTintList(ColorStateList.valueOf(Color.RED));
        seekBar.setMax(360);
        seekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener(){

            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                progress = progress / 15;
                progress = progress * 15;
                seekBarValue.setText(String.valueOf(progress));
                if (lastProgressSeek != progress) {
                    JSON.Axies axie = new JSON.Axies("Angle", (double) progress);
                    Gson gson = new Gson();
                    Log.i("JSON", gson.toJson(axie));
                }
                lastProgressSeek = progress;
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });
    }

    public void changeBended(View view) {
      //  messenger.SendMessage("{ Button :" + changeBended.getText().toString() + " }");
    }
}
