using UnityEngine;
using System.Collections;

public class SkillT2Twist : Skill {

	public SkillT2Twist() : base()
	{
		tier = 2;
		mod2 = 0;
		mod1 = 2f;
		tiempoFase = 0.05f;
		cooldown = 6.0f;
		codigo = 3;
        


        if (CONFIG.idioma == 0)
        {
            _nombre = "Espada Danzante";
            _descripcion = "Su espada gira alrededor del personaje\n y daña todo lo que lo rodea.\nRealiza 200% de daño.\nEnfriamiento: 6 seg.";
        }
        else
        {
            _nombre = "Dancing sword";
            _descripcion = "Your sword rotates around you\n hitting every enemy near you.\nHits 200% damage.\nCooldown: 6 sec.";
        }

    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		//recorrer array a ver si hay enemigos cerca, que le pegue a TODOS los enemigos cerca, no solo al primero que encuentre. Distancia CONO
		if (enabled)
			return 0;
		enabled = true;
        refGame.refControl.PlaySonido(9);
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
            int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (refGame.enemigoArray[c].pos.x - refGame.player.pos.x) * CONFIG.TAM + refGame.enemigoArray[c].microPosAbsoluta.x - refGame.player.microPosAbsoluta.x);
            int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(refGame.enemigoArray[c].pos.y) + refGame.player.pos.y) * CONFIG.TAM - refGame.enemigoArray[c].microPosAbsoluta.y + refGame.player.microPosAbsoluta.y);
            enemigo = new Vector2(xx - Screen.width / 2 + CONFIG.TAM / 2, Screen.height / 2 - (yy + CONFIG.TAM / 2));

            //En este caso no mira el angulo porque es efecto 360°
            if (enemigo.magnitude <= (2.5f * CONFIG.TAM))
			{
				dmg = Random.Range(dmgMin, dmgMax + 1);
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
        GUI.color = new Color(1f, 1f, 1f, 0.5f);
        GUI.DrawTexture (new Rect (Screen.width/2  - CONFIG.TAM/2 - CONFIG.TAM * 2.0f /*- refGame.player.microPos.x*/, Screen.height/2 - CONFIG.TAM/2 - CONFIG.TAM * 2.0f /*+ refGame.player.microPos.y*/, CONFIG.TAM * 5, CONFIG.TAM * 5), effectSkill[currentTexSkill]);
		GUI.matrix = matrixBackup;
        GUI.color = Color.white;
		return true;
	}
}
