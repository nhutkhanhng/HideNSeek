using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Regulator
{

    //the time period between updates 
    private float m_dUpdatePeriod;

    //the next time the regulator allows code flow
    private float m_dwNextUpdateTime;

    public Regulator(float NumUpdatesPerSecondRqd)
    {
        m_dwNextUpdateTime = (Time.time + UnityEngine.Random.Range(0.0f, 1f) * 1000);

        if (NumUpdatesPerSecondRqd > 0)
        {
            m_dUpdatePeriod = 1000f / NumUpdatesPerSecondRqd;
        }

        else if (0.0f == NumUpdatesPerSecondRqd)
        {
            m_dUpdatePeriod = 0f;
        }

        else if (NumUpdatesPerSecondRqd < 0)
        {
            m_dUpdatePeriod = -1;
        }
    }


    //returns true if the current time exceeds m_dwNextUpdateTime
    bool isReady()
    {
        //if a regulator is instantiated with a zero freq then it goes into
        //stealth mode (doesn't regulate)
        if (0.0 == m_dUpdatePeriod) return true;

        //if the regulator is instantiated with a negative freq then it will
        //never allow the code to flow
        if (m_dUpdatePeriod < 0) return false;

        float CurrentTime = Time.time;

        //the number of milliseconds the update period can vary per required
        //update-step. This is here to make sure any multiple clients of this class
        //have their updates spread evenly
        const float UpdatePeriodVariator = 10f;

        if (CurrentTime >= m_dwNextUpdateTime)
        {
            m_dwNextUpdateTime = (CurrentTime + m_dUpdatePeriod + UnityEngine.Random.Range(-UpdatePeriodVariator, UpdatePeriodVariator));

            return true;
        }

        return false;
    }
};
