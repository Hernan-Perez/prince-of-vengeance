using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_INFO : NPC
{
    protected Texture2D globoo;

    public NPC_INFO() : base()
    {
        //constructor default
    }

    public NPC_INFO(int posX, int posY, Texture2D spr = null, Texture2D globo = null) : base(spr, posX, posY, true) //nota: le paso al base que si se mueve sino el otro constructor llama a setsolido si o si
    {
        globoo = globo;
    }

    public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (_sprite == null)
            return;

        int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
        int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
        if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
        {
            GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), _sprite);
        }
    }

    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (!_conversacion || globoo == null)
            return;

        int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
        int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y + 1) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
        if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
        {
            GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), globoo);
        }
    }

    public override void EjecutarAccionAI()
    {
        
    }

    public override void Actualizar()
    {
        
    }
}
