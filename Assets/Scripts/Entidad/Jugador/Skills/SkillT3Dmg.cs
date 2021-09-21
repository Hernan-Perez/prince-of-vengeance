using UnityEngine;
using System.Collections;

public class SkillT3Dmg : Skill 
{
	public SkillT3Dmg() : base()
	{
		tier = 3;
		mod2 = 0;
		mod1 = 0.25f;	//25% aumento dmg
		tiempoFase = 10.0f; //duracion de la skill
		persistente = true;
		cooldown = 45.0f;
		codigo = 5;
        

        if (CONFIG.idioma == 0)
        {
            _nombre = "Ansia De Sangre";
            _descripcion = "Aumenta el daño total un 25%\ndurante 10 segundos.\nEnfriamiento: 45 seg.";
        }
        else
        {
            _nombre = "Urge for blood";
            _descripcion = "Increases your damage by 25%\nfor 10 seconds.\nCooldown: 45 sec.";
        }

    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		if (enabled)
			return 0;
		base.Accion(dmgMin,dmgMax,refGame);
		enabled = true;
        refGame.refControl.PlaySonido(3);
        currentTexSkill = 0;
		ultTiempoSkill = Game.TiempoTranscurrido;
		refGame.player.CambiarEstado(EntidadCombate.estado.idle);	//como es buff no muestra animacion de ataque
		refGame.player.modificadorDmg += mod1;
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
                refGame.player.modificadorDmg -= mod1;
				//refGame.player.CambiarEstado(EntidadCombate.estado.idle);
				return false;
			}
		}

		GUI.DrawTexture (new Rect (Screen.width/2  - CONFIG.TAM/2 /*- refGame.player.microPos.x*/, Screen.height/2 - CONFIG.TAM/2/* + refGame.player.microPos.y*/, CONFIG.TAM, CONFIG.TAM), effectSkill[currentTexSkill]);
		return true;
	}

}
