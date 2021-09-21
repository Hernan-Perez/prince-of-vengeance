using UnityEngine;
using System.Collections;

public class SkillT5Fury : Skill 
{
	int hpInicio;
	float ultimoUpdate;

	public SkillT5Fury() : base()
	{
		tier = 5;
		mod2 = 0.025f; //% de daño aumentado por cada 1% de vida perdido (+ aumento dmg base). TOTAL 300% del daño base
		mod1 = 0.5f;	//50% aumento dmg BASE
		tiempoFase = 20.0f; //duracion de la skill
		persistente = true;
		cooldown = 90.0f;
		codigo = 9;
        

        if (CONFIG.idioma == 0)
        {
            _nombre = "Susurro De La Muerte";
            _descripcion = "Durante 10 segundos el jugador\nno puede morir.\nSe cura 25% cuando termina el efecto.\nEnfriamiento: 90 seg.";
        }
        else
        {
            _nombre = "Death's Whisper";
            _descripcion = "For 10 seconds you can not die.\nYou are healed 25% of your maximum hp\nwhen the effect ends.\nCooldown: 90 sec.";
        }
    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		if (enabled)
			return 0;
		enabled = true;

        refGame.refControl.PlayEfectos(7, 1f);
        refGame.player.noMorir = true;
        refGame.player.filtro = Color.black;
        //refGame.player.ganarVida((int)(refGame.player.getHpMax() * 0.25f));

        currentTexSkill = 0;
        ultTiempoSkill = Game.TiempoTranscurrido;
		refGame.player.CambiarEstado(EntidadCombate.estado.idle);	//como es buff no muestra animacion de ataque

        base.Accion(dmgMin, dmgMax, refGame);

		return 0; //esta habilidad no pega
	}
	
	public override bool Draw(Game refGame)
	{
		if (!enabled)
			return false;

        if (Game.TiempoTranscurrido - ultTiempoSkill > 10f)
        {
            refGame.refControl.StopEfectos();
            enabled = false;
            refGame.player.noMorir = false;
            refGame.player.filtro = Color.white;
            refGame.player.ganarVida((int)(refGame.player.getHpMax() * 0.25f));
            
        }
		return true;
	}
	
}
