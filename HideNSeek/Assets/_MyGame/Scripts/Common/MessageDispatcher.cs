using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDispatcher
{
    public const float SEND_MSG_IMEEDIATELY = 0.0f;
    public const int NO_ADDITIONAL_IFNO = 0;
    public const int SENDER_ID_IRRELEVANT = -1;

    public static SortedSet<Telegram> PriorityQ;


    public void DispatchMessage(float delay, int sender, int receiver, int msg, System.Action ExtraInfo)
    {
        BaseUnit _Receiver = EntityManager.Instance.GetEntity(receiver);

        if (_Receiver == null)
        {

#if SHOW_MESSAGING_INFO
            Debug.LogError($"\nWarning! No Receiver with ID of {receiver} found");
#endif
            return;
        }

        Telegram telegram = new Telegram(0, sender, receiver, msg, ExtraInfo);
        if (delay <= 0.0f)
        {
#if !SHOW_MESSAGING_INFO
            //       Debug.Log($("\nTelegram dispatched at time: " << TickCounter->GetCurrentFrame()
            // " by " << sender << " for " { receiver} 
            //Msg is { msg}) ";
#endif
        }
    }

    public void DispatchDelayedMessages() { }
}
