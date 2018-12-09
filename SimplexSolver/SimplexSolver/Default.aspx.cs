using SimplexSolver.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text.RegularExpressions;

namespace SimplexSolver
{
    public partial class _Default : Page
    {
        private Simplex simplex;
        private FuncaoObjetiva funcObjetiva;
        private Restricao restricao;

        protected void Page_Load(object sender, EventArgs e)
        {
            simplex = new Simplex();

        }

        protected void btnResolverSimplex_Click(object sender, EventArgs e)
        {
            //variaveis gerais
            string[] arrayVariaveis;
            double[] arrayValores;

            //variaveis para função objetiva
            Boolean isMaximizacao = true;
            string[] eqFuncObjetiva = txtEqBase.Text.Split(' ');

            //variáveis para restrição
            string[] equacoes = hidenRestricoes.Value.Split('@');
            string[] eqRestricao;
            ArrayList valores = new ArrayList();
            ArrayList variaveis = new ArrayList();
            double resultadoRestricao = 0.0f;
            string sinalRestricaoo = "";
            Boolean negativo = false;


            //Resolve a função objetiva
            if (cmbTipoFunc.SelectedValue.Equals(1)) isMaximizacao = false;
            for(int k = 0; k< eqFuncObjetiva.Length; k++)
            {
                if (eqFuncObjetiva[k] == "-")
                {
                    negativo = true;
                }
                else
                {
                    String[] separadoObjetiva = System.Text.RegularExpressions.Regex.Split(eqFuncObjetiva[k], @"[^\d]");
                    if (separadoObjetiva[0] != "")
                    {
                        double valor = Convert.ToDouble(separadoObjetiva[0]);

                        if (negativo)
                        {
                            valores.Add(Convert.ToDouble("-" + valor));
                            negativo = false;
                        }
                        else
                        {
                            valores.Add(valor);
                        }
                    }

                    int indiceValor = eqFuncObjetiva[k].IndexOf(separadoObjetiva[0]);
                    string variavel = eqFuncObjetiva[k].Substring(indiceValor + 1);
                    if (variavel != "") variaveis.Add(variavel);

                }
            }
            arrayVariaveis = variaveis.ToArray(typeof(string)) as string[];
            arrayValores = valores.ToArray(typeof(double)) as double[];
            funcObjetiva = new FuncaoObjetiva(arrayValores, arrayVariaveis, isMaximizacao);
            simplex.FObjetiva = funcObjetiva;

            
            //Resolve as restrições
            for (int i = 0; i < equacoes.Length; i++)
            {
                //limpa os valores e variáveis para reaproveitar pra cada restrição
                valores.Clear();
                variaveis.Clear();

                eqRestricao = equacoes[i].Split(' ');
                for (int j = 0; j < eqRestricao.Length; j++)
                {
                    if (eqRestricao[j] == "-")
                    {
                        negativo = true;
                    }
                    else if (eqRestricao[j] == "<=" || eqRestricao[j] == ">=" || eqRestricao[j] == "=" ||
                        eqRestricao[j] == "<" || eqRestricao[j] == ">")
                    {
                        sinalRestricaoo = eqRestricao[j];
                    }
                    else if (double.TryParse(eqRestricao[j], out resultadoRestricao))
                    {
                        Console.WriteLine(resultadoRestricao);
                    }
                    else
                    {
                        String[] separado = System.Text.RegularExpressions.Regex.Split(eqRestricao[j], @"[^\d]");
                        if (separado[0] != "")
                        {
                            double valor = Convert.ToDouble(separado[0]);

                            if (negativo)
                            {
                                valores.Add(Convert.ToDouble("-" + valor));
                                negativo = false;
                            }
                            else
                            {
                                valores.Add(valor);
                            }
                        }
                        
                        int indiceValor = eqRestricao[j].IndexOf(separado[0]);
                        string variavel = eqRestricao[j].Substring(indiceValor + 1);
                        if (variavel != "") variaveis.Add(variavel);
                        
                    }
                    
                }
                arrayVariaveis = variaveis.ToArray(typeof(string)) as string[];
                arrayValores = valores.ToArray(typeof(double)) as double[];

                restricao = new Restricao(arrayValores, arrayVariaveis, sinalRestricaoo, resultadoRestricao);
                simplex.Restricoes.Add(restricao);
            }

            //Normaliza e monta o quadro
            simplex.Normaliza();
            simplex.MontaQuadro();

            lblResultados.Text = "Resultados: \n\n";
            foreach (KeyValuePair<string, double> entry in simplex.Solucao())
            {
                lblResultados.Text += "variável " + entry.Key + " valendo " + entry.Value + "\n ";
            }
        }
    }
}