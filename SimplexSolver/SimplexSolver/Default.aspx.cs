using SimplexSolver.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace SimplexSolver
{
    public partial class _Default : Page
    {
        private Simplex simplex;
        private FuncaoObjetiva funcObjetiva;
        private Restricao restricao;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnResolverSimplex_Click(object sender, EventArgs e)
        {
            simplex = new Simplex();

            if (txtEqBase.Text == "" || hidenRestricoes.Value == "")
            {
                MostraErro();
                return;
            }

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
            if (cmbTipoFunc.SelectedValue.Equals(1))
            {
                isMaximizacao = false;
            }

            for (int k = 0; k < eqFuncObjetiva.Length; k++)
            {
                if (eqFuncObjetiva[k] == "-" || eqFuncObjetiva[k] == "+")
                {
                    if (eqFuncObjetiva[k] == "-")
                    {
                        negativo = true;
                    }
                    else
                    {
                        negativo = false;
                    }
                }
                else
                {
                    String[] separadoObjetiva = System.Text.RegularExpressions.Regex.Split(eqFuncObjetiva[k], @"[^\d]");
                    double valor;

                    //pega o valor
                    if (separadoObjetiva[0] == "")
                    {
                        valor = Convert.ToDouble("1");
                    }
                    else if (double.TryParse(separadoObjetiva[0], out valor))
                    {
                        valor = Convert.ToDouble(separadoObjetiva[0]);
                    }
                    else
                    {
                        valor = Convert.ToDouble(separadoObjetiva[0]);
                    }
                    //verifica se a string anterior foi um sinal negativo, caso seja, o valor é negativado
                    if (negativo)
                    {
                        valores.Add(Convert.ToDouble("-" + valor));
                        negativo = false;
                    }
                    else
                    {
                        valores.Add(valor);
                    }

                    //pega a variável
                    string variavelFuncObjetiva;
                    if (separadoObjetiva[0] == "")
                    {
                        variavelFuncObjetiva = eqFuncObjetiva[k];
                        if (variavelFuncObjetiva != "")
                        {
                            variaveis.Add(variavelFuncObjetiva);
                        }
                    }
                    else
                    {
                        var match = Regex.Match(eqFuncObjetiva[k], @"[^\d]");
                        int indiceValorFuncObjetiva = eqFuncObjetiva[k].IndexOf(match.Value);
                        variavelFuncObjetiva = eqFuncObjetiva[k].Substring(indiceValorFuncObjetiva);
                        if (variavelFuncObjetiva != "")
                        {
                            variaveis.Add(variavelFuncObjetiva);
                        }
                    }
                    
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
                    if (eqRestricao[j] == "-" || eqRestricao[j] == "+")
                    {
                        if (eqRestricao[j] == "-")
                        {
                            negativo = true;
                        }
                        else
                        {
                            negativo = false;
                        }
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
                        double valor;

                        //pega o valor
                        if (separado[0] == "")
                        {
                            valor = Convert.ToDouble("1");
                        }
                        else if (double.TryParse(separado[0], out valor))
                        {
                            valor = Convert.ToDouble(separado[0]);
                        }
                        else
                        {
                            valor = Convert.ToDouble(separado[0]);
                        }
                        //verifica se a string anterior foi um sinal negativo, caso seja, o valor é negativado
                        if (negativo)
                        {
                            valores.Add(Convert.ToDouble("-" + valor));
                            negativo = false;
                        }
                        else
                        {
                            valores.Add(valor);
                        }

                        //pega a variável
                        if (separado[0] == "")
                        {
                            string variavel = eqRestricao[j];
                            if (variavel != "")
                            {
                                variaveis.Add(variavel);
                            }
                        }
                        else
                        {
                            var match = Regex.Match(eqRestricao[j], @"[^\d]");
                            int indiceValor = eqRestricao[j].IndexOf(match.Value);
                            string variavel = eqRestricao[j].Substring(indiceValor);
                            if (variavel != "")
                            {
                                variaveis.Add(variavel);
                            }
                        }
                    }
                }
                //pega os valores da equação e monta a restriçao
                arrayVariaveis = variaveis.ToArray(typeof(string)) as string[];
                arrayValores = valores.ToArray(typeof(double)) as double[];

                restricao = new Restricao(arrayValores, arrayVariaveis, sinalRestricaoo, resultadoRestricao);
                simplex.Restricoes.Add(restricao);
            }

            //Normaliza e monta o quadro
            simplex.Normaliza();
            simplex.MontaQuadro();
            simplex.QuadroSimplex.Iteracao();

            lblResultados.Text = "Resultados: \n\n";
            foreach (KeyValuePair<string, double> entry in simplex.Solucao())
            {
                lblResultados.Text += "variável " + entry.Key + " valendo " + entry.Value + "\n ";
            }
        }

        private void MostraErro()
        {
            pnlInfo.Visible = true;
            lblMsg.Text = "Os campos não estão preenchidos ou foram preenchidos incorretamente. Preencha-os ou verifique os padrões!";
        }
    }
}