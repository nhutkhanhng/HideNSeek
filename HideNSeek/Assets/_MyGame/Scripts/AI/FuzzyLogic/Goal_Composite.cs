using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RavenAI
{

    public abstract class Goal_Composite<T> : Goal<T>
    {
        ~Goal_Composite()
        {

        }
        public class SubGoalList : List<Goal<T>>
        {

        }
        protected SubGoalList m_SubGoals = new SubGoalList();
        protected int ProcessSubgoals()
        {
            //remove all completed and failed goals from the front of the subgoal list
            while (!m_SubGoals.empty() &&
                   (m_SubGoals[0].isComplete() || m_SubGoals[0].hasFailed()))
            {
                m_SubGoals[0].Terminate();
                // Destroy it if u want or GC collect;

                m_SubGoals.RemoveAt(0);
            }

            //if any subgoals remain, process the one at the front of the list
            if (!m_SubGoals.empty())
            {
                //grab the status of the front-most subgoal
                int StatusOfSubGoals = m_SubGoals[0].Process();

                //we have to test for the special case where the front-most subgoal
                //reports 'completed' *and* the subgoal list contains additional goals.When
                //this is the case, to ensure the parent keeps processing its subgoal list
                //we must return the 'active' status.
                if (StatusOfSubGoals == (int)_Goal.Completed && m_SubGoals.Count > 1)
                {
                    return (int)_Goal.Active;
                }

                return StatusOfSubGoals;
            }

            //no more subgoals to process - return 'completed'
            else
            {
                return (int)_Goal.Completed;
            }
        }
        //passes the message to the front-most subgoal
        protected bool ForwardMessageToFrontMostSubgoal(in Telegram msg)
        {
            if (!m_SubGoals.Available())
            {
                return m_SubGoals[0].HandleMessage(msg);
            }

            //return false if the message has not been handled
            return false;
        }

        //if a child class of Goal_Composite does not define a message handler
        //the default behavior is to forward the message to the front-most
        //subgoal
        public override bool HandleMessage(in Telegram msg)
        {
            return ForwardMessageToFrontMostSubgoal(msg);
        }

        //adds a subgoal to the front of the subgoal list
        public void AddSubgoal(Goal<T> g) { m_SubGoals.Insert(0, g); }

        //this method iterates through the subgoals and calls each one's Terminate
        //method before deleting the subgoal and removing it from the subgoal list
        public void RemoveAllSubgoals()
        {
            for (int i = 0; i < m_SubGoals.Count; i++)
            {
                m_SubGoals[i].Terminate();

                // Destroy it if u want
            }

            m_SubGoals.Clear();
        }
    }
}