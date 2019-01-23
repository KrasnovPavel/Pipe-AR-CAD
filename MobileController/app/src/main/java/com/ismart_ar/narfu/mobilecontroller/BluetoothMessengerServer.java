package com.ismart_ar.narfu.mobilecontroller;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.util.Log;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;


/**
 * Класс, реализующий обмен сообщениями по bluetooth.
 * ВНИМАНИЕ: Использование не-ASCII символов не поддерживается.
 * Пример использования: https://github.com/KrasnovPavel/AndroidBluetoothChatServer
 */
public abstract class BluetoothMessengerServer {
    protected BluetoothServerSocket serverSocket;
    protected OutputStream outputStream;
    protected InputStream inputStream;
    protected String name = BluetoothAdapter.getDefaultAdapter().getName();
    protected BluetoothSocket socket;
    public boolean lookingStarted = false;
    protected boolean listeningStarted = false;

    /**
     * UUID сервиса Bluetooth, второе устройство должно иметь такой же UUID.
     */
    public final UUID uuid = UUID.fromString("34B1CF4D-1069-4AD6-89B6-E161D79BE4D8");

    /**
     * Конструктор. Выбрасывает иключениме, если возникли проблемы с Bluetooth,
     * например если Bluetooth выключен.
     * @throws IOException
     */
    public BluetoothMessengerServer () throws IOException {
        BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        serverSocket = mBluetoothAdapter.listenUsingRfcommWithServiceRecord(name, uuid);
    }

    /**
     * Функция обрабатывающая полученное сообщение.
     * @param message сообщение
     */
    public abstract void MessageReceived(String message);


    /**
     * Функция обрабатывающая разрыв соединиения с другим утсройством.
     */
    public void ConnectionClosed() {
        //StartServer();
    }

    /**
     * Функция запускающая ожидание Bluetooth-клиента.
     */
    public void StartServer() {
        if (lookingStarted) return;

        lookingStarted = true;
        CompletableFuture.runAsync(this::LookingForClient);

        Log.i("","start");
    }

    /**
     * Функция отправляющаяя сообщение на подключенное Bluetooth-устройство.
     * @param message сообщение
     */
    public void SendMessage(String message) {
        byte[] length = {(byte)message.length()};
        byte[] str = message.getBytes();
        try {
            outputStream.write(length);
            outputStream.write(str);
            outputStream.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    /**
     * Функция обрабатывающая подключение Bluetooth-устройства.
     */
    public void ClientConnected() {
        try {
            outputStream = socket.getOutputStream();
            inputStream = socket.getInputStream();
            CompletableFuture.runAsync(this::ListenSocket);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    protected void LookingForClient() {
        while (true) {
            try {
                socket = serverSocket.accept();
                if (socket != null) {
                    ClientConnected();
                    lookingStarted = false;
                    return;
                }
            } catch (IOException e) {
                lookingStarted = false;
                return;
            }
        }
    }

    protected void ListenSocket() {
        while (true) {
            try {
                byte[] c = new byte[2048];
                int size = inputStream.read(c);
                String s = new String(c, 0, size);
                MessageReceived(s);
            } catch (Exception e) {
                ConnectionClosed();
                return;
            }
        }
    }
}
