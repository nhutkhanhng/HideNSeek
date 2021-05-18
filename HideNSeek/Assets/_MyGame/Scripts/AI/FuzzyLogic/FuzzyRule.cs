using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class FuzzyRule
{
    //antecedent (usually a composite of several fuzzy sets and operators)
    private readonly FuzzyTerm m_pAntecedent;
    
    //consequence (usually a single fuzzy set, but can be several ANDed together)
    private FuzzyTerm m_pConsequence;

    public

  FuzzyRule(in FuzzyTerm ant,
            in FuzzyTerm con)
    {
        m_pAntecedent = ant.Clone();
        m_pConsequence = con.Clone();
    }    

    public void SetConfidenceOfConsequentToZero() { m_pConsequence.ClearDOM(); }

    //this method updates the DOM (the confidence) of the consequent term with
    //the DOM of the antecedent term. 
    public void Calculate()
    {
        m_pConsequence.ORwithDOM(m_pAntecedent.GetDOM());
    }
};
