using UnityEngine;
public abstract class Puzzle
{
    public static Game refGame;
    public static ControlScript refControl;
    public int cod;
    public Puzzle()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Draw()
    {

    }

    public virtual bool Touch(Vector2 pos)
    {

        return false;
    }
}
