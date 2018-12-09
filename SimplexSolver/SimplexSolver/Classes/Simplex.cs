using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplexSolver.Classes
{
    public class Simplex
    {
        public List<Restricao> Restricoes { get; set; }
        public FuncaoObjetiva FObjetiva { get; set; }
        private Boolean isNormalizado;
        public Quadro QuadroSimplex { get; set; }
        private int QuantF;
        private int QuantA;


        public Simplex()
        {
            this.isNormalizado = false;
            Restricoes = new List<Restricao>();

        }

        public void Normaliza()
        {
            if (!this.isNormalizado)
            {
                string[] letras = this.FObjetiva.Letras;

                int j = 0;
                int i = 0;
                for (i = 0; i < Restricoes.Count; i++)
                {
                    if (Restricoes[i].Sinal.Equals("=") || Restricoes[i].Sinal.Equals(">="))
                    {
                        j++;
                    }

                }

                QuantF = i;
                QuantA = j;
                int u = 0;

                for (int y = 0; y < Restricoes.Count; y++)
                {
                    if (Restricoes[y].Sinal.Equals("=") || Restricoes[y].Sinal.Equals(">="))
                    {
                        u++;
                    }

                    Restricoes[y].Normaliza(QuantF, QuantA, (y + 1), (u + 1), letras);

                }

                FObjetiva.NormalizaFuncao();

                this.isNormalizado = true;
            }
        }

        public void MontaQuadro()
        {
            if (isNormalizado)
            {
                this.QuadroSimplex = new Quadro(this.FObjetiva, this.Restricoes, this.QuantA, this.QuantF);
            }
        }

        public Dictionary<string, double> Solucao()
        {
            Dictionary<string, double> solucao = new Dictionary<string, double>();
            solucao.Add("Z", this.QuadroSimplex.LinhaZ.Value[this.QuadroSimplex.LinhaZ.Value.Length - 1]);

            foreach (KeyValuePair<string, double[]> entry in QuadroSimplex.Linhas)
            {
                solucao.Add(entry.Key, entry.Value[entry.Value.Length - 1]);
            }

            return solucao;
        }
    }
}