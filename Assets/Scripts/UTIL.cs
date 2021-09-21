using UnityEngine;
using System.Collections;


public static class UTIL
{
	public static int TextoProporcion(int size)
	{
		size = (int)((float)size * ((float)Screen.height/750.0f));
		return size;
	}

    public static Item[] AsignarDrop(int oro, ITEMLIST.ITEM_GROUP iG, int piezas = 1)
    {
        Item[] aux = new Item[1 + piezas];
        aux[0] = aux[1] = null;
        Item.Calidad tipoDrop = 0;
        bool sinDropItem = false;

        

        if (iG != null)
        {
            for (int i = 0; i < piezas; i++)
            {
                float res = Random.Range(0.001f, 100f);
                if (res <= iG.getProbabilidad(Item.Calidad.legendario))
                {
                    tipoDrop = Item.Calidad.legendario;
                }
                else if (res <= iG.getProbabilidad(Item.Calidad.epico))
                {
                    tipoDrop = Item.Calidad.epico;
                }
                else if (res <= iG.getProbabilidad(Item.Calidad.raro))
                {
                    tipoDrop = Item.Calidad.raro;
                }
                else if (res <= iG.getProbabilidad(Item.Calidad.normal))
                {
                    tipoDrop = Item.Calidad.normal;
                }
                else
                {
                    sinDropItem = true;
                }

                /*Debug.Log(res);
                Debug.Log(iG.getProbabilidad(Item.Calidad.normal));
                Debug.Log(iG.getProbabilidad(Item.Calidad.raro));
                Debug.Log(iG.getProbabilidad(Item.Calidad.epico));
                Debug.Log(iG.getProbabilidad(Item.Calidad.legendario));*/

                if (!sinDropItem)
                {
                    int drop = Random.Range(0, iG.getItemCant());  //que pieza toca
                    aux[i] = new Item(iG.getItemAtIndex(drop), tipoDrop);
                }
            }
            
        }

        if (oro > 0)
        {
            oro = (int)Random.Range(oro * 0.75f, oro * 1.25f); //random puede tener desde 25% menos a 25% mas
            aux[piezas] = new Item(999, 1, oro + " oro", Item.TipoItem.Oro, oro, 0);
        }
        

        return aux;
    }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
		/* ////////////////////////////DEPRICADO/////////////////////////////////////////////////////////////////
		 * TABLA DE DROPS
		 * 
		 * LVL 01-14 TIER 0		transicion 12 - 16
		 * LVL 15-31 TIER 1		transicion 29 - 33
		 * LVL 32-45 TIER 2		transicion 43 -
		 * ...
		 * PONER TRANSICIONES: EJ LVL 12 - 17 CAE TIER 0 Y 1
		 * 
		 * 
		 * NORMAL:
		 * 
		 * ORO = SIEMPRE
		 * 70% NO DROP
		 * 25% DROP 1 PIEZA
		 * 4.8% DROP 2 PIEZAS
		 * 0.2% DROP 3 PIEZAS
		 * 
		 * DROP DE PIEZAS:
		 * 
		 * 70% NORMAL
		 * 20% RARO
		 * 7.5% EPICO
		 * 2.5% LEGENDARIO
		 * */
    /*
	private static int generarModificador()
	{
		int mod = 0;
		float res = Random.Range(0.0f, 100.001f);


		if (res < 70.0f)
		{
			mod = Random.Range(0, 6);
		}
		else if (res < 90.0f)
		{
			mod = Random.Range(6, 15);
		}
		else if (res < 97.5f)
		{
			mod = Random.Range(15, 25);
		}
		else
		{
			mod = Random.Range(25, 51);
		}



		return mod;
	}*/

    public static bool valorEnRango(float value, float min, float max)
    {
        return (value > min) && (value <= max);
    }
}
