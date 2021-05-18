using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FzSet : FuzzyTerm
{
    //a reference to the fuzzy set this proxy represents
    private FuzzySet m_Set;

    public override FuzzyTerm Clone()
    {
        FzSet _new = this.MemberwiseClone() as FzSet;

        _new.m_Set = this.m_Set;

        return _new;
    }

    public override float GetDOM()
    {
        return m_Set.GetDOM();
    }

    public override void ClearDOM()
    {
        m_Set.ClearDOM();
    }

    public override void ORwithDOM(float val)
    {
        m_Set.ORwithDOM(val);
    }
};
