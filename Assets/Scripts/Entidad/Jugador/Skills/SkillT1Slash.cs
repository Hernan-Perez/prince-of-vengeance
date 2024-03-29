﻿using UnityEngine;
using System.Collections;

public sealed class SkillT1Slash : Skill
{
	public SkillT1Slash() : base()
	{
		tier = 1;
		mod2 = 0;
		mod1 = 1.0f;	//100% del daño base
		tiempoFase = 0.05f;
		cooldown = 1f;
		codigo = 1;

        if (CONFIG.idioma == 0)
        {
            _nombre = "Golpe Frontal";
            _descripcion = "Golpe simple con poco enfriamiento\nque daña a los enemigos\nque tenga en frente.\nRealiza 100% de daño.\nEnfriamiento: 1 seg.";
        }
        else
        {
            _nombre = "Frontal strike";
            _descripcion = "Simple blow with low cooldown\nthat damages the enemies in front of you.\n Hits 100% damage.\nCooldown: 1 sec.";
        }

       
	}

	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		//recorrer array a ver si hay enemigos cerca, que le pegue a TODOS los enemigos cerca, no solo al primero que encuentre. Distancia CONO
		if (enabled)
			return 0;
        refGame.refControl.PlaySonido(2, 0.5f);
        base.Accion(dmgMin,dmgMax,refGame);
		refGame.player.CambiarEstado(EntidadCombate.estado.atacando);
		enabled = true;
		currentTexSkill = 0;
		ultTiempoSkill = Game.TiempoTranscurrido;
		Vector2 posCentro = refGame.player.getCoordenadasPixeles();
		posCentro.x += CONFIG.TAM/2;
		posCentro.y += CONFIG.TAM/2;	//verificar despues si es suma

		int dmg = 0;
		int dmgOutput = 0;
		//Debug.Log(posCentro);
		
		Vector2 dir = refGame.player.directionVector;
		//tener en cuenta el vector forward al jugador
		dir.Normalize();
		Vector2 enemigo;
		float ang;
		for (int c = 0; refGame.enemigoArray != null && c < refGame.enemigoArray.Length; c++)
		{
            int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (refGame.enemigoArray[c].pos.x - refGame.player.pos.x) * CONFIG.TAM + refGame.enemigoArray[c].microPosAbsoluta.x - refGame.player.microPosAbsoluta.x);
            int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(refGame.enemigoArray[c].pos.y) + refGame.player.pos.y) * CONFIG.TAM - refGame.enemigoArray[c].microPosAbsoluta.y + refGame.player.microPosAbsoluta.y);
            enemigo = new Vector2(xx - Screen.width / 2 + CONFIG.TAM/2, Screen.height / 2 - (yy + CONFIG.TAM / 2));

			ang = Vector2.Angle(dir, enemigo);

			if (enemigo.magnitude < 15f || ang <= 45.0f && enemigo.magnitude <= (2.0f * CONFIG.TAM))
			{
				dmg = Random.Range(dmgMin, dmgMax + 1);
				if (refGame.player.esGCritico)
					dmgOutput += refGame.enemigoArray[c].RecibirDmg((int)(dmg * mod1 * Jugador.DMG_GC), true);
				else
					dmgOutput += refGame.enemigoArray[c].RecibirDmg((int)(dmg * mod1));
			}
		}

		return dmgOutput;
	}

	public override bool Draw(Game refGame)
	{
		if (!enabled)
			return false;

		if (Game.TiempoTranscurrido - ultTiempoSkill >= tiempoFase)
		{
			currentTexSkill++;
			ultTiempoSkill = Game.TiempoTranscurrido;
			if (currentTexSkill >= effectSkill.Length)
			{
				enabled = false;
				refGame.player.CambiarEstado(EntidadCombate.estado.idle);
				return false;
			}
		}

		Matrix4x4 matrixBackup = GUI.matrix;
		float angle = Vector2.Angle(refGame.player.directionVector, new Vector2(0, 1.0f));
		Vector3 cross = Vector3.Cross(refGame.player.directionVector, new Vector2(0, 1.0f));
		if (cross.z > 0)
			angle = 360 - angle;
		
		GUIUtility.RotateAroundPivot(-angle, new Vector2(Screen.width/2, Screen.height/2)); // angulo negativo, la rotacion tiene que ir para el otro lado
		GUI.DrawTexture (new Rect (Screen.width/2  - CONFIG.TAM/2 - CONFIG.TAM * 1.0f /*- refGame.player.microPos.x*/, Screen.height/2 - CONFIG.TAM/2 - CONFIG.TAM * 2.0f /*+ refGame.player.microPos.y*/, CONFIG.TAM * 3, CONFIG.TAM * 3), effectSkill[currentTexSkill]);
		GUI.matrix = matrixBackup;
		return true;
	}
}
