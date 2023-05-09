namespace MoogleEngine;
using System.IO;

public static class Moogle
{
public static SearchResult Query(string query) {

        string[] NombreDeDocumentos = new string[0];

        string[] DireccionDeDocumentos = Directory.GetFiles(".\\..\\Content\\");

        List<string> TodasPalabrasDeLosDocumentosSinRepetir = new List<string>();

        List<string> Debe = new List<string>();

        List<string> NoDebe = new List<string>();

        List<string> Cerca = new List<string>();

        string Sugerencia = "";   

        List<string> CercaDos = new List<string>();

        List<string> snippet = new List<string>();

        Dictionary<string, int> Importancia = new Dictionary<string, int>();

        Dictionary<string, int> Palabra = new Dictionary<string, int>();

        Dictionary<string, double> ScoreDeDocumentos = new Dictionary<string, double>();

        Dictionary<string, int> PalabraQueMasSeRepitePorDocumentos = new Dictionary<string, int>();

        Dictionary<string, int> CantidadDeDocumentosQueApareceUnaPalabra = new Dictionary<string, int>();

        Dictionary<string, Dictionary<string, double>> TFxIDF = new Dictionary<string, Dictionary<string, double>>();

        Dictionary<string, Dictionary<string, List<int>>> Vocabulary = new Dictionary<string, Dictionary<string, List<int>>>();

        Dictionary<string, double> scoreOrdenado = new Dictionary<string, double>();
        string[] elmtos = new string[2];

        void Rellenar()
        {
            //Rellenar Vocabulary
            foreach (var doc in DireccionDeDocumentos)
            {
                Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
                string[] TextoCompleto = File.ReadAllText(doc).Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < TextoCompleto.Length; i++)
                {
                    string palabra = LimpiarPalabra(TextoCompleto[i]);
                    if (dic.ContainsKey(palabra))
                    {
                        dic[palabra].Add(i);
                    }
                    else dic.Add(palabra, new List<int>() { i });
                }
                Vocabulary.Add(doc, dic);

            }

            //Rellenar los Nombre De los Documentos y la Direccion De los Documentos
            NombreDeDocumentos = new string[DireccionDeDocumentos.Length];
            for (int i = 0; i < NombreDeDocumentos.Length; i++)
            {
                NombreDeDocumentos[i] = DireccionDeDocumentos[i][".\\..\\Content\\".Length..];
            }

            //Rellenar todas las palabras de los documentos en una lista(TodasPalabrasDeLosDocumentosSinRepetir)
            foreach (var doc in DireccionDeDocumentos)
            {
                string[] TextoCompleto = File.ReadAllText(doc).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < TextoCompleto.Length; i++)
                {
                    string palabra = LimpiarPalabra(TextoCompleto[i]);
                    if (!TodasPalabrasDeLosDocumentosSinRepetir.Contains(palabra))
                    {
                        TodasPalabrasDeLosDocumentosSinRepetir.Add(palabra);
                    }
                }
            }


            //Rellenar Palabra que mas repite por documentos
            foreach (var doc in DireccionDeDocumentos)
            {
                PalabraQueMasSeRepitePorDocumentos.Add(doc, 0);
                foreach (var voc in Vocabulary[doc])
                {
                    if (PalabraQueMasSeRepitePorDocumentos[doc] < voc.Value.Count)
                    {
                        PalabraQueMasSeRepitePorDocumentos[doc] = voc.Value.Count;
                    }
                }
            }


            //Rellenar cantidad de documentos que aparece una palabra

            foreach (var item in TodasPalabrasDeLosDocumentosSinRepetir)
            {
                int a = 0;
                foreach (var doc in Vocabulary)
                {
                    if (doc.Value.ContainsKey(item))
                        a++;
                }
                CantidadDeDocumentosQueApareceUnaPalabra.Add(item, a);
            }

            //Rellenar el TFxIDF diccionario
            foreach (var item in DireccionDeDocumentos)
            {
                Dictionary<string, double> TFxIDFI = new Dictionary<string, double>();

                for (int i = 0; i < TodasPalabrasDeLosDocumentosSinRepetir.Count; i++)
                {
                    if (Vocabulary[item].ContainsKey(TodasPalabrasDeLosDocumentosSinRepetir[i]))
                    {
                        TFxIDFI.Add(TodasPalabrasDeLosDocumentosSinRepetir[i], HallarTFxIDF(TodasPalabrasDeLosDocumentosSinRepetir[i], item));
                    }
                    else
                    {
                        TFxIDFI.Add(TodasPalabrasDeLosDocumentosSinRepetir[i], 0);
                    }

                }
                TFxIDF.Add(item, TFxIDFI);
            }

            //Rellenar el Score de Dcocumentos

            foreach (var doc in DireccionDeDocumentos)
            {
                for (int i = 0; i < Debe.Count; i++)
                {
                    if (!Vocabulary[doc].ContainsKey(Debe[i]))
                    {
                        ScoreDeDocumentos.Add(doc, 0);
                        break;
                    }
                }

                if (ScoreDeDocumentos.ContainsKey(doc))
                {
                    continue;
                }

                for (int i = 0; i < NoDebe.Count; i++)
                {
                    if (Vocabulary[doc].ContainsKey(NoDebe[i]))
                    {
                        ScoreDeDocumentos.Add(doc, 0);
                        break;
                    }
                }

                if (ScoreDeDocumentos.ContainsKey(doc))
                {
                    continue;
                }

                foreach (var item in Importancia)
                {
                    if (Vocabulary[doc].ContainsKey(item.Key))
                    {
                        ScoreDeDocumentos.Add(doc, Score(query, doc)*item.Value+1);
                        break;
                    }
                }
                    if (ScoreDeDocumentos.ContainsKey(doc))
                    {
                        continue;
                    }

                for (int k = 0; k < elmtos.Length; k++)
                {
                    if (elmtos[0]!=null && Vocabulary[doc].ContainsKey(elmtos[0]) && Vocabulary[doc].ContainsKey(elmtos[1]))
                    {
                        int min = int.MaxValue;
                        for (int l = 0; l < Vocabulary[doc][elmtos[0]].Count; l++)
                        {
                            for (int s = 0; s < Vocabulary[doc][elmtos[1]].Count; s++)
                            {
                                if (Math.Abs(Vocabulary[doc][elmtos[0]][l] -
                                             Vocabulary[doc][elmtos[1]][s]) < min)
                                {
                                    Console.WriteLine(min);
                                    min = Math.Abs(Vocabulary[doc][elmtos[0]][l] -
                                          Vocabulary[doc][elmtos[1]][s]);
                                }
                            }
                        }
                        if (min < 15)
                        {
                        ScoreDeDocumentos.Add(doc, Score(query, doc)* (4f * (15 - min)));
                            break;
                        }
                    }
                }
                if (ScoreDeDocumentos.ContainsKey(doc))
                {
                    continue;
                }


                ScoreDeDocumentos.Add(doc, Score(query, doc));
            }
            ScoreDeDocumentos = ScoreDeDocumentos.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var item in ScoreDeDocumentos)
            {
                if (item.Value != 0)
                {
                    scoreOrdenado.Add(item.Key, item.Value);
                }
                else break;
            }
        }

        List<string> Snippet()
        {
            List<string> snippets = new List<string>();
            foreach (var item in scoreOrdenado)
            {
                    string finalSnippet = "";
                int i = 0;
                    foreach (var item2 in Palabra)
                    {
                        string snippet = "";
                        if (Vocabulary[item.Key].ContainsKey(item2.Key) )
                        {
                        snippet = item2.Key;
                            int pos = Vocabulary[item.Key][item2.Key][0];
                            for (int j = 1; j <= 6; j++)
                            {
                                if (pos + j < File.ReadAllText(item.Key).Split(' ', StringSplitOptions.RemoveEmptyEntries).Length)
                                {
                                    snippet = snippet + " " + File.ReadAllText(item.Key).Split(' ', StringSplitOptions.RemoveEmptyEntries)[pos + j];
                                }
                                if (pos - j >= 0)
                                {
                                    snippet = File.ReadAllText(item.Key).Split(' ', StringSplitOptions.RemoveEmptyEntries)[pos - j] + " " + snippet;
                                }
                            }
                            if (i < Palabra.Count - 1)
                            {
                                snippet = snippet + "...";
                            }
                        }
                        finalSnippet += snippet;
                    }
                    snippets.Add(finalSnippet);
            }
            return snippets;
        }
        double Score(string texto, string doc)
        {
            double count1 = 0;
            double count2 = 0;
            double count3 = 0;
            double count4 = 0;
            double result = 0;
            string[] TextoCompleto = new string[texto.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length];
            for (int j = 0; j < texto.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length; j++)
            {
                TextoCompleto[j] = LimpiarPalabra(texto.Split(' ', StringSplitOptions.RemoveEmptyEntries)[j]);
            }
            for (int i = 0; i < TodasPalabrasDeLosDocumentosSinRepetir.Count; i++)
            {
                count1 += TFxIDF[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]] * HallarTFxIDFDelQuery(texto)[i];
                count2 += Math.Pow(TFxIDF[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]], 2);
                count3 += Math.Pow(HallarTFxIDFDelQuery(texto)[i], 2);
            }
            count4 = Math.Sqrt(count2) * Math.Sqrt(count3);
            result = count1 / count4;
            return result;
        }
        string LimpiarPalabra(string palabra)
        {
            string edit = palabra;
            if (edit != "" && !char.IsLetterOrDigit(edit[0]))
            {
                int a = 1;
                for (int i = 1; i < edit.Length - 1; i++)
                {
                    if (!char.IsLetterOrDigit(edit[i]))
                    {
                        a++;
                    }
                    else break;
                }
                edit = edit[(a)..];
            }
            if (edit != "" && !char.IsLetterOrDigit(edit[edit.Length - 1]))
            {
                int b = edit.Length - 2;
                for (int i = edit.Length - 2; i > 0; i--)
                {
                    if (!char.IsLetterOrDigit(edit[i]))
                    {
                        b--;
                    }
                }
                edit = edit[..(b + 1)];
            }
            return edit;
        }
        double HallarTFxIDF(string palabra, string doc)
        {
            if (CantidadDeDocumentosQueApareceUnaPalabra.ContainsKey(palabra))
            {
                double valor1 = Vocabulary[doc][palabra].Count;
                double valor2 = PalabraQueMasSeRepitePorDocumentos[doc];
                double valor3 = DireccionDeDocumentos.Length;
                double valor4 = CantidadDeDocumentosQueApareceUnaPalabra[palabra];
                return (valor1 / valor2) * Math.Log(valor3 / valor4);
            }
            return 0;
        }
        double[] HallarTFxIDFDelQuery(string texto)
        {
            double[] result = new double[TodasPalabrasDeLosDocumentosSinRepetir.Count];
            List<string> textoEntero = new List<string>();
            for (int i = 0; i < texto.Split(new char[] { ' ', '~' }, StringSplitOptions.RemoveEmptyEntries).Length; i++)
            {
                textoEntero.Add(LimpiarPalabra(texto.Split(new char[] { ' ', '~' }, StringSplitOptions.RemoveEmptyEntries)[i]));
                if (!Palabra.ContainsKey(textoEntero[i]))
                {
                    Palabra.Add(textoEntero[i], 1);
                }
                else
                {
                    Palabra[textoEntero[i]]++;
                }
            }

            int j = 0;
            foreach (var voc in TodasPalabrasDeLosDocumentosSinRepetir)
            {
                if (Palabra.ContainsKey(voc))
                {
                    double valor1 = Palabra[voc];
                    double valor2 = textoEntero.Count;
                    double valor3 = DireccionDeDocumentos.Length;
                    double valor4 = CantidadDeDocumentosQueApareceUnaPalabra[voc];

                    result[j] = (valor1 / valor2) * Math.Log(valor3 / valor4);
                }
                else
                {
                    result[j] = 0;

                }
                j++;
            }
            return result;
        }
        string sugerencia()
        {
            string resultado = "";
            foreach (var item in Palabra)
            {
                if (TodasPalabrasDeLosDocumentosSinRepetir.Contains(item.Key))
                {
                    resultado += " " + item.Key;
                }
                else
                {
                    int min = 10;
                    List<string> stringMinimo = new List<string>();
                    List<int> posicionString = new List<int>();
                    for (int i = 0; i < TodasPalabrasDeLosDocumentosSinRepetir.Count; i++)
                    {
                        if (distanciaL(TodasPalabrasDeLosDocumentosSinRepetir[i], item.Key) < min)
                        {
                            stringMinimo = new List<string>();
                            posicionString = new List<int>();

                            min = distanciaL(TodasPalabrasDeLosDocumentosSinRepetir[i], item.Key);
                            stringMinimo.Add(TodasPalabrasDeLosDocumentosSinRepetir[i]);
                            posicionString.Add(i);

                        }
                        else if (distanciaL(TodasPalabrasDeLosDocumentosSinRepetir[i], item.Key) == min)
                        {
                            stringMinimo.Add(TodasPalabrasDeLosDocumentosSinRepetir[i]);
                            posicionString.Add(i);
                        }
                    }
                    resultado += " " + stringMinimo[0];
                }
            }
            return resultado;
        }
        int distanciaL(string s, string t)
        {
            double porcentaje = 0;

            // d es una tabla con m+1 renglones y n+1 columnas
            int costo = 0;
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];

            // Verifica que exista algo que comparar
            if (n == 0) return m;
            if (m == 0) return n;

            // Llena la primera columna y la primera fila.

            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }


            /// recorre la matriz llenando cada unos de los pesos.
            /// i columnas, j renglones
            for (int i = 1; i <= m; i++)
            {
                // recorre para j
                for (int j = 1; j <= n; j++)
                {
                    /// si son iguales en posiciones equidistantes el peso es 0
                    /// de lo contrario el peso suma a uno.
                    costo = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1,  //Eliminacion
                                  d[i, j - 1] + 1),                             //Insercion 
                                  d[i - 1, j - 1] + costo);                     //Sustitucion
                }
            }

            /// Calculamos el porcentaje de cambios en la palabra.
            if (s.Length > t.Length)
                porcentaje = ((double)d[m, n] / (double)s.Length);
            else
                porcentaje = ((double)d[m, n] / (double)t.Length);
            return d[m, n];
        }
        bool busca1(string consulta, char[] operador)
        {
            for (int i = 0; i < operador.Length; i++)
            {
                if (consulta[0] == operador[i]) return true;

            }
            return false;
        }
        bool busca2(string palabra)
        {
            for (int i = 0; i < palabra.Length; i++)
            {
                if (palabra[i] == '~') return true;
            }
            return false;
        }
        void Operadores(string TextoCompleto)
        {
            string[] frase = TextoCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries) ;
            char[] operadores = new char[] { '^', '!', '*' };
            for (int i = 0; i < frase.Length; i++)
            {
                int index = 0;
                if (busca1(frase[i], operadores))
                {
                    char operadoR = frase[i][index];
                    while (frase[i][index] == operadoR)
                    {
                        index++;
                    }
                    if (operadoR == '^') Debe.Add(frase[i][index..]);
                    if (operadoR == '!') NoDebe.Add(frase[i][index..]);
                    if (operadoR == '*')
                    {
                        int contador = index;
                        Importancia.Add(frase[i][index..], contador);
                    }
                }
                if (busca2(frase[i]))
                {
                     elmtos = frase[i].Split('~');   
                }
            }
        }
        Operadores(query);
        Rellenar();
        snippet = Snippet();
        Sugerencia = query;

        foreach (var item in Palabra)
        {
            if (!TodasPalabrasDeLosDocumentosSinRepetir.Contains(item.Key))
            {
                Sugerencia = sugerencia();
                break;
            }
        }
        SearchItem[] items = new SearchItem[scoreOrdenado.Count];
        int w = 0;
        foreach (var item in scoreOrdenado)
        {
            items[w] = new SearchItem(item.Key[".\\..\\Content\\".Length..],snippet[w], (float)item.Value);
                w++;
        }
        return new SearchResult(items, Sugerencia);
    }
}
