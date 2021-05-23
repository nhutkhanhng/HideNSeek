using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RavenAI
{
    public abstract class Goal<T>
    {
        public enum _Goal { Active, Inactive, Completed, Failed };

        //an enumerated type specifying the type of goal
        protected int m_iType;
        //a pointer to the entity that owns this goal
        protected T m_pOwner;


        //an enumerated value indicating the goal's status (active, inactive,
        //completed, failed)
        protected _Goal m_iStatus;

        /* the following methods were created to factor out some of the commonality
                         in the implementations of the Process method() */

        //if m_iStatus = inactive this method sets it to active and calls Activate()
        protected void ActivateIfInactive()
        {
            if (hasFailed())
            {
                m_iStatus = _Goal.Inactive;
            }
        }

        //if m_iStatus is failed this method sets it to inactive so that the goal
        //will be reactivated (and therefore re-planned) on the next update-step.
        protected void ReactiveIfFailed()
        {
            if (isInactive())
            {
                Activate();
            }
        }


        //logic to run when the goal is activated.
        public abstract void Activate();

        //logic to run each update-step
        public abstract int Process();

        //logic to run when the goal is satisfied. (typically used to switch
        //off any active steering behaviors)
        public abstract void Terminate();

        //goals can handle messages. Many don't though, so this defines a default
        //behavior
        public virtual bool HandleMessage(in Telegram msg)
        {
            return false;
        }

        public virtual bool isComplete() { return m_iStatus == _Goal.Completed; }
        public bool isActive() { return m_iStatus == _Goal.Active; }
        public bool isInactive() { return m_iStatus == _Goal.Inactive; }
        public bool hasFailed() { return m_iStatus == _Goal.Failed; }
        public int GetiType() { return this.m_iType; }
    }
}
