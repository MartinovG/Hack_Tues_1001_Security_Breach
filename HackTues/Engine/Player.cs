using HackTues.Controls;
using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public abstract class Player: IPlayer {
    protected abstract Vector2 Friction { get; }
    public Layer Layer { get; set; }
    public Hitbox Hitbox { get; set; } = new Hitbox(new(0, .01f), new(1, 1.99f));
    public Vector2 Position => pos;
    public Vector2 Velocity => vel;

    protected Vector2 pos, vel;


    protected abstract Vector2 GetAcceleration(float detla, IController? controller);
    protected virtual void HitX() {
        vel.X = 0;
    }
    protected virtual void HitY() {
        vel.Y = 0;
    }

    public virtual void Update(float delta, ICollider environment, IController? controller) {
        var acc = GetAcceleration(delta, controller);

        vel += acc * delta;
        vel *= new Vector2(MathF.Pow(Friction.X, delta), MathF.Pow(Friction.Y, delta));

        pos.X += vel.X * delta;
        if (environment.CollidesWith(Hitbox + pos)) {
            pos.X -= vel.X * delta;
            HitX();
        }

        pos.Y += vel.Y * delta;
        if (environment.CollidesWith(Hitbox + pos)) {
            pos.Y -= vel.Y * delta;
            HitY();
        }
    }

    public void AddLayers(SortedSet<Layer> layers) {
        layers.Add(Layer + Position);
    }

    public Player(Layer layer) {
        this.Layer = layer;
    }
}
