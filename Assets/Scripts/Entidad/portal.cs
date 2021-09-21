using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//esta clase es unicamente para el grafico del portal en la biblioteca que permite pasar a la isla (y tambien el portal de vuelta en la isla)
//es un npc invisible con el grafico del portal como postdraw

public class Portal : NPC_INFO
{
    protected Texture2D grafico;
    protected Vector2 posPostDraw;
    protected float fade = 0.5f;
    protected bool aumentandoFade = true;
    protected float offset = 0f;

    public Portal() : base()
    {
        //default
    }

    public Portal(bool portalIda) : base(0, 0) //nota: le paso al base que si se mueve sino el otro constructor llama a setsolido si o si
    {
        grafico = refControl.otrasTexturas[47];
        if (portalIda)
        {
            pos = new Vector2(9, 12);
            posPostDraw = new Vector2(10, 9);
        }
        else
        {
            pos = new Vector2(9, 12);
            posPostDraw = new Vector2(26, 13);
        }
        
    }

    public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        return;
    }

    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (aumentandoFade)
        {
            fade += Game.elapsed / 4f;
            if (fade >= 1f)
            {
                fade = 1f;
                aumentandoFade = false;
            }
        }
        else
        {
            fade -= Game.elapsed / 4f;
            if (fade <= 0.5f)
            {
                fade = 0.5f;
                aumentandoFade = true;
            }
        }

        offset += Game.elapsed * 10f;
        int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+posPostDraw.x - posPlayer.x) * CONFIG.TAM- microPosPlayer.x);
        int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(posPostDraw.y) + posPlayer.y) * CONFIG.TAM + microPosPlayer.y);
        if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
        {
            Matrix4x4 matrixBackup = GUI.matrix;
            GUIUtility.RotateAroundPivot(-5.0f * offset, new Vector2(x + CONFIG.TAM, y + CONFIG.TAM));
            //GUI.DrawTexture(new Rect(Screen.width / 2 - CONFIG.TAM * 4.5f, Screen.height / 2 - CONFIG.TAM * 4.5f, CONFIG.TAM * 9f, CONFIG.TAM * 9f), effectSkill[0], ScaleMode.ScaleAndCrop);

            GUI.color = new Color(1f, 1f, 1f, fade);
            GUI.DrawTexture(new Rect(x, y, CONFIG.TAM * 2, CONFIG.TAM * 2), grafico);
            GUI.color = Color.white;
            GUI.matrix = matrixBackup;

            
        }
    }

    public override void EjecutarAccionAI()
    {

    }

    public override void Actualizar()
    {

    }
}
