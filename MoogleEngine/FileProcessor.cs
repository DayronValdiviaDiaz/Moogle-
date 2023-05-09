using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public class FileProcessor
    {
        string[] NombreDeDocumentos = new string[0];

        string[] DireccionDeDocumentos = Directory.GetFiles(".\\..\\Content\\");

        List<string> TodasPalabrasDeLosDocumentosSinRepetir = new List<string>();

        Dictionary<string, double> ScoreDeDocumentos = new Dictionary<string, double>();

        Dictionary<string, int> PalabraQueMasSeRepitePorDocumentos = new Dictionary<string, int>();

        Dictionary<string, int> CantidadDeDocumentosQueApareceUnaPalabra = new Dictionary<string, int>();

        Dictionary<string, Dictionary<string, double>> TFxIDF = new Dictionary<string, Dictionary<string, double>>();

        Dictionary<string, Dictionary<string, List<int>>> Vocabulary = new Dictionary<string, Dictionary<string, List<int>>>();


        public void Rellenar()
        {
            

            //Rellenar Vocabulary
            foreach (var doc in DireccionDeDocumentos)
            {
                Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
                string[] TextoCompleto = File.ReadAllText(doc).Split();
                for (int i = 0; i < TextoCompleto.Length; i++)
                {
                    string palabra = LimpiarPalabra(TextoCompleto[i]);
                    if (dic.ContainsKey(palabra))
                        dic[palabra].Add(i);
                    else dic.Add(palabra, new List<int>() { i });
                }
                Vocabulary.Add(doc, dic);
            }

            //Rellenar Palabra que mas repite por documentos
            foreach (var doc in DireccionDeDocumentos)
            {
                PalabraQueMasSeRepitePorDocumentos.Add(doc, 0);
                for (int i = 0; i < TodasPalabrasDeLosDocumentosSinRepetir.Count; i++)
                {
                    foreach (var voc in Vocabulary[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]])
                    {
                        if (PalabraQueMasSeRepitePorDocumentos[doc] < Vocabulary[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]].Count)
                            PalabraQueMasSeRepitePorDocumentos[doc] = Vocabulary[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]].Count;
                    }
                }
            }

            //Rellenar los Nombre De los Documentos y la Direccion De los Documentos
            NombreDeDocumentos = new string[DireccionDeDocumentos.Length];
            for (int i = 0; i < NombreDeDocumentos.Length; i++)
            {
                NombreDeDocumentos[i] = DireccionDeDocumentos[i]["D:\\discord\\sherlyn\\First project moogle - 2021\\Content".Length..];
            }

            //Rellenar todas las palabras de los documentos en una lista( TodasPalabrasDeLosDocumentosSinRepetir)
            foreach (var doc in DireccionDeDocumentos)
            {
                string[] TextoCompleto = File.ReadAllText(doc).Split();
                for (int i = 0; i < TextoCompleto.Length; i++)
                {
                    string palabra = LimpiarPalabra(TextoCompleto[i]);
                    if (!TodasPalabrasDeLosDocumentosSinRepetir.Contains(palabra))
                    {
                        TodasPalabrasDeLosDocumentosSinRepetir.Add(palabra);
                    }
                }
            }

            //Rellenar el TFxIDF diccionario
            foreach (var item in DireccionDeDocumentos)
            {
                TFxIDF.Add(item, new Dictionary<string, double>());
                foreach (var item2 in Vocabulary.Values)
                {
                    for (int i = 0; i < TodasPalabrasDeLosDocumentosSinRepetir.Count; i++)
                    {
                        if (Vocabulary[item].ContainsKey(TodasPalabrasDeLosDocumentosSinRepetir[i]))
                        {
                            TFxIDF[item].Add(TodasPalabrasDeLosDocumentosSinRepetir[i], HallarTFxIDF(TodasPalabrasDeLosDocumentosSinRepetir[i], item));
                        }
                        else TFxIDF[item].Add(TodasPalabrasDeLosDocumentosSinRepetir[i], 0);
                    }
                }
            }

            //Rellenar el Score de Dcocumentos
            foreach (var doc in NombreDeDocumentos)
            {
                foreach (var doc2 in DireccionDeDocumentos)
                {
                    foreach (var word in TodasPalabrasDeLosDocumentosSinRepetir)
                    {
                        ScoreDeDocumentos.Add(doc, Score(doc2, word));
                    }
                }
            }
        }
        public double Score(string texto, string doc)
        {
            double count1 = 0;
            double count2 = 0;
            double count3 = 0;
            double count4 = 0;
            double result = 0;
            string[] TextoCompleto = new string[texto.Split().Length];
            for (int j = 0; j < texto.Split().Length; j++)
            {
                TextoCompleto[j] = LimpiarPalabra(texto.Split()[j]);
            }
            for (int i = 0; i < TodasPalabrasDeLosDocumentosSinRepetir.Count; i++)
            {
                if (TextoCompleto.Contains(TodasPalabrasDeLosDocumentosSinRepetir[i]))
                {
                    count1 += TFxIDF[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]] * HallarTFxIDFDelQuery(texto)[i];
                    count2 += Math.Pow(TFxIDF[doc][TodasPalabrasDeLosDocumentosSinRepetir[i]], 2);
                    count3 += Math.Pow(HallarTFxIDFDelQuery(texto)[i], 2);
                }
            }
            count4 = Math.Sqrt(count2) * Math.Sqrt(count3);
            result = count1 / count4;
            return result;
        }
        public string LimpiarPalabra(string palabra)
        {
            string edit = palabra;
            if (edit != "")
            {
                if (!char.IsLetterOrDigit(edit[0]))
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
                if (!char.IsLetterOrDigit(edit[edit.Length - 1]))
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
            }
            return edit;
        }
        public double HallarTFxIDF(string palabra, string doc)
        {
            double valor1 = Vocabulary[doc][palabra].Count;
            double valor2 = PalabraQueMasSeRepitePorDocumentos[doc];
            double valor3 = DireccionDeDocumentos.Length;
            double valor4 = CantidadDeDocumentosQueApareceUnaPalabra[palabra];
            return (valor1 / valor2) * Math.Log10(valor3 / valor4);
        }

        public double[] HallarTFxIDFDelQuery(string texto)
        {
            double[] result = new double[TodasPalabrasDeLosDocumentosSinRepetir.Count];
            List<string> textoEntero = new List<string>();
            Dictionary<string, int> Palabra = new Dictionary<string, int>();
            for (int i = 0; i < texto.Split().Length; i++)
            {
                textoEntero.Add(LimpiarPalabra(texto.Split()[i]));
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
                    result[j] = 0;
                j++;
            }
            return result;
        }

        public bool Find(string consulta, char[] operador)

        {
            for (int i = 0; i < operador.Length; i++)
            {
                if (consulta[0] == operador[i]) return true;

            }
            return false;
        }
        public bool Find2(string palabra)
        {
            for (int i = 0; i < palabra.Length; i++)
            {
                if (palabra[i] == '~') return true;
            }
            return false;
        }

        void Operadores(string TextoCompleto)
        {
            string[] frase = new string[TextoCompleto.Split().Length];
            for (int i = 0; i < TextoCompleto.Split().Length; i++)
            {
                foreach (var word in TextoCompleto.Split())
                {
                    frase[i] = LimpiarPalabra(word);
                }
            }
            List<string> Importante = new List<string>();
            List<string> Debe = new List<string>();
            List<string> NoDebe = new List<string>();
            List<string> Cerca = new List<string>();
            List<string> CercaDos = new List<string>();
            char[] operadores = new char[] { '^', '!', '*' };
            for (int i = 0; i < frase.Length; i++)
            {
                int index = 0;
                if (Find(frase[i], operadores))
                {
                    char operadoR = frase[i][index];
                    while (frase[i][index] == operadoR)
                    {
                        index++;
                    }
                    if (operadoR == '^') Debe.Add(frase[i][index..]);
                    if (operadoR == '!') NoDebe.Add(frase[i][index..]);
                    if (operadoR == '*') Importante.Add(frase[i][index..]);
                }
                if (Find2(frase[i]))
                {
                    int count = 2;
                    string[] elmtos = frase[i].Split('~');
                    for (int k = 0; k < elmtos.Length; k++)
                    {
                        if (count % 2 == 0) Cerca.Add(elmtos[k]);
                        else CercaDos.Add(elmtos[k]);
                        count++;
                    }
                }
            }

        }
            void snippet(string text, string consulta)
            {
            string[] frase = new string[text.Split().Length];
            for (int i = 0; i < text.Split().Length; i++)
            {
                foreach (var word in text.Split())
                {
                    frase[i] = LimpiarPalabra(word);
                }
            }
            string[] consultaC = new string[consulta.Split().Length];

            for (int i = 0; i < consulta.Split().Length; i++)
            {
                foreach (var word in consulta.Split())
                {
                    consultaC[i] = LimpiarPalabra(word);
                }
            }
            int a = 0;
                for (int i = 0; i < consultaC.Length; i++)
                {
                    for (int j = 0; j < frase.Length; j++)
                    {
                        if (consultaC[i] == frase[j])
                        {
                            if (j == 0)
                            { System.Console.Write(dame(frase, j, 2)); }
                            else if (a != j)
                            {
                                System.Console.Write(dame(frase, j, 2));
                                a = j;
                            }
                        }
                    }
                }
            }
            string dame(string[] cadenaText, int index, int cant)
            {
                string dame = "";
                string dameDos = "";
                string result = "";
                int indexOriginal = index;
                for (int i = 0; i <= cant; i++)
                {
                    if ((index - i) < 0) break;
                    dame = cadenaText[index] + " " + dame;
                    index--;
                }
                index = indexOriginal;
                for (int j = 0; j < cant; j++)
                {
                    if ((j + index) >= cadenaText.Length) break;
                    if (dameDos == "")
                    {
                        dameDos = cadenaText[index + 1];
                    }
                    else
                    {
                        dameDos = dameDos + " " + cadenaText[index + 1];
                    }
                    index++;
                }
                result = dame + dameDos;
                return result;
            }
    }
}