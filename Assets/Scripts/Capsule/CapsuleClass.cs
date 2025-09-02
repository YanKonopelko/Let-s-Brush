using System.Collections.Generic;
using UnityEngine;
    public struct Square
    {
        public PointInQuadrilateral.Point LU;
        public PointInQuadrilateral.Point RU;
        public PointInQuadrilateral.Point LD;
        public PointInQuadrilateral.Point RD;
        // a     b
        // c     d
        public Square(PointInQuadrilateral.Point lu, PointInQuadrilateral.Point ru, PointInQuadrilateral.Point ld, PointInQuadrilateral.Point rd)
        {
            LU = lu;
            RU = ru;
            LD = ld;
            RD = rd;
        }
    }
public class CapsuleClass : MonoBehaviour
{
    [SerializeField] private GameObject ParticleSystemPrefab;
    [SerializeField] private AudioSource AudioSource;

    public void ParticlePlay()
    {
        var ps = LevelManager.instance.pool.GetFromPool(ParticleSystemPrefab);
        var psComponent = ps.GetComponent<CapsuleParticle>();
        psComponent.Init(ParticleSystemPrefab);
        ps.transform.SetParent(transform);
        ps.transform.localPosition = new Vector3(0, 0.5f, 0);
        psComponent.Play();
    
        // ParticleSystem.Play();
        // AudioSource.Play();
    }

    public List<Vector2> pixelsPositions;
    public List<bool> outPixelPainted;

    public Vector2 brusherPosition;
    public Vector2 brusherSize;
    public Vector2 pixelSize;
    public Square square;
    public PointInQuadrilateral.Point circle1;
    public float radius;
    public PointInQuadrilateral.Point circle2;
    // public float angle;


}

public class PointInQuadrilateral
{
    public static bool IsPointInsideCircle(Point circleCentre, float radius, Point point)
    {
        // Вычисляем расстояние от центра окружности до точки
        double distanceSquared = Mathf.Pow((float)(point.X - circleCentre.X), 2) + Mathf.Pow((float)(point.Y - circleCentre.Y), 2);
        double radiusSquared = Mathf.Pow(radius, 2);

        // Если квадрат расстояния меньше квадрата радиуса, то точка внутри окружности
        return distanceSquared <= radiusSquared;
    }
    // Структура для представления точки
    public struct Point
    {
        public double X;
        public double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    // Метод для вычисления векторного произведения двух векторов
    private static double CrossProduct(Point p1, Point p2, Point p3)
    {
        return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
    }

    // Метод для проверки, находится ли точка внутри четырёхугольника
    public static bool IsPointInside(Point p1, Point p2, Point p3, Point p4, Point testPoint)
    {
        // Проверка знаков векторных произведений для каждой из сторон
        double cross1 = CrossProduct(p1, p2, testPoint);
        double cross2 = CrossProduct(p2, p3, testPoint);
        double cross3 = CrossProduct(p3, p4, testPoint);
        double cross4 = CrossProduct(p4, p1, testPoint);

        // Если все произведения имеют одинаковые знаки (все положительные или все отрицательные),
        // то точка внутри четырёхугольника
        bool isPositive = cross1 > 0 && cross2 > 0 && cross3 > 0 && cross4 > 0;
        bool isNegative = cross1 < 0 && cross2 < 0 && cross3 < 0 && cross4 < 0;

        return isPositive || isNegative;
    }


}