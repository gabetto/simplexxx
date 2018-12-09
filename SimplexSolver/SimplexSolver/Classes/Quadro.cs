using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplexSolver.Classes
{
    public class Quadro
    {
        public Dictionary<String, double[]> Linhas { get; set; }
        public string[] Cabecario { get; set; }
        public KeyValuePair<String, double[]> LinhaZ { get; set; }


        public Quadro(FuncaoObjetiva FObjetiva, List<Restricao> restricoes, int quantA, int quantF)
        {
            int tamanho = FObjetiva.Tamanho();
            tamanho = tamanho + quantA + quantF + 1;

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
            for (int i = 0; i < quantF; i++)
            {

                double[] valorQuadro = new double[Cabecario.Length];
                restricoes[i].Valores.CopyTo(valorQuadro, 0);
                valorQuadro[restricoes[i].Valores.Length] = restricoes[i].Resultado;
                novasLinhas.Add("f" + f, valorQuadro);
                f++;
            }

            Linhas = novasLinhas;
            MontarLinhaZ(FObjetiva);

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

                LinhaZ = new KeyValuePair<string, double[]>("Z", novaLinhaZ);
                this.Linhas = novasLinhas;
            }
        }
        public int ColunaPivo()
        {

            double menor = 0;
            int indice = -1;

            for (int i = 0; i < LinhaZ.Value.Length; i++)
            {
                if (LinhaZ.Value[i] < menor)
                {
                    menor = LinhaZ.Value[i];
                    indice = i;
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

                if (valor < menor)
                {
                    menor = valor;
                    key = entry.Key;
                }
            }

            return key;
        }

        public bool IsUltimoQuadro()
        {
            foreach (double valor in LinhaZ.Value)
            {
                if (valor < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}