//------------------------------------------------------------------------
//
//  Name:   Telegram.h
//
//  Desc:   This defines a telegram. A telegram is a data structure that
//          records information required to dispatch messages. Messages 
//          are used by game agents to communicate with each other.
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//
//------------------------------------------------------------------------

public struct Telegram
{

    //these telegrams will be stored in a priority queue. Therefore the >
    //operator needs to be overloaded so that the PQ can sort the telegrams
    //by time priority. Note how the times must be smaller than
    //SmallestDelay apart before two Telegrams are considered unique.
    const double SmallestDelay = 0.25;

    //the entity that sent this telegram
    int Sender;

    //the entity that is to receive this telegram
    int Receiver;

    //the message itself. These are all enumerated in the file
    //"MessageTypes.h"
    int Msg;

    //messages can be dispatched immediately or delayed for a specified amount
    //of time. If a delay is necessary this field is stamped with the time 
    //the message should be dispatched.
    float DispatchTime;

    //any additional information that may accompany the message
    System.Action ExtraInfo;

    public Telegram(float time,
             int sender,
             int receiver,
             int msg,
             System.Action info = null)
    {
        DispatchTime = time;
        Sender = sender;
        Receiver = receiver;
        Msg = msg;
        ExtraInfo = info;
    }

}

