using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Se encarga de crear enemigos automaticamente a partir de los parametros dados.
/// </summary>
class EnemigoZoneLoader
{
    private List<Enemigo> listaModeloEnemigos;
    private List<Enemigo> listaEnemigos;
    private List<Rect> zonasFree;
    private List<Rect> zonasHotSpot;
    private List<int> zonasHotSpotCantEnem;

    private Rect zonaSpawn;
    private bool zonaSpawnCustom = false;
    private float distanciaMinEnems;
    private int cantidadEnems;
    private int lvlMin, lvlMax;
    private ITEMLIST.ITEM_GROUP itemsDrop;

    public EnemigoZoneLoader(int cantEnems, int lMin, int lMax, ITEMLIST.ITEM_GROUP drop, float distMin)
    {
        listaModeloEnemigos = new List<Enemigo>();
        listaEnemigos = new List<Enemigo>();
        zonasFree = new List<Rect>();
        zonasHotSpot = new List<Rect>();
        zonasHotSpotCantEnem = new List<int>();

        zonaSpawn = new Rect(0, 0, 0, 0);
        if (cantEnems < 1)
            cantEnems = 1;
        cantidadEnems = cantEnems;
        lvlMin = lMin;
        lvlMax = lMax;
        itemsDrop = drop;
        distanciaMinEnems = distMin;
    }

    public void setZonaSpawnCustom(Rect zona)
    {
        zonaSpawn = zona;
        zonaSpawnCustom = true;
    }

    public void setZonaFree(Rect zFree)
    {
        zonasFree.Add(zFree);
    }

    public void setZonaFree(List<Rect> listaZonaFree)
    {
        for (int i = 0; i < listaZonaFree.Count; i++)
        {
            zonasFree.Add(listaZonaFree[i]);
        }
    }

    /// <summary>
    /// Agrega Zona Hot Spot, lo que significa que setea una zona especial en el mapa que asegura que ese lugar tenga enemigos siempre.
    /// NOTA: los enemigos agregados aca son un extra, no se debitan de la cantidad original seteada en el constructor.
    /// </summary>
    /// <param name="zHS"></param>
    /// <param name="cantidad"></param>
    /// <param name="agregarAZonaLibre"></param>
    public void setZonaHotSpot(Rect zHS, int cantidad, bool agregarAZonaLibre = true)
    {
        if (agregarAZonaLibre)
            zonasFree.Add(zHS);
        zonasHotSpot.Add(zHS);
        zonasHotSpotCantEnem.Add(cantidad);
    }

    public void agregarTipoDeEnemigo(Enemigo enem)
    {
        listaModeloEnemigos.Add(enem);
    }

    public Enemigo[] GenerarEnemigos(Mapa refMapa, int indice = 0)
    {
        if (listaModeloEnemigos.Count == 0)
        {
            Debug.LogError("LISTA MODELO DE ENEMIGOS ESTA VACIA");
            return null;
        }

        if (!zonaSpawnCustom)
            zonaSpawn = new Rect(0, 0, refMapa.DIMX, refMapa.DIMY);

        Enemigo aux, modelo;
        int cantidad = 0;
        int intentos = 0;
        bool seguir = false;
        int i = 0;
        Vector2 distAux;

        while (cantidad < cantidadEnems && intentos < 500)
        {
            intentos++;
            seguir = false;
            modelo = getModeloEnemigoAlAzar();
            aux = new Enemigo(modelo.sprite,
                            (int)Random.Range(zonaSpawn.xMin, zonaSpawn.xMax), 
                            (int)Random.Range(zonaSpawn.yMin, zonaSpawn.yMax),
                            Random.Range(lvlMin, lvlMax + 1),
                            modelo.esMelee(),
                            itemsDrop, 
                            modelo.presetAnimacion);

            if (modelo.esMago())
            {
                aux.setMago(modelo.getTexMago());
            }

            //comprueba si es valido

            //comprueba que no este sobre un obstaculo
            if (refMapa.esPosObstaculo((int)(aux.pos.x + aux.pos.y * refMapa.DIMX)))
                continue;

            //comprueba que no este sobre una zona free
            for (i = 0; i < zonasFree.Count; i++)
            {
                if (aux.pos.x >= zonasFree[i].xMin && aux.pos.x <= zonasFree[i].xMax
                    && aux.pos.y >= zonasFree[i].yMin && aux.pos.y <= zonasFree[i].yMax)
                {
                    seguir = true;
                }
                    
            }

            if (seguir)
                continue;

            //comprueba que no este cerca de ningun enemigo
            for (i = 0; i < listaEnemigos.Count; i++)
            {
                distAux = new Vector2(aux.pos.x - listaEnemigos[i].pos.x, aux.pos.y - listaEnemigos[i].pos.y);
                if (distAux.magnitude < distanciaMinEnems)
                    seguir = true;
            }

            if (seguir)
                continue;

            listaEnemigos.Add(aux);
            intentos = 0;
            cantidad++;
        }

        if (intentos >= 500)
            Debug.LogWarning("En enemigoZonaLoader, 'INTENTOS' SUPERO EL LIMITE PROGRAMADO");

        //carga hotspots
        for (i = 0; i < zonasHotSpot.Count; i++)
        {
            cantidad = 0;
            intentos = 0;
            while (cantidad < zonasHotSpotCantEnem[i] && intentos < 500)
            {
                intentos++;

                modelo = getModeloEnemigoAlAzar();
                aux = new Enemigo(modelo.sprite,
                                (int)Random.Range(zonasHotSpot[i].xMin, zonasHotSpot[i].xMax),
                                (int)Random.Range(zonasHotSpot[i].yMin, zonasHotSpot[i].yMax),
                                Random.Range(lvlMin, lvlMax + 1),
                                modelo.esMelee(),
                                itemsDrop,
                                modelo.presetAnimacion);

                //comprueba si es valido

                //comprueba que no este sobre un obstaculo
                if (refMapa.esPosObstaculo((int)(aux.pos.x + aux.pos.y * refMapa.DIMX)))
                    continue;

                //comprueba que no este cerca de ningun enemigo
                for (int c = 0; c < listaEnemigos.Count; c++)
                {
                    distAux = new Vector2(aux.pos.x - listaEnemigos[c].pos.x, aux.pos.y - listaEnemigos[c].pos.y);
                    if (distAux.magnitude < 1)  //no se si deberia usar la variable en el hotspot, es un caso especial
                        seguir = true;
                }

                if (seguir)
                    continue;

                listaEnemigos.Add(aux);
                intentos = 0;
                cantidad++;
            }

            if (intentos >= 500)
                Debug.LogWarning("En enemigoZonaLoader (hotspot " + i + "), 'INTENTOS' SUPERO EL LIMITE PROGRAMADO");
        }


        Enemigo[] arrayOutput = new Enemigo[listaEnemigos.Count + indice];
        for (i = 0; i < listaEnemigos.Count; i++)
            arrayOutput[i + indice] = listaEnemigos[i];
        return arrayOutput;
    }

    private Enemigo getModeloEnemigoAlAzar()
    {
        return listaModeloEnemigos[Random.Range(0, listaModeloEnemigos.Count)];
    }

}
