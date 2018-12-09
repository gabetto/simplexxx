using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplexSolver.Classes
{
    public class Restricao
    {
        public string[] Letras { get; set; }
        public double[] Valores { get; set; }
        public string Sinal { get; set; }
        public double Resultado { get; set; }
        private Boolean isNormalizada;


        public Restricao(double[] valores, string[] letras, string sinal, double resultado)
        {
            this.Letras = letras;
            this.Valores = valores;
            this.Sinal = sinal;
            this.Resultado = resultado;
        }

        public void Normaliza(int quantF, int quantA, int numeroF, int numeroA, string[] letras)
        {
            if (!this.isNormalizada)
            {

                int j = 1;
                int k = 1;
                string[] novasLetras = new string[letras.Length + quantF + quantA];
                letras.CopyTo(novasLetras, 0);
                for (int i = letras.Length; i < novasLetras.Length; i++)
                {
                    if (j <= quantF)
                    {

                        novasLetras[i] = "f" + j;
                        j++;
                    }
                    else if (k <= quantA)
                    {
                        novasLetras[i] = "a" + k;
                        k++;
                    }

                }
                string[] antigasLetras = this.Letras;
                this.Letras = novasLetras;

                double[] novosValores = new double[this.Letras.Length];

                for (int n = 0; n < Letras.Length; n++)
                {
                    for (int m = 0; m < antigasLetras.Length; m++)
                    {
                        if (antigasLetras[m].Equals(Letras[n]))
                        {
                            novosValores[n] = this.Valores[m];
                        }
                    }
                }


                if (this.Sinal.Equals("<="))
                {



                    int index = 0;
                    for (int i = 0; i < Letras.Length; i++)
                    {
                        if (Letras[i].Equals("f" + numeroF))
                        {
                            index = i;
                            break;
                        }
                    }

                    for (int i = 0; i < novosValores.Length; i++)
                    {
                        if (i == index)
                        {
                            novosValores[i] = 1;
                        }
                    }

                    this.Valores = novosValores;

                }
                else if (this.Sinal.Equals(">="))
                {

                    int indexA = 0;
                    int indexF = 0;

                    for (int i = 0; i < Letras.Length; i++)
                    {
                        if (Letras[i].Equals("f" + numeroF))
                        {
                            indexF = i;
                        }
                        if (Letras[i].Equals("a" + numeroA))
                        {
                            indexA = i;
                        }
                    }


                    for (int i = Valores.Length - 1; i < novosValores.Length; i++)
                    {
                        if (i == indexF)
                        {
                            novosValores[i] = -1;
                        }
                        else if (i == indexA)
                        {
                            novosValores[i] = 1;
                        }

                    }

                    this.Valores = novosValores;

                }
                else if (this.Sinal.Equals("="))
                {
                    int index = 0;
                    for (int i = 0; i < Letras.Length; i++)
                    {
                        if (Letras[i].Equals("a" + numeroA))
                        {
                            index = i;
                            break;
                        }
                    }

                    for (int i = Valores.Length - 1; i < novosValores.Length; i++)
                    {
                        if (i == index)
                        {
                            novosValores[i] = 1;
                        }
                    }

                    this.Valores = novosValores;
                }

                this.Sinal = "=";
                this.isNormalizada = true;
            }

        }
    }
}