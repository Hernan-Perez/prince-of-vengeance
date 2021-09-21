using UnityEngine;
using System.Collections;

public class SkillT4Reflect : Skill 
{
	public SkillT4Reflect() : base()
	{
		tier = 4;
		mod2 = 3.0f;	//aumentaria otro % teniendo 0% de vida
		mod1 = 2.0f;	//%  dmg (BASE)
		tiempoFase = 0.01f; 
		cooldown = 12.0f;
		codigo = 6;

        if (CONFIG.idioma == 0)
        {
            _nombre = "Rechazo De La Muerte";
            _descripcion = "Un golpe que daña a todo lo que rodee.\nMientras menos vida tenga\nel personaje más daño causa.\nRealiza de 200% a 500% de daño.\nEnfriamiento: 12 seg.";
        }
        else
        {
            _nombre = "Rejection Of Death";
            _descripcion = "A blow that damages everyone around you.\nThe less hp you have,\nthe more damage this blow will do.\nHits fron 200% to 500% damage.\nCooldown: 12 sec.";
        }

        
    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		if (enabled)
			return 0;
		enabled = true;
        refGame.refControl.PlaySonido(11);
        base.Accion(dmgMin,dmgMax,refGame);
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
				dmgOutput += refGame.enemigoArray[c].RecibirDmg((int)(dmg * (mod1 + mod2 - mod2 * refGame.player.getHp()/(float)refGame.player.getHpMax())));
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
			if (currentTexSkill >= 20)
			{
				enabled = false;
				refGame.player.CambiarEstado(EntidadCombate.estado.idle);
				return false;
			}
		}
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - 0.05f * currentTexSkill);	//currentTexSkill = 20 => termina 
		GUI.DrawTexture (new Rect (Screen.width/2  - CONFIG.TAM/2 - CONFIG.TAM * (0.5f + 0.1f * currentTexSkill), Screen.height/2 - CONFIG.TAM/2 - CONFIG.TAM * (0.5f + 0.1f * currentTexSkill), CONFIG.TAM * (2f + 0.2f * currentTexSkill), CONFIG.TAM * (2f + 0.2f * currentTexSkill)), effectSkill[0], ScaleMode.ScaleAndCrop);
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		return true;
	}
	
}
