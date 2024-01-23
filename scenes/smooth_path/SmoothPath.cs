using Godot;

namespace Vertigo
{
    [Tool]
    public class SmoothPath : Path2D
    {
        [Export] float splineLength = 100;
        [Export] bool _smooth { get { return false; } set { Smooth(value); } }
        [Export] bool _straighten { get { return false; } set { Straighten(value); } }
        
        public void Straighten(bool value)
        {  
            if (!value)
            {
                return;
            }
                
            for(int i = 0; i < Curve.GetPointCount(); i++)
            {
                Curve.SetPointIn(i, new Vector2());
                Curve.SetPointOut(i, new Vector2());
            }
        }
        
        public void Smooth(bool value)
        {  
            if(!value)
                return;
        
            for(int i = 0; i < Curve.GetPointCount(); i++)
            {
                var spline = _GetSpline(i);
                Curve.SetPointIn(i, -spline);
                Curve.SetPointOut(i, spline);
            }
        }
        
        public Vector2 _GetSpline(int i)
        {  
            var lastPoint = _GetPoint(i - 1);
            var nextPoint = _GetPoint(i + 1);
            var spline = lastPoint.DirectionTo(nextPoint) * splineLength;
            return spline;
        }
        
        public Vector2 _GetPoint(int i)
        {  
            var pointCount = Curve.GetPointCount();
            i = Mathf.Wrap(i, 0, pointCount - 1);
            return Curve.GetPointPosition(i);
        }
        
        public override void _Draw()
        {  
            var points = Curve.GetBakedPoints();
            if (points.Length > 0)
            {
                DrawPolyline(points, new Color(1, 1, 1, 1), 2, false);
            }
        }
    }
}