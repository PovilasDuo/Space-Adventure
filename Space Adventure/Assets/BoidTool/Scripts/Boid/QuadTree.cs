using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private int capacity;
    private bool divided;
    private Rect boundary;
    private HashSet<Boid> boids;
    private QuadTree northeast, northwest, southeast, southwest;

    private static QuadTree instance;

    private QuadTree(Rect boundary, int capacity)
    {
        this.boundary = boundary;
        this.capacity = capacity;
        boids = new HashSet<Boid>();
        divided = false;
    }

    public static QuadTree GetInstance(Rect boundary, int capacity)
    {
        if (instance == null)
        {
            instance = new QuadTree(boundary, capacity);
        }
        return instance;
    }

    public static QuadTree GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("QuadTree instance has not been initialized. Call GetInstance(Rect boundary, int capacity) first.");
        }
        return instance;
    }


    public void Insert(Boid boid)
    {
        if (boid == null)
        {
            return;
        }

        if (!boundary.Contains(boid.transform.position))
        {
            return;
        }

        if (boids.Count < capacity)
        {
            boids.Add(boid);
        }
        else
        {
            if (!divided)
            {
                Subdivide();
            }
            northeast.Insert(boid);
            northwest.Insert(boid);
            southeast.Insert(boid);
            southwest.Insert(boid);
        }
    }

    private void Subdivide()
    {
        float x = boundary.x;
        float y = boundary.y;
        float w = boundary.width / 2;
        float h = boundary.height / 2;

        northeast = new QuadTree(new Rect(x + w, y, w, h), capacity);
        northwest = new QuadTree(new Rect(x, y, w, h), capacity);
        southeast = new QuadTree(new Rect(x + w, y + h, w, h), capacity);
        southwest = new QuadTree(new Rect(x, y + h, w, h), capacity);
        divided = true;
    }

    public HashSet<Boid> Query(Boid queryingBoid, float range)
    {
        HashSet<Boid> found = new HashSet<Boid>();
        Vector3 point = queryingBoid.transform.position;
        Rect rangeRect = new Rect(point.x - range, point.y - range, range * 2, range * 2);

        if (!boundary.Overlaps(rangeRect))
        {
            return found;
        }

        foreach (var interimBoid in boids)
        {
            if (queryingBoid != null && interimBoid != null)
            {
                if (Vector3.Distance(interimBoid.transform.position, point) <= range && interimBoid != queryingBoid)
                {
                    if (interimBoid != null)
                    {
                        found.Add(interimBoid);
                    }
                }
            }
        }

        if (divided)
        {
            found.UnionWith(northeast.Query(queryingBoid, range));
            found.UnionWith(northwest.Query(queryingBoid, range));
            found.UnionWith(southeast.Query(queryingBoid, range));
            found.UnionWith(southwest.Query(queryingBoid, range));
        }

        return found;
    }

    public void Clear()
    {
        boids.Clear();
        if (divided)
        {
            northeast.Clear();
            northwest.Clear();
            southeast.Clear();
            southwest.Clear();
            northeast = northwest = southeast = southwest = null;
            divided = false;
        }
    }

    public void Remove(Boid boid)
    {
        boids.Remove(boid);

        if (divided)
        {
            northeast.Remove(boid);
            northwest.Remove(boid);
            southeast.Remove(boid);
            southwest.Remove(boid);
        }
    }
}
