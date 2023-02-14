using System.Collections.Generic;
using BehaviorTree;

public class KyleBT : Tree
{
    public UnityEngine.Transform[] waypoints;
    public static float speed = 2f;
    public static float fovRange = 6f;
    public static float hitRange = 1f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckBallInRange(transform),
                new HitBall(transform),
            }),

            new Sequence(new List<Node>
            {
                new CheckBall(transform),
                new ApproachBall(transform),
            }),
            new PatrolCourt(transform, waypoints),
        });

        return root;
    }
}
