using UnityEngine;
using System.Collections;

public class SkillT3Def : Skill 
{
	public SkillT3Def() : base()
	{
		tier = 3;
		mod2 = 0;
		mod1 = 0.25f;	//25% aumento def
		tiempoFase = 10.0f; //duracion de la skill
		persistente = true;
		cooldown = 45.0f;
		codigo = 4;
        

        if (CONFIG.idioma == 0)
        {
            _nombre = "Piel De Acero";
            _descripcion = "Aumenta la armadura un 25%\ndurante 10 segundos.\nEnfriamiento: 45 seg.";
        }
        else
        {
            _nombre = "Steel skin";
            _descripcion = "Raises your armor by 25%\nfor 10 seconds.\nCooldown: 45 sec.";
        }

    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		if (enabled)
			return 0;
		enabled = true;
        refGame.refControl.PlaySonido(3);
		base.Accion(dmgMin,dmgMax,refGame);
		currentTexSkill = 0;
		ultTiempoSkill = Game.TiempoTranscurrido;
		refGame.player.CambiarEstado(EntidadCombate.estado.idle);	//como es buff no muestra animacion de ataque
		refGame.player.modificadorDef += mod1;
		return 0; //esta habilidad no pega
	}
	
	public override bool Draw(Game refGame)
	{
		if (!enabled)
			return false;
		
		if (Game.TiempoTranscurrido - ultTiempoSkill >= tiempoFase)
		{
			currentTexSkill++;
			if (currentTexSkill >= effectSkill.Length)
			{
				enabled = false;
                refGame.refControl.PlaySonido(4);
                refGame.player.modificadorDef -= mod1;
				//refGame.player.CambiarEstado(EntidadCombate.estado.idle);
				return false;
			}
		}
		
		GUI.DrawTexture (new Rect (Screen.width/2  - CONFIG.TAM/2 /*- refGame.player.microPos.x*/, Screen.height/2 - CONFIG.TAM/2/* + refGame.player.microPos.y*/, CONFIG.TAM, CONFIG.TAM), effectSkill[currentTexSkill]);
		return true;
	}
	
}
