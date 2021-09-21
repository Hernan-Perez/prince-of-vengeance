using UnityEngine;
using System.Collections;

public class SkillT5TSwords : Skill 
{
    private float contador;
	float ultGolpe;
	public SkillT5TSwords() : base()
	{
		tier = 5;
		mod2 = 0.5f;	//intervalos de dmg
		mod1 = 0.2f;	//%  dmg (BASE)
		tiempoFase = 0.05f; 
		cooldown = 90f;
		codigo = 8;

        if (CONFIG.idioma == 0)
        {
            _nombre = "Mil Espadas";
            _descripcion = "Durante 10 segundos mil espadas\ngiran en torno al personaje\n y dañan todo a su paso.\nRealiza 20% de daño (cada golpe).\nEnfriamiento: 90 seg.";
        }
        else
        {
            _nombre = "Thousand Swords";
            _descripcion = "For 10 seconds a thousand swords\nrotate around you\n damaging everything.\nHits 20% damage (each hit).\nCooldown: 90 sec.";
        }

    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		/*if (enabled)
			return 0;*/
		if (!enabled)
		{
			currentTexSkill = 0;
			ultGolpe = 0;
			base.Accion(dmgMin,dmgMax,refGame);
            refGame.refControl.PlayEfectos(8, 1f);
            contador = Game.TiempoTranscurrido;
        }

		enabled = true;
        
		//refGame.player.CambiarEstado(EntidadCombate.estado.idle);
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
        bool golpear = false;
		for (int c = 0; refGame.enemigoArray != null && c < refGame.enemigoArray.Length; c++)
		{
            //enemigo = new Vector2(refGame.enemigoArray[c].getCoordenadasPixeles().x + CONFIG.TAM/2 - posCentro.x , refGame.enemigoArray[c].getCoordenadasPixeles().y + CONFIG.TAM/2 - posCentro.y);
            //En este caso no mira el angulo porque es efecto 360°

            int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (refGame.enemigoArray[c].pos.x - refGame.player.pos.x) * CONFIG.TAM + refGame.enemigoArray[c].microPosAbsoluta.x - refGame.player.microPosAbsoluta.x);
            int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(refGame.enemigoArray[c].pos.y) + refGame.player.pos.y) * CONFIG.TAM - refGame.enemigoArray[c].microPosAbsoluta.y + refGame.player.microPosAbsoluta.y);
            enemigo = new Vector2(xx - Screen.width / 2 + CONFIG.TAM / 2, Screen.height / 2 - (yy + CONFIG.TAM / 2));

            if (enemigo.magnitude <= (4.5f * CONFIG.TAM) && refGame.enemigoArray[c].Estado != EntidadCombate.estado.miss && refGame.enemigoArray[c].Estado != EntidadCombate.estado.muerto)
			{
                golpear = true;
				dmg = Random.Range(dmgMin, dmgMax + 1);
				dmgOutput += refGame.enemigoArray[c].RecibirDmg((int)(dmg * mod1));
			}
		}
		
        if (golpear)
        {
            refGame.refControl.PlaySonido(2);
        }

		return dmgOutput;
	}
	
	public override bool Draw(Game refGame)
	{
		if (!enabled)
			return false;

        if (Game.TiempoTranscurrido - contador > 10f)
        {
            contador = 0f;
            enabled = false;
            refGame.refControl.StopEfectos();
            return false;
        }

		//no deberia llamar a accion desde draw, pero deberia funcionar bien
		if (Game.TiempoTranscurrido - ultGolpe >= mod2)
		{
			ultGolpe = Game.TiempoTranscurrido;
			Accion(refGame.player.getDmgMin(), refGame.player.getDmgMax(), refGame); 
		}

		if (Game.TiempoTranscurrido - ultTiempoSkill >= tiempoFase)
		{
			currentTexSkill++;
			ultTiempoSkill = Game.TiempoTranscurrido;
			if (currentTexSkill >= 200)
			{
                currentTexSkill = 0;
                /*refGame.refControl.StopSonido();
                enabled = false;
				//refGame.player.CambiarEstado(EntidadCombate.estado.idle);
				return false;*/
			}
		}

		Matrix4x4 matrixBackup = GUI.matrix;
		GUIUtility.RotateAroundPivot(-5.0f * currentTexSkill, new Vector2(Screen.width/2, Screen.height/2));
		GUI.DrawTexture (new Rect (Screen.width/2 - CONFIG.TAM * 4.5f, Screen.height/2 - CONFIG.TAM * 4.5f, CONFIG.TAM * 9f, CONFIG.TAM * 9f), effectSkill[0], ScaleMode.ScaleAndCrop);
		GUI.matrix = matrixBackup;
		return true;
	}
	
}