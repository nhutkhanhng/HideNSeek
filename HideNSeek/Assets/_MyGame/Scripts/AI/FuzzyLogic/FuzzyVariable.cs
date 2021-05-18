using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberSets : Dictionary<string, FuzzySet>
{ }

public class FuzzyVariable
{

    private

  //a map of the fuzzy sets that comprise this variable
  MemberSets m_MemberSets;

    //the minimum and maximum value of the range of this variable
    double m_dMinRange;
    double m_dMaxRange;


    //this method is called with the upper and lower bound of a set each time a
    //new set is added to adjust the upper and lower range values accordingly
    void AdjustRangeToFit(double minBound, double maxBound)
    {
        if (minBound < m_dMinRange) m_dMinRange = minBound;
        if (maxBound > m_dMaxRange) m_dMaxRange = maxBound;
    }

    public FuzzyVariable() { m_dMinRange = 0.0; m_dMaxRange = 0.0; }

    //the following methods create instances of the sets named in the method
    //name and add them to the member set map. Each time a set of any type is
    //added the m_dMinRange and m_dMaxRange are adjusted accordingly. All of the
    //methods return a proxy class representing the newly created instance. This
    //proxy set can be used as an operand when creating the rule base.
    public FzSet AddLeftShoulderSet(string name, double minBound, double peak, double maxBound)
    {
        return new FzSet();
    }

    public FzSet AddRightShoulderSet(string name, double minBound, double peak, double maxBound)
    {
        return new FzSet();
    }

    public FzSet AddTriangularSet(string name,
                               double minBound,
                               double peak,
                               double maxBound)
    {
        return new FzSet();
    }

    public FzSet AddSingletonSet(string name,
                              double minBound,
                              double peak,
                              double maxBound)
    {
        return new FzSet();
    }


    //fuzzify a value by calculating its DOM in each of this variable's subsets
    public void Fuzzify(float val)
    {
        
        foreach (var curSet in m_MemberSets)
        {
            curSet.Value.SetDOM(curSet.Value.CalculateDOM(val));
        }
    }

    //defuzzify the variable using the max average method
    public double DeFuzzifyMaxAv()
    {
        double bottom = 0.0;
        double top = 0.0;

    
        foreach (var curSet in m_MemberSets)
        {
            bottom += curSet.Value.GetDOM();
            top += curSet.Value.GetRepresentativeVal() * curSet.Value.GetDOM();
        }

        if (bottom.Equals(0))
        {
            return 0;
        }

        return top / bottom;
    }

    //defuzzify the variable using the centroid method
    public double DeFuzzifyCentroid(int NumSamples)
    {
        //calculate the step size
        double StepSize = (m_dMaxRange - m_dMinRange) / (double)NumSamples;

        double TotalArea = 0.0;
        double SumOfMoments = 0.0;

        //step through the range of this variable in increments equal to StepSize
        //adding up the contribution (lower of CalculateDOM or the actual DOM of this
        //variable's fuzzified value) for each subset. This gives an approximation of
        //the total area of the fuzzy manifold.(This is similar to how the area under
        //a curve is calculated using calculus... the heights of lots of 'slices' are
        //summed to give the total area.)
        //
        //in addition the moment of each slice is calculated and summed. Dividing
        //the total area by the sum of the moments gives the centroid. (Just like
        //calculating the center of mass of an object)
        for (int samp = 1; samp <= NumSamples; ++samp)
        {
            //for each set get the contribution to the area. This is the lower of the 
            //value returned from CalculateDOM or the actual DOM of the fuzzified 
            //value itself               
            foreach (var curSet in m_MemberSets)
            {
                double contribution =
                    Mathf.Min(curSet.Value.CalculateDOM((float)(m_dMinRange + samp * StepSize)),
                          curSet.Value.GetDOM());

                TotalArea += contribution;

                SumOfMoments += (m_dMinRange + samp * StepSize) * contribution;
            }
        }

        //make sure total area is not equal to zero
        if (Mathf.Abs((float)TotalArea) <= Mathf.Epsilon) return 0.0;

        return (SumOfMoments / TotalArea);
    }
};


