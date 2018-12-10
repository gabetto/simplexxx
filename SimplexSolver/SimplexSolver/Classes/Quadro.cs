using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SimplexSolver.Classes
{
    public class Quadro
    {
        public Dictionary<String, double[]> Linhas { get; set; }
        public string[] Cabecario { get; set; }
        public KeyValuePair<String, double[]> LinhaZ { get; set; }
        public KeyValuePair<String, double[]> LinhaZ2 { get; set; }
        public bool duasFases;
        public Quadro QuadroAnterior;




        public Quadro(FuncaoObjetiva FObjetiva, List<Restricao> restricoes, int quantA, int quantF)
        {
            int tamanho = FObjetiva.Tamanho();
            tamanho = tamanho + quantA + quantF + 1;

            if (quantA > 0)
            {
                duasFases = true;
            }
            else
            {
                duasFases = false;
            }

            string[] novoCabecario = new string[tamanho];

            int a = 1;
            int f = 1;
            int j = 0;
            int jAux = FObjetiva.Letras.Length;

            for (int i = 0; i < tamanho; i++)
            {
                if (j < jAux)
                {
                    novoCabecario[i] = FObjetiva.Letras[j];
                    j++;
                }
                else if (f <= quantF)
                {
                    novoCabecario[i] = "f" + f;
                    f++;
                }
                else if (a <= quantA)
                {
                    novoCabecario[i] = "a" + a;
                    a++;
                }
                else
                {
                    novoCabecario[i] = "b";
                }
            }
            this.Cabecario = novoCabecario;

            f = 1;

            Dictionary<string, double[]> novasLinhas = new Dictionary<string, double[]>();
            for (int i = 0; i < restricoes.Count(); i++)
            {

                double[] valorQuadro = new double[Cabecario.Length];
                restricoes[i].Valores.CopyTo(valorQuadro, 0);
                valorQuadro[restricoes[i].Valores.Length] = restricoes[i].Resultado;
                int auxiliares = quantA + quantF;
                for (int p = Cabecario.Length - auxiliares - 1; p < Cabecario.Length - 1; p++)
                {
                    if(valorQuadro[p] == 1)
                    {
                        novasLinhas.Add(Cabecario[p], valorQuadro);
                    }
                }
               
                f++;
            }
            
            Linhas = novasLinhas;
            MontarLinhaZ(FObjetiva);
            if (duasFases)
            {
                double[] linha = new double[Cabecario.Length];
                int aux = 1;
                List<int> indexesArtificiais = new List<int>();
                for (int i = 0; i < Cabecario.Length; i++)
                {
                    foreach (KeyValuePair<string, double[]> entry in this.Linhas)
                    {
                        //^a\d*
                        if(Regex.IsMatch(entry.Key, @"^a\d*"))
                        {
                            linha[i] = linha[i] + entry.Value[i];
                        }
                    }

                    linha[i] = linha[i] * -1;

                    if (Cabecario[i].Equals("a" + aux))
                    {
                        linha[i] = 0;
                        aux++;
                    }

                }
                this.LinhaZ2 = new KeyValuePair<string, double[]>("Z'", linha);

            }

        }

        public Quadro(Quadro quadro)
        {
            if (quadro != null)
            {
                if (quadro.Cabecario != null)
                {
                    this.Cabecario = (string[])quadro.Cabecario.Clone();
                    this.Linhas = new Dictionary<String, double[]>();
                    foreach (KeyValuePair<String, double[]> entry in quadro.Linhas)
                    {
                        this.Linhas.Add((string)entry.Key.Clone(), (double[])entry.Value.Clone());
                    }
                    this.LinhaZ = new KeyValuePair<string, double[]>((string)quadro.LinhaZ.Key.Clone(), (double[])quadro.LinhaZ.Value.Clone());
                    this.QuadroAnterior = new Quadro(quadro.QuadroAnterior);
                }

            }

        }

        public void MontarLinhaZ(FuncaoObjetiva fObjetiva)
        {
            double[] valorQuadro = new double[Cabecario.Length];

            for (int i = 0; i < Cabecario.Length; i++)
            {
                for (int j = 0; j < fObjetiva.Letras.Length; j++)
                {
                    if (fObjetiva.Letras[j].Equals(Cabecario[i]))
                    {
                        valorQuadro[i] = fObjetiva.Valores[j];
                    }
                }
            }
            this.LinhaZ = new KeyValuePair<string, double[]>("Z", valorQuadro);

        }

        public void Iteracao()
        {
            while (!IsUltimoQuadro())
            {
                int cp = ColunaPivo();
                string lp = LinhaPivo(cp);

                double numeroPivo = this.Linhas[lp][cp];

                double[] valoresNovaLinha = new double[this.Cabecario.Length];
                for (int i = 0; i < Cabecario.Length; i++)
                {
                    valoresNovaLinha[i] = this.Linhas[lp][i] / numeroPivo;
                }

                Dictionary<string, double[]> novasLinhas = new Dictionary<string, double[]>();

                foreach (KeyValuePair<string, double[]> entry in this.Linhas)
                {
                    if (entry.Key.Equals(lp))
                    {
                        novasLinhas.Add(this.Cabecario[cp], valoresNovaLinha);
                    }
                    else
                    {
                        double coeficienteCP = entry.Value[cp];
                        double[] novosValores = new double[entry.Value.Length];
                        for (int i = 0; i < novosValores.Length; i++)
                        {
                            novosValores[i] = entry.Value[i] - coeficienteCP * valoresNovaLinha[i];
                        }
                        novasLinhas.Add(entry.Key, novosValores);
                    }
                }

                double coeficienteCPZ = LinhaZ.Value[cp];
                double[] novaLinhaZ = new double[LinhaZ.Value.Length];
                for (int i = 0; i < novaLinhaZ.Length; i++)
                {
                    novaLinhaZ[i] = LinhaZ.Value[i] - coeficienteCPZ * valoresNovaLinha[i];
                }

                if (duasFases)
                {
                    double coeficienteCPZ2 = LinhaZ2.Value[cp];
                    double[] novaLinhaZ2 = new double[LinhaZ2.Value.Length];
                    for (int i = 0; i < novaLinhaZ2.Length; i++)
                    {
                        double divisor = coeficienteCPZ2 * valoresNovaLinha[i];
                        novaLinhaZ2[i] = (LinhaZ2.Value[i]) - divisor;
                    }
                    LinhaZ2 = new KeyValuePair<string, double[]>("Z2", novaLinhaZ2);

                }



                LinhaZ = new KeyValuePair<string, double[]>("Z", novaLinhaZ);
                this.Linhas = novasLinhas;
            }
        }
        public int ColunaPivo()
        {

            double menor = double.MaxValue;
            int indice = -1;
            if (duasFases)
            {
                for (int i = 0; i < LinhaZ2.Value.Length - 1; i++)
                {
                    if (LinhaZ2.Value[i] < menor)
                    {
                        menor = LinhaZ2.Value[i];
                        indice = i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < LinhaZ.Value.Length - 1; i++)
                {
                    if (LinhaZ.Value[i] < menor)
                    {
                        menor = LinhaZ.Value[i];
                        indice = i;
                    }
                }
            }
            
            return indice;
        }

        public string LinhaPivo(int indiceDaColuna)
        {
            double menor = double.MaxValue;
            string key = "";


            foreach (KeyValuePair<string, double[]> entry in this.Linhas)
            {
                double valor = entry.Value[entry.Value.Length - 1] / entry.Value[indiceDaColuna];

                if (valor < menor && valor > 0)
                {
                    menor = valor;
                    key = entry.Key;
                }
            }

            return key;
        }
        public void TerminaFase()
        {
            int aux = 1;
            List<int> indexesArtificiais = new List<int>();
            for (int i = 0; i < Cabecario.Length; i++)
            {
                if (this.Cabecario[i].Equals("a" + aux))
                {
                    indexesArtificiais.Add(i);
                    aux++;
                }
            }

            string[] novoCabecario = new string[Cabecario.Length - indexesArtificiais.Count()];
            List<double[]> valoresLinhas = new List<double[]>();
            List<double[]> novosValoresLinhas = new List<double[]>();
            List<string> novasChaves = new List<string>();
            double[] novaLinhaZ = new double[Cabecario.Length - indexesArtificiais.Count()];

            for (int j = 0; j < this.Linhas.Count(); j++)
            {
                novosValoresLinhas.Add(new double[this.Cabecario.Length - indexesArtificiais.Count()]);
            }
            foreach (KeyValuePair<String, double[]> entry in Linhas)
            {
                valoresLinhas.Add(entry.Value);
                novasChaves.Add(entry.Key);
            }
            aux = 0;
            for (int i = 0; i < Cabecario.Length; i++)
            {
                if (!indexesArtificiais.Contains(i))
                {
                    novoCabecario[aux] = Cabecario[i];
                    novaLinhaZ[aux] = LinhaZ.Value[i];

                    for (int j = 0; j < valoresLinhas.Count; j++)
                    {
                        novosValoresLinhas[j][aux] = valoresLinhas[j][i];
                    }

                    aux++;
                }
            }
            this.Cabecario = novoCabecario;
            this.LinhaZ = new KeyValuePair<string, double[]>("Z", novaLinhaZ);
            Dictionary<String, double[]> novasLinhas = new Dictionary<string, double[]>();
            for (int i = 0; i < this.Linhas.Count(); i++)
            {
                novasLinhas.Add(novasChaves[i], novosValoresLinhas[i]);
            }
            this.Linhas = novasLinhas;

        }

        public bool AcabouDuasFases()
        {
            int aux = 1;
            List<int> indexesArtificiais = new List<int>();
            for (int i = 0; i < Cabecario.Length; i++)
            {
                if (this.Cabecario[i].Equals("a" + aux))
                {
                    indexesArtificiais.Add(i);
                    aux++;
                }
            }
            int cont = 0;
            foreach (int valor in indexesArtificiais)
            {
                if (this.LinhaZ2.Value[valor] > 0.99)
                {
                    cont++;
                }
            }

            if (cont < indexesArtificiais.Count())
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool IsUltimoQuadro()
        {
            if (duasFases)
            {
                if (AcabouDuasFases())
                {
                    duasFases = false;
                    TerminaFase();
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else if (!(QuadroAnterior == null))
            {
                if (!(QuadroAnterior.QuadroAnterior == null))
                {
                    if (QuadroAnterior.QuadroAnterior.Cabecario != null)
                    {
                        int soma = 0;
                        for (int i = 0; i < this.LinhaZ.Value.Length; i++)
                        {
                            if (Math.Floor(this.LinhaZ.Value[i]) == Math.Floor(QuadroAnterior.QuadroAnterior.LinhaZ.Value[i]))
                            {
                                soma++;
                            }
                        }


                        if (soma == this.LinhaZ.Value.Length)
                        {
                            this.Cabecario = this.QuadroAnterior.Cabecario;
                            this.Linhas = this.QuadroAnterior.Linhas;
                            this.LinhaZ = this.QuadroAnterior.LinhaZ;
                            this.QuadroAnterior = this.QuadroAnterior.QuadroAnterior;
                            return true;
                        }
                    }

                }

            }

            foreach (double valor in LinhaZ.Value)
            {
                if (valor < 0)
                {
                    QuadroAnterior = new Quadro(this);
                    return false;
                }
            }

            return true;

        }
    }
}