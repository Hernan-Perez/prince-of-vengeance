using UnityEngine;
using System.Collections;

public class SkillT4Vamp : Skill 
{
	int hpGanada;
	public SkillT4Vamp() : base()
	{
		tier = 4;
		mod2 = 0.05f;	// cura % del dmg que pego
		mod1 = 1.5f;	//%  dmg (BASE)
		tiempoFase = 0.02f; 
		hpGanada = 0;
		cooldown = 12.0f;
		codigo = 7;

        if (CONFIG.MODO_EXTREMO)
        {
            mod2 = 0.025f;
        }


        if (CONFIG.idioma == 0)
        {
            _nombre = "Esencia Vampirica";
            _descripcion = "Un golpe que daña a todo lo que rodee.\nRecupera vida en base al daño causado.\nRealiza 250% de daño.\nEnfriamiento: 12 seg.";
        }
        else
        {
            _nombre = "Vampiric Essence";
            _descripcion = "A blow that damages everyone around you.\nRecovers life based on damage done.\nHits 250% damage.\nCooldown: 12 sec.";
        }


        
    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		if (enabled)
			return 0;
		base.Accion(dmgMin,dmgMax,refGame);
		enabled = true;
        refGame.refControl.PlaySonido(11);
        hpGanada = 0;
		refGame.player.CambiarEstado(EntidadCombate.estado.atacando);
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
		for (int c = 0; refGame.enemigoArray != null && c < refGame.enemigoArray.Length; c++)
        {
            //enemigo = new Vector2(refGame.enemigoArray[c].getCoordenadasPixeles().x + CONFIG.TAM/2 - posCentro.x , refGame.enemigoArray[c].getCoordenadasPixeles().y + CONFIG.TAM/2 - posCentro.y);
            //En este caso no mira el angulo porque es efecto 360°

            int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (refGame.enemigoArray[c].pos.x - refGame.player.pos.x) * CONFIG.TAM + refGame.enemigoArray[c].microPosAbsoluta.x - refGame.player.microPosAbsoluta.x);
            int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(refGame.enemigoArray[c].pos.y) + refGame.player.pos.y) * CONFIG.TAM - refGame.enemigoArray[c].microPosAbsoluta.y + refGame.player.microPosAbsoluta.y);
            enemigo = new Vector2(xx - Screen.width / 2 + CONFIG.TAM / 2, Screen.height / 2 - (yy + CONFIG.TAM / 2));

            if (enemigo.magnitude <= (3f * CONFIG.TAM))
			{
				dmg = Random.Range(dmgMin, dmgMax + 1);
				dmgOutput += refGame.enemigoArray[c].RecibirDmg((int)(dmg * mod1));
			}
		}
		hpGanada = (int)(dmgOutput * mod2);
		refGame.player.ganarVida(hpGanada);
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
			if (currentTexSkill >= 20)
			{
				enabled = false;
				refGame.player.CambiarEstado(EntidadCombate.estado.idle);
				return false;
			}
		}
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.05f * currentTexSkill);	//currentTexSkill = 20 => termina 
		GUI.DrawTexture (new Rect (Screen.width/2  - CONFIG.TAM/2 - CONFIG.TAM * (0.5f + 0.1f * (20 - currentTexSkill)), Screen.height/2 - CONFIG.TAM/2 - CONFIG.TAM * (0.5f + 0.1f * (20 - currentTexSkill)), CONFIG.TAM * (2f + 0.2f * (20 - currentTexSkill)), CONFIG.TAM * (2f + 0.2f * (20 - currentTexSkill))), effectSkill[0], ScaleMode.ScaleAndCrop);
		if (hpGanada != 0)
		{
			GUIStyle estilo = new GUIStyle ();
			estilo.normal.textColor = new Color(0f, 1.0f, 0f, 1.0f - 0.05f * currentTexSkill);
			estilo.fontSize = UTIL.TextoProporcion(34);
			estilo.alignment = TextAnchor.UpperCenter;
			//Debug.Log("ASDASD");
			GUI.Label(new Rect (Screen.width/2 - CONFIG.TAM * 1.5f, Screen.height/2 - CONFIG.TAM/2 - CONFIG.TAM * currentTexSkill/30.0f, CONFIG.TAM * 3f, CONFIG.TAM * 1.5f), "+" + hpGanada, estilo);
		}
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		return true;
	}
	
}
