using UnityEngine;
using System.Collections;

public class SkillCuracionAll : Skill	//recuperacion veloz, recupera vida cada x intervalos, despues de un determinado tiempo de no recibir daño
{
	private float ultimaCuracion;
	
	public SkillCuracionAll() : base()
	{
		tier = 0;
		mod2 = 0;
		mod1 = 0.1f;	//10% de curacion en cada intervalo
		tiempoFase = 0f;
		cooldown = 10f;	//cada 10 segundos se cura
		ultimaCuracion = 0;
		pasiva = true;
	}
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		if (Game.TiempoTranscurrido - refGame.player.tiempoUltimoGolpeRecibido >= cooldown && Game.TiempoTranscurrido - ultimaCuracion >= cooldown)
		{
			ultimaCuracion = Game.TiempoTranscurrido;
            if (refGame.player.getHp() != 0)
			    refGame.player.ganarVida((int)(refGame.player.getHpMax() * mod1));
		}
		
		return 0;
		
	}
}