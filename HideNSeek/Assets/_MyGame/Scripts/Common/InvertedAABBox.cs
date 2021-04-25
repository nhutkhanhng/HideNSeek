//-----------------------------------------------------------------------------
//
//  Name:   InvertedAABBox2D.h
//
//  Author: Mat Buckland (www.ai-junkie.com)
//
//  Desc:   v simple inverted (y increases down screen) axis aligned bounding
//          box class
//-----------------------------------------------------------------------------
using UnityEngine;

class InvertedAABBox2D
{

    Vector2 m_vTopLeft;
    Vector2 m_vBottomRight;

    Vector2 m_vCenter;

    public InvertedAABBox2D(Vector2 tl,
                   Vector2 br)
    {
        m_vTopLeft = tl;
        m_vBottomRight = br;

        m_vCenter = (tl + br) / 2f;
    }

    //returns true if the bbox described by other intersects with this one
    bool isOverlappedWith(in InvertedAABBox2D other)
    {
        return !((other.Top() > Bottom()) ||
               (other.Bottom() < Top()) ||
               (other.Left() > Right()) ||
               (other.Right() < Left()));
    }


    Vector2 TopLeft() { return m_vTopLeft; }
    Vector2 BottomRight() { return m_vBottomRight; }

    double Top() { return m_vTopLeft.y; }
    double Left() { return m_vTopLeft.x; }
    double Bottom() { return m_vBottomRight.y; }
    double Right() { return m_vBottomRight.x; }
    Vector2 Center() { return m_vCenter; }
}