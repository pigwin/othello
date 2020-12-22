﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class Title : MonoBehaviour
{
    [SerializeField] List<TMP_Text> player_text;
    int AInum = 0;
    private static Socket listener;
    public void Player1Button()
    {
        Manager.player[0] = Manager.PLAYTYPE.UsingHand;
    }
    public void Player2Button()
    {
        Manager.player[1] = Manager.PLAYTYPE.UsingHand;
    }
    void HowPlay(int turn)
    {
        switch (Manager.player[turn])
        {
            case Manager.PLAYTYPE.UsingAI:
                player_text[turn].text = "AI";
                break;
            case Manager.PLAYTYPE.UsingHand:
                player_text[turn].text = "Hand";
                break;
            case Manager.PLAYTYPE.Empty:
                player_text[turn].text = "";
                break;
        }
    }
    public async static void SocketServer()
    {
        byte[] bytes = new byte[1024];
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[3];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
        Debug.Log(ipAddress);
        listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(2);
        Manager.handler = new Socket[2];
        Manager.handler[0] = await listener.AcceptAsync();
        Manager.handler[1] = await listener.AcceptAsync();
    }
    private void Start()
    {
        SocketServer();
    }
    // Update is called once per frame
    void Update()
    {
        //どちらも手動であればplayer1->黒 player2->白
        //少なくとも一方がAIであればランダムに順番が選ばれる。
        if (Manager.handler[0] != null && AInum==0)
        {
            AInum++;
            if(Manager.player[0]== Manager.PLAYTYPE.Empty)
            {
                Manager.player[0] = Manager.PLAYTYPE.UsingAI;
            }
            else
            {
                Manager.player[1] = Manager.PLAYTYPE.UsingAI;
            }
        }
        if(Manager.handler[1] != null && AInum==1)
        {
            AInum++;
            if (Manager.player[1] == Manager.PLAYTYPE.Empty)
            {
                Manager.player[1] = Manager.PLAYTYPE.UsingAI;
            }
        }
        HowPlay(0);
        HowPlay(1);
        if(Manager.player[0] != Manager.PLAYTYPE.Empty && Manager.player[1] != Manager.PLAYTYPE.Empty)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
