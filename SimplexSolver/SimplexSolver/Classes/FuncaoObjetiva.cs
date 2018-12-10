using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplexSolver.Classes
{
    public class FuncaoObjetiva
    {
        public Boolean isMaximizacao { get; }
        public string[] Letras { get; }
        public double[] Valores { get; }
        private Boolean isNormalizada;


        public FuncaoObjetiva(double[] valores, string[] letras, Boolean maximazacao)
        {
            this.isMaximizacao = maximazacao;
            this.Letras = letras;
            this.Valores = valores;
            this.isNormalizada = false;
        }

        public void NormalizaFuncao()
        {
            if (!this.isNormalizada)
            {
                if (isMaximizacao)
                {
                    for (int i = 0; i < this.Valores.Length; i++)
                    {
                        this.Valores[i] = -this.Valores[i];
                    }
                }
                
                this.isNormalizada = true;
            }

        }

        public int Tamanho()
        {
            return this.Valores.Length;
        }
    }
}