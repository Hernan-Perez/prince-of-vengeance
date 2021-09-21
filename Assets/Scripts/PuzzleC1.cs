using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// C1 significa Custom1, esto quiere decir que ES SOLO APLICABLE PARA UN CASO ESPECIFICO Y UNICO.
/// Esto no es exactamente un puzzle, pero es el termino que uso para encerrar los diferentes eventos.
/// En este caso es el boss final de la zona 1.
/// Caracteristicas: El boss esta quieto e invulnerable hasta que mueran los adds. Cuando muere el boss se abre la puerta.
/// </summary>

public sealed class PuzzleC1 : Puzzle
{
    private List<short> texCerrado;
    private List<short> texANTCerrado;
    private List<Vector2> posCerrado;
    private List<bool> obsCerrado;
    private List<bool> obsANTCerrado;   //ESTO ES PARA DEVOLVER LA CONDICION INICIAL, SI UNA PUERTA ESTA CERRADA PARA QUE DESPUES SE PEUDA PASAR

    private List<Enemigo> listaEnemigosAMorir;
    private Boss refBoss;

    private bool _desactivado;
    private bool faseBossActivo;
    private string mensaje = "";

    public PuzzleC1(int codigo, Boss rf, bool hayEnemigosAMorir = true, bool ActivarBoss = true)
    {
        cod = -1;
        _desactivado = false;//refGame.puzzleResuelto[cod];
        refBoss = rf;
        texCerrado = new List<short>();
        texANTCerrado = new List<short>();
        posCerrado = new List<Vector2>();
        obsANTCerrado = new List<bool>();
        obsCerrado = new List<bool>();
        faseBossActivo = false;
        listaEnemigosAMorir = new List<Enemigo>();

        if (!hayEnemigosAMorir)
        {
            faseBossActivo = true;
            if (ActivarBoss)
                refBoss.ActivarBoss(true);
        }
    }

    public override void Update()
    {
        if (_desactivado)
            return;
        
        if (!faseBossActivo)
        {
            bool muertos = true;
            for (int i = 0; i < listaEnemigosAMorir.Count; i++)
            {
                if (listaEnemigosAMorir[i].Estado != EntidadCombate.estado.miss)
                {
                    muertos = false;
                    break;
                }
            }

            if (muertos)
            {
                faseBossActivo = true;
                refBoss.ActivarBoss(true);

                if (mensaje != "")
                {
                    refGame.hud.AgregarTextoConversacion(mensaje);
                }
                
                //ACA FALTA PONER MENSAJE EN PANTALLA

                //----A FUTURO DESDE ACA TENDRIA QUE CAMBIAR LA CANCION POR UNA MAS DRAMATICA----
            }
        }
        else
        {
            //CHEQUEAR ACA CUANDO MUERA EL BOSS ABRIR LAS PUERTAS Y DESACTIVAR EL PUZZLE
            if (refBoss.Estado == EntidadCombate.estado.miss)
            {
                Desactivar();
            }
        }
        
    }

    public override void Draw()
    {
        if (_desactivado)
            return;
    }
    /*
    public override bool Touch(Vector2 pos)
    {
        if (_desactivado)
            return false;
        return false;
    }*/

    public void AgregarZonaCerrada(short tex, Vector2 pos, bool obs)
    {
        if (_desactivado)
            return;
        int index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
        texCerrado.Add(tex);
        texANTCerrado.Add(refGame.currentMapa._layer2[index]);
        refGame.currentMapa._layer2[index] = tex;
        posCerrado.Add(pos);
        obsANTCerrado.Add(refGame.currentMapa.esPosObstaculo((int)(pos.x + pos.y * refGame.currentMapa.DIMX))); //guarda la anterior condicion obs
        obsCerrado.Add(obs);    //por ahora se guarda al pedo pero puede llegar a servir despues
        refGame.currentMapa._mundoObstaculos[index] = obs;
    }

    private bool intercambiar(bool cond)
    {
        if (cond)
            return false;
        return true;
    }

    private void Desactivar()
    {
        if (_desactivado)
            return;
        _desactivado = true;
        //refGame.puzzleResuelto[cod] = true;
        int index = 0;
        Vector2 pos;
        for (int i = 0; i < texCerrado.Count; i++)
        {
            pos = posCerrado[i];
            index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
            refGame.currentMapa._layer2[index] = texANTCerrado[i];
            refGame.currentMapa._mundoObstaculos[index] = obsANTCerrado[i];
        }

    }

    public void agregarEnemigoAMorir(Enemigo e)
    {
        listaEnemigosAMorir.Add(e);
    }

    public void setMensajeAlActivarBoss(string msg)
    {
        mensaje = msg;
    }
}
