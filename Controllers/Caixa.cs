using CaixaWEB.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaixaWEB.Controllers
{
    public class Caixa
    {
        public static Tuple<int, int, int, int, int, int> GeraNotas()
        {
            Random rnd = new Random();
            int n100 = rnd.Next(0, 20), n50 = rnd.Next(0, 20), n20 = rnd.Next(0, 20), n10 = rnd.Next(0, 20), n5 = rnd.Next(0, 20), n2 = rnd.Next(0, 20);

            var tuple = new Tuple<int, int, int, int, int, int>(n100, n50, n20, n10, n5, n2);

            return tuple;
        }

        public static Tuple<double, Tuple<int, int, int, int, int, int, string>> CaixaEletronico(double valor, CaixaModel not, double saldo)
        {
            int n100 = not.N100, n50 = not.N50, n20 = not.N20, n10 = not.N10, n5 = not.N5, n2 = not.N2;

            int n100t = 0, n50t = 0, n20t = 0, n10t = 0, n5t = 0, n2t = 0;

            string notas = "";

            bool suficiente = true;

            int somaNotas = (100 * n100) + (50 * n50) + (20 * n20) + (10 * n10) + (5 * n5) + (2 * n2);

            if (valor > somaNotas)
            {
                suficiente = false;
            }
            else
            {
                while (valor % 5 != 0 && n2 > 0 && saldo > 0)
                {
                    valor = valor - 2;
                    saldo -= 2;
                    n2t++;
                }
                if (valor % 5 != 0 || saldo - valor < 0)
                {
                    suficiente = false;
                }
                else
                {
                    while (valor > 0)
                    {
                        if (valor % 100 == 0 && n100 > 0 && saldo - 100 >= 0)
                        {
                            saldo -= 100;
                            valor = valor - 100;
                            n100t++;
                        }
                        else if (valor % 50 == 0 && n50 > 0 && saldo - 50 >= 0)
                        {
                            saldo -= 50;
                            valor = valor - 50;
                            n50t++;
                        }
                        else if (valor % 20 == 0 && n20 > 0 && saldo - 20 >= 0)
                        {
                            saldo -= 20;
                            valor = valor - 20;
                            n20t++;
                        }
                        else if (valor % 10 == 0 && n10 > 0 && saldo - 10 >= 0)
                        {
                            saldo -= 10;
                            valor = valor - 10;
                            n10t++;
                        }
                        else if (valor % 5 == 0 && n5 > 0 && saldo - 5 >= 0)
                        {
                            saldo -= 5;
                            valor = valor - 5;
                            n5t++;
                        }
                        else if (valor % 2 == 0 && n2 > 0 && saldo - 2 >= 0)
                        {
                            saldo -= 2;
                            valor = valor - 2;
                            n2t++;
                        }
                        else
                        {
                            suficiente = false;
                            break;
                        }
                    }
                }
            }

            if (suficiente)
            {
                n100 -= n100t; n50 -= n50t; n20 -= n20t; n10 -= n10t; n5 -= n5t; n2 -= n2t;
                notas = "Notas sacadas: Notas 100: " + n100t +
                    " / Notas 50: " + n50t +
                    " / Notas 20: " + n20t +
                    " / Notas 10: " + n10t +
                    " / Notas 5: " + n5t +
                    " / Notas 2: " + n2t;
            }
            else
            {
                notas = "Notas insuficientes :(";
            }

            var ret = new Tuple<int, int, int, int, int, int, string>(n100, n50, n20, n10, n5, n2, notas);

            var retorno = new Tuple<double, Tuple<int, int, int, int, int, int, string>>(saldo, ret);

            return retorno;
        }

        public static Tuple<double, bool, Tuple<int, int, int, int, int, int, string>> Deposit(double value, double balance, CaixaModel bills)
        {
            int n100 = bills.N100, n50 = bills.N50, n20 = bills.N20, n10 = bills.N10, n5 = bills.N5, n2 = bills.N2;
            int n100t = 0, n50t = 0, n20t = 0, n10t = 0, n5t = 0, n2t = 0;

            string depo = "";
            bool possib = true;

            if (value % 5 != 0)
            {
                while (value % 5 != 0 && value > 0)
                {
                    value -= 2;
                    balance += 2;
                    n2t += 1;
                    if (value < 0)
                    {
                        possib = false;
                        break;
                    }
                }
            }
            if (value - 2 > 0)
            {
                while (value > 0)
                {

                    if (value % 100 == 0)
                    {
                        value -= 100;
                        balance += 100;
                        n100t += 1;
                    }
                    else if (value % 50 == 0)
                    {
                        value -= 50;
                        balance += 50;
                        n50t += 1;
                    }
                    else if (value % 20 == 0)
                    {
                        value -= 20;
                        balance += 20;
                        n20t += 1;
                    }
                    else if (value % 10 == 0)
                    {
                        value -= 10;
                        balance += 10;
                        n10t += 1;
                    }
                    else if (value % 5 == 0)
                    {
                        value -= 5;
                        balance += 5;
                        n5t += 1;
                    }
                    else if (value % 2 == 0)
                    {
                        value -= 2;
                        balance += 2;
                        n2t += 1;
                    }
                    else
                    {
                        possib = false;
                    }
                }

            }

            if (possib)
            {
                n100 += n100t; n50 += n50t; n20 += n20t; n10 += n10t; n5 += n5t; n2 += n2t;
                depo = "Deposito efetuado com sucesso!";
            }
            else
            {
                depo = "Depósito inválido :(";
            }

            var ret = new Tuple<int, int, int, int, int, int, string>(n100, n50, n20, n10, n5, n2, depo);

            var retorno = new Tuple<double, bool, Tuple<int, int, int, int, int, int, string>>(balance, possib, ret);
                        
            return retorno;
        }
    }
}
