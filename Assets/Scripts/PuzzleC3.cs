using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// C3 aplicable para general ghaldum, simplemente combina c1 y c2
/// </summary>

public sealed class PuzzleC3 : Puzzle
{
    private List<short> texCerrado;
    private List<short> texANTCerrado;
    private List<Vector2> posCerrado;
    private List<bool> obsCerrado;
    private List<bool> obsANTCerrado;   //ESTO ES PARA DEVOLVER LA CONDICION INICIAL, SI UNA PUERTA ESTA CERRADA PARA QUE DESPUES SE PEUDA PASAR

    private Boss refBoss;

    private bool _desactivado;
    private bool _obstaculosActivados = false;
    private string mensaje = "";

    public PuzzleC3(int codigo, Boss rf)
    {
        cod = -1;
        _desactivado = false;//refGame.puzzleResuelto[cod];
        refBoss = rf;
        texCerrado = new List<short>();
        texANTCerrado = new List<short>();
        posCerrado = new List<Vector2>();
        obsANTCerrado = new List<bool>();
        obsCerrado = new List<bool>();
    }

    public override void Update()
    {
        if (_desactivado)
            return;

        if (refBoss.activado)
        {
            if (!_obstaculosActivados)
            {
                _obstaculosActivados = true;
                Activar();
                if (mensaje != "")
                {
                    refGame.hud.AgregarTextoConversacion(mensaje);
                }
            }
            //CHEQUEAR ACA CUANDO MUERA EL BOSS ABRIR LAS PUERTAS Y DESACTIVAR EL PUZZLE
            if (refBoss.Estado == EntidadCombate.estado.miss || refBoss.estadoAI == Enemigo.AiState.DEAD)
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
        posCerrado.Add(pos);
        obsANTCerrado.Add(refGame.currentMapa.esPosObstaculo((int)(pos.x + pos.y * refGame.currentMapa.DIMX))); //guarda la anterior condicion obs
        obsCerrado.Add(obs);    //por ahora se guarda al pedo pero puede llegar a servir despues
        //refGame.currentMapa._mundoObstaculos[index] = obs;
    }

    private bool intercambiar(bool cond)
    {
        if (cond)
            return false;
        return true;
    }

    private void Activar()  //esto se tiene que llamar cuando se active el boss
    {
        if (_desactivado)
            return;

        Vector2 pos;
        int index = 0;
        for (int i = 0; i < obsCerrado.Count; i++)
        {
            pos = posCerrado[i];
            index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
            refGame.currentMapa._layer2[index] = texCerrado[i];
            refGame.currentMapa._mundoObstaculos[index] = obsCerrado[i];
        }
    }

    private void Desactivar()
    {
        if (_desactivado)
            return;
        //_desactivado = true;
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


    public void setMensajeAlActivarBoss(string msg)
    {
        mensaje = msg;
    }
}
