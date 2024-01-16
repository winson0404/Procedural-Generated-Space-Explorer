using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky.Sprites
{
  public class Bullet : Sprite
  {
    private float _timer;
    public float damage_factor = 1;

    public Bullet()
    {
    }
    public Bullet( Vector2 position, Vector2 heading) 
    {
        Position = position;
        Heading = heading;
        Heading.Normalize();
    }
  }
}
