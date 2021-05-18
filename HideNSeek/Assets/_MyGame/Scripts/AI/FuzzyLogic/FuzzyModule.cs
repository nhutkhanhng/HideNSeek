using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VarMap : Dictionary<string, FuzzyVariable>
{

}
class FuzzyModule
{
    //you must pass one of these values to the defuzzify method. This module
    //only supports the MaxAv and centroid methods.
    public enum DefuzzifyMethod { max_av, centroid };



    //when calculating the centroid of the fuzzy manifold this value is used
    //to determine how many cross-sections should be sampled
    enum NumSamples { NumSamples = 15 };

    //a map of all the fuzzy variables this module uses
    private VarMap m_Variables;

    


    //a vector containing all the fuzzy rules
    protected List<FuzzyRule> m_Rules;


    //zeros the DOMs of the consequents of each rule. Used by Defuzzify()
    public void SetConfidencesOfConsequentsToZero()
    {
        foreach (var curRule in m_Rules)
        {
            curRule.SetConfidenceOfConsequentToZero();
        }
    }

    //creates a new 'empty' fuzzy variable and returns a reference to it.
    FuzzyVariable CreateFLV(string VarName)
    {
        m_Variables[VarName] = new FuzzyVariable(); ;

        return m_Variables[VarName];
    }

    //adds a rule to the module
    public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
    {
        m_Rules.Add(new FuzzyRule(antecedent, consequence));
    }

    //----------------------------- Fuzzify ---------------------------------------
    //
    //  this method calls the Fuzzify method of the variable with the same name
    //  as the key
    //-----------------------------------------------------------------------------
    public void Fuzzify(string NameOfFLV, float val)
{
    m_Variables[NameOfFLV].Fuzzify(val);
}

    //---------------------------- DeFuzzify --------------------------------------
    //
    //  given a fuzzy variable and a deffuzification method this returns a 
    //  crisp value
    //-----------------------------------------------------------------------------
    public float    DeFuzzify(string NameOfFLV, DefuzzifyMethod method)
{
    //clear the DOMs of all the consequents of all the rules
    SetConfidencesOfConsequentsToZero();

        foreach (var curRule in m_Rules)
        {
            curRule.Calculate();
        }

    //now defuzzify the resultant conclusion using the specified method
    switch (method)
    {
        case DefuzzifyMethod.centroid:

            return (float)m_Variables[NameOfFLV].DeFuzzifyCentroid((int)NumSamples.NumSamples);

            break;

        case DefuzzifyMethod.max_av:

            return (float)m_Variables[NameOfFLV].DeFuzzifyMaxAv();

            break;
    }

    return 0;
}


};

///////////////////////////////////////////////////////////////////////////////



