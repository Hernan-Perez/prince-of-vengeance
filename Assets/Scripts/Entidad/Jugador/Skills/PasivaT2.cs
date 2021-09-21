using UnityEngine;
using System.Collections;

public class PasivaT2 : Skill	//recuperacion veloz, recupera vida cada x intervalos, despues de un determinado tiempo de no recibir daño
{
	
	public PasivaT2() : base()
	{
		tier = 2;
		mod2 = 0;
		mod1 = 0.05f;	//5% de curacion en cada intervalo
		tiempoFase = 0f;
		cooldown = 10f;	//cada 10 segundos se cura
		pasiva = true;
		codigo = 11;

        if (CONFIG.idioma == 0)
        {
            _nombre = "Aura de Poder";
            _descripcion = "Un aura persistente que aumenta el daño realizado 30%,\nla armadura 25% y la velocidad de movimiento 20%.";
        }
        else
        {
            _nombre = "Aura of Power";
            _descripcion = "A persistent aura that grants you 30% more damage,\n25% more armor and 20% faster walking speed. ";
        }

        
    }
	
	public override int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		
		return 0;
		
	}
}