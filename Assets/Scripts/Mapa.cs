using UnityEngine;

public class Mapa
{
    private int _dimX;
    private int _dimY;
    public int DIMX
    {
        get
        {
            return _dimX;
        }
    }
    public int DIMY
    {
        get
        {
            return _dimY;
        }
    }
    public int MAXPOS
    {
        get
        {
            return (_dimX * _dimY);
        }
    }

    public short[] _mundoAbierto;
    public short[] _layer0, _layer1, _layer2, _layer3, _layer4;
    public bool[] _mundoObstaculos;
    public bool[] mundoObstaculos
    {
        get
        {
            return _mundoObstaculos;
        }
    }
    public bool esPosObstaculo(int index)
    {
        if (index > 0 && index < MAXPOS)
        {
            return _mundoObstaculos[index];
        }
        return true;
    }
    private bool _mapaCargado;
    public bool mapaCargado
    {
        get
        {
            return _mapaCargado;
        }
    }
    private Vector2 _coordenadasGlobales;
    public Vector2 coordenadasGlobales
    {
        set
        {
            _coordenadasGlobales = value;
        }
        get
        {
            return _coordenadasGlobales;
        }
    }
    private string _nombreMapaActual;
    public string nombreMapaActual
    {
        get
        {
            return _nombreMapaActual;
        }
    }

    public Mapa(string nombreMapa = "")
    {
        _mapaCargado = false;
        _coordenadasGlobales = new Vector2(-1, -1);
    }

    public void DrawCapasInferiores(int px, int py, int mpx, int mpy, Texture2D[] textura)
    {
        Vector2 centroPantalla = new Vector2(Screen.width / 2, Screen.height / 2);

        //aaa = (Screen.width - CONFIG.TAM)/2 -> distancia desde el medio
        //segmentos = (int)(aaa/CONFIG.TAM);
        //xi = px - segmentos; -> comprobacion de que no sea menor que 0
        //xf = px + segmentos + 1; -> comprobacion de que no supere DIMX
        int xi, xf, yi, yf, segmentosX, segmentosY;
        segmentosX = (int)(((Screen.width - CONFIG.TAM) / 2) / CONFIG.TAM) + 3;
        xi = px - segmentosX;
        if (xi < 0) { xi = 0; }
        xf = px + segmentosX;
        if (xf > _dimX) { xf = _dimX; }

        segmentosY = (int)(((Screen.height - CONFIG.TAM) / 2) / CONFIG.TAM) + 3;
        yi = py - segmentosY;
        if (yi < 0) { yi = 0; }
        yf = py + segmentosY;
        if (yf > _dimY) { yf = _dimY; }

        for (int ya = yi; ya < yf; ya++)
        {
            for (int xa = xi; xa < xf; xa++)
            {
                float xAux = centroPantalla.x - CONFIG.TAM / 2 + (xa - px) * CONFIG.TAM - mpx;
                float yAux = centroPantalla.y - CONFIG.TAM / 2 - (ya - py) * CONFIG.TAM + mpy;
                if (xAux > -CONFIG.TAM && xAux <= Screen.width && yAux > -CONFIG.TAM && yAux <= Screen.height)
                {
                    int index = xa + ya * DIMX;
                    GUI.DrawTexture(new Rect(xAux, yAux, CONFIG.TAM, CONFIG.TAM), textura[_mundoAbierto[index]]);
                    short aux = _layer0[index];
                    if (aux != 0)
                    {
                        GUI.DrawTexture(new Rect(xAux, yAux, CONFIG.TAM, CONFIG.TAM), textura[aux]);
                    }
                    aux = _layer1[index];
                    if (aux != 0)
                    {
                        GUI.DrawTexture(new Rect(xAux, yAux, CONFIG.TAM, CONFIG.TAM), textura[aux]);
                    }
                    aux = _layer2[index];
                    if (aux != 0)
                    {
                        GUI.DrawTexture(new Rect(xAux, yAux, CONFIG.TAM, CONFIG.TAM), textura[aux]);
                    }
                }
            }
        }
    }

    public void DrawCapasSuperiores(int px, int py, int mpx, int mpy, Texture2D[] textura)
    {
        Vector2 centroPantalla = new Vector2(Screen.width / 2, Screen.height / 2);

        //aaa = (Screen.width - CONFIG.TAM)/2 -> distancia desde el medio
        //segmentos = (int)(aaa/CONFIG.TAM);
        //xi = px - segmentos; -> comprobacion de que no sea menor que 0
        //xf = px + segmentos + 1; -> comprobacion de que no supere DIMX
        int xi, xf, yi, yf, segmentosX, segmentosY;
        segmentosX = (int)(((Screen.width - CONFIG.TAM) / 2) / CONFIG.TAM) + 3;
        xi = px - segmentosX;
        if (xi < 0) { xi = 0; }
        xf = px + segmentosX;
        if (xf > _dimX) { xf = _dimX; }

        segmentosY = (int)(((Screen.height - CONFIG.TAM) / 2) / CONFIG.TAM) + 3;
        yi = py - segmentosY;
        if (yi < 0) { yi = 0; }
        yf = py + segmentosY;
        if (yf > _dimY) { yf = _dimY; }

        for (int ya = yi; ya < yf; ya++)
        {
            for (int xa = xi; xa < xf; xa++)
            {
                int index = xa + ya * DIMX;
                float xAux = centroPantalla.x - CONFIG.TAM / 2 + (xa - px) * CONFIG.TAM - mpx;
                float yAux = centroPantalla.y - CONFIG.TAM / 2 - (ya - py) * CONFIG.TAM + mpy;
                if (xAux > -CONFIG.TAM && xAux <= Screen.width && yAux > -CONFIG.TAM && yAux <= Screen.height)
                {
                    short aux = _layer3[index];
                    if (aux != 0)
                    {
                        GUI.DrawTexture(new Rect(xAux, yAux, CONFIG.TAM, CONFIG.TAM), textura[aux]);
                    }
                    aux = _layer4[index];
                    if (aux != 0)
                    {
                        GUI.DrawTexture(new Rect(xAux, yAux, CONFIG.TAM, CONFIG.TAM), textura[aux]);
                    }
                }
            }
        }
    }

    public bool CargarMundo(string nombreMapa)
    {
        int i = 0;
        TextAsset fBase = (TextAsset)Resources.Load(nombreMapa);
        byte[] fileBase = fBase.bytes;

        _dimX = fileBase[0] * 100 + fileBase[1] * 10 + fileBase[2];
        _dimY = fileBase[3] * 100 + fileBase[4] * 10 + fileBase[5];

        _mundoAbierto = new short[_dimX * _dimY];
        _mundoObstaculos = new bool[_dimX * _dimY];
        _layer0 = new short[_dimX * _dimY];
        _layer1 = new short[_dimX * _dimY];
        _layer2 = new short[_dimX * _dimY];
        _layer3 = new short[_dimX * _dimY];
        _layer4 = new short[_dimX * _dimY];

        for (i = 0; i < MAXPOS; i++)
        {
            _mundoAbierto[i] = fileBase[i + 6];
        }

        for (i = 0; i < MAXPOS; i++)
        {

            if (fileBase[MAXPOS + i + 6] == 1)
                _mundoObstaculos[i] = true;
            else
                _mundoObstaculos[i] = false;
        }

        i = (MAXPOS * 2) + 2;  // primeros 2 caracteres L0
        int l = 0;
        while (fileBase[i] != 255 && fileBase[i + 1] != 255)    //señal de eof
        {
            if (!(fileBase[i] == 'L' && fileBase[i + 1] >= '0' && fileBase[i + 1] <= '4'))
            {
                //Tile aux = new Tile();
                //aux.nroTextura = fileBase[i];
                short val = fileBase[i];
                i++;
                int x = fileBase[i] * 256 + fileBase[i + 1];
                i += 2;
                int y = fileBase[i] * 256 + fileBase[i + 1];
                i += 2;
                //aux.pos = new Vector2(x, y);
                // aux.identificador = x + y * dimX;
                if (x >= 0 && x < _dimX && y >= 0 && y < _dimY)
                {
                    switch (l)
                    {
                        case 0:
                            _layer0[x + y * _dimX] = val;
                            break;
                        case 1:
                            _layer1[x + y * _dimX] = val;
                            break;
                        case 2:
                            _layer2[x + y * _dimX] = val;
                            break;
                        case 3:
                            _layer3[x + y * _dimX] = val;
                            break;
                        case 4:
                            _layer4[x + y * _dimX] = val;
                            break;
                    }
                    //layer[l].Add(aux);
                }


            }
            else
            {
                i += 2;
                l++;
            }
        }
        _nombreMapaActual = nombreMapa;
        _mapaCargado = true;
        setCoordenadasGlobalesMapa();
        return true;
    }

    /// <summary>
    /// Establece las coordenadas globales para el mapa que se va a cargar.
    /// Esto sirve para el tema del minimapa.
    /// </summary>
    private void setCoordenadasGlobalesMapa()
    {
        string nombreMapa = _nombreMapaActual;
        switch (nombreMapa)
        {
            case "mapas/mapaCaminoInicial":
                coordenadasGlobales = new Vector2(1151, 1010 - 70);
                break;

            case "mapas/mapaPuebloA":
                coordenadasGlobales = new Vector2(1250, 900 - 70);
                break;

            case "mapas/mapaCiudad":
                coordenadasGlobales = new Vector2(1250, 600 - 70);
                break;

            case "mapas/mapaMount":
                coordenadasGlobales = new Vector2(1250, 300 - 70);
                break;

            case "mapas/mapaPrado0":
                coordenadasGlobales = new Vector2(1151, 842 - 70);
                break;

            case "mapas/mapaPrado":
                coordenadasGlobales = new Vector2(651, 842 - 70);
                break;

            case "mapas/mapaBosque0":
                coordenadasGlobales = new Vector2(1100, 592 - 70);
                break;

            case "mapas/mapaBosque1":
                coordenadasGlobales = new Vector2(800, 642 - 70);
                break;

            case "mapas/mapaBosque2":
                coordenadasGlobales = new Vector2(650, 592 - 70);
                break;

            case "mapas/mapaBosque3":
                coordenadasGlobales = new Vector2(450, 442 - 70);
                break;

            case "mapas/mapaBosque4":
                coordenadasGlobales = new Vector2(1100, 442 - 70);
                break;

            case "mapas/mapaBosqueCiudad":
                coordenadasGlobales = new Vector2(650, 442 - 70);
                break;

            case "mapas/mapaBosqueAlt1":
                coordenadasGlobales = new Vector2(1650, 400 - 70);
                break;

            case "mapas/mapaBosqueAlt2":
                coordenadasGlobales = new Vector2(1840, 500 - 70);
                break;

            case "mapas/mapaDesierto0":
                coordenadasGlobales = new Vector2(150, 572);
                break;

            case "mapas/mapaDesierto1":
                coordenadasGlobales = new Vector2(150, 972);
                break;

            case "mapas/mapaDesierto2":
                coordenadasGlobales = new Vector2(0, 1252);
                break;

            case "mapas/mapaIslaVolcanica":
                coordenadasGlobales = new Vector2(650, 0);
                break;

            case "mapas/mapaPantano0":
                coordenadasGlobales = new Vector2(1340, 900 - 70);
                break;

            case "mapas/mapaPantano1":
                coordenadasGlobales = new Vector2(1840, 600 - 70);
                break;

            case "mapas/mapaPantano2":
                coordenadasGlobales = new Vector2(2090, 600 - 70);
                break;

            case "mapas/mapaPuebloB":
                coordenadasGlobales = new Vector2(150, 442);
                break;

            case "mapas/mapaRios":
                coordenadasGlobales = new Vector2(300, 442 - 70);   //CORREGIDO
                break;

            default:

                break;
        }
    }

}
