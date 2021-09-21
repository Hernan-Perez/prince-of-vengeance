using UnityEngine;
using System.Collections;

public class PasivaT1 : Skill	//voluntad inquebrantable, mientras menos vida menos dmg recibe. 100% vida -> 0% reduccion |||| 0% vida -> 50% reduccion
{
	private float ultimaReduccion;

	public PasivaT1() : base()
	{
		tier = 1;
		mod2 = 0;
		mod1 = 0.5f;	//50% reduccion dmg cuando tiene 0% vida
		tiempoFase = 0f;
		cooldown = 0f;
		ultimaReduccion = 0;
		pasiva = true;
		codigo = 10;

        if (CONFIG.idioma == 0)
        {
            _nombre = "Voluntad Inquebrantable";
            _descripcion = "Mientras más cerca este de\nmorir menos daño recibe.";
        }
        else
        {
            _nombre = "Unbreakable Will";
            _descripcion = "The lower hp you have\nthe less damage you recive.";
        }
       
    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		refGame.player.modificadorDef2 -= ultimaReduccion;
		ultimaReduccion = mod1 * (1f - refGame.player.getHp()/(float)refGame.player.getHpMax());
		refGame.player.modificadorDef2 += ultimaReduccion;

		return 0;

	}
}