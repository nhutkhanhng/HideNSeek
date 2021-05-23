using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lightbug.CharacterControllerPro.Core;


namespace RavenAI
{
    public class GoalEvaluators : List<Goal_Evaluator> { }

    public class Goal_Think : Goal_Composite<CharacterActor>
    {

        internal double RandInRange(double x, double y)
        {
            return x + UnityEngine.Random.Range(0, 1) * (y - x);

            // return (double)UnityEngine.Random.Range((float)x, y);
        }

        private GoalEvaluators m_Evaluators;

        public Goal_Think(CharacterActor pBot)
        {
            //these biases could be loaded in from a script on a per bot basis
            //but for now we'll just give them some random values
            const double LowRangeOfBias = 0.5;
            const double HighRangeOfBias = 1.5;

            double HealthBias = RandInRange(LowRangeOfBias, HighRangeOfBias);
            double ShotgunBias = RandInRange(LowRangeOfBias, HighRangeOfBias);
            double RocketLauncherBias = RandInRange(LowRangeOfBias, HighRangeOfBias);
            double RailgunBias = RandInRange(LowRangeOfBias, HighRangeOfBias);
            double ExploreBias = RandInRange(LowRangeOfBias, HighRangeOfBias);
            double AttackBias = RandInRange(LowRangeOfBias, HighRangeOfBias);

            //create the evaluator objects
            // m_Evaluators.Add(new GetHealthGoal_Evaluator(HealthBias));
        }

        //this method iterates through each goal evaluator and selects the one
        //that has the highest score as the current goal
        public void Arbitrate()
        {
            double best = 0;
            Goal_Evaluator MostDesirable = null;

            foreach (var curDes in m_Evaluators)
            {
                double desirabilty = curDes.CalculateDesirability(m_pOwner);

                if (desirabilty >= best)
                {
                    best = desirabilty;
                    MostDesirable = curDes;
                }
            }

            if (MostDesirable != null)
                MostDesirable.SetGoal(m_pOwner);
        }

        //returns true if the given goal is not at the front of the subgoal list
        public bool notPresent(uint GoalType)
        {
            if (!m_SubGoals.empty())
            {
                return m_SubGoals[0].GetiType() != GoalType;
            }

            return true;
        }

        //top level goal types
        //void AddGoal_MoveToPosition(Vector2D pos);
        //void AddGoal_GetItem(unsigned int ItemType);
        //void AddGoal_Explore();
        //void AddGoal_AttackTarget();

        ////this adds the MoveToPosition goal to the *back* of the subgoal list.
        //void QueueGoal_MoveToPosition(Vector2D pos);


        public override void Activate()
        {

        }

        public override int Process()
        {
            ActivateIfInactive();

            int SubgoalStatus = ProcessSubgoals();

            if (SubgoalStatus == (int)_Goal.Completed || SubgoalStatus == (int)_Goal.Failed)
            {
                //if (!m_pOwner.isPossessed())
                //{
                //    m_iStatus = inactive;
                //}
            }

            return (int)m_iStatus;
        }

        public override void Terminate()
        {

        }
    };
}