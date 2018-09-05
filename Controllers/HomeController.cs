using System;
using CaixaWEB.Models;
using CaixaWEB.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaixaWEB.Controllers
{
    public class HomeController : Controller
    {
        private IUserRepository<User> repo;

        public HomeController(IUserRepository<User> repository)
        {
            repo = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            var user = new User();

            return View(user);
        }

        [HttpPost]
        public IActionResult Index(string login, string psw)
        {
            var findUser = repo.FindUser(login);

            if (findUser == null)
            {
                @ViewData["LoginError"] = "Usuário não encontrado";
                return View();
            }
            else if (findUser.Login == login && findUser.Password == psw)
            {
                var resultN = Caixa.GeraNotas();
                var caixaModel = new CaixaModel
                {
                    N100 = resultN.Item1,
                    N50 = resultN.Item2,
                    N20 = resultN.Item3,
                    N10 = resultN.Item4,
                    N5 = resultN.Item5,
                    N2 = resultN.Item6
                };
                HttpContext.Session.SetObjectAsJson("Notas", caixaModel);

                var log = new LogModel
                {
                    UserId = findUser.Id,
                    Details = "Usuário acessou o sistema",
                    Date = DateTime.Now
                };
                repo.SaveLog(log);

                var user = new User
                {
                    AccountBalance = findUser.AccountBalance,
                    Id = findUser.Id,
                    Login = findUser.Login,
                    Password = findUser.Password
                };
                caixaModel.User = user;
                HttpContext.Session.SetObjectAsJson("User", user);

                return Redirect("Home/Options");
            }
            else
            {
                ViewData["LoginError"] = "Senha incorreta";
                return View();
            }
        }

        public IActionResult LogOut()
        {
            var user = HttpContext.Session.GetObjectFromJson<User>("User");
            var log = new LogModel
            {
                UserId = user.Id,
                Details = "Usuário saiu do sistema",
                Date = DateTime.Now
            };
            repo.SaveLog(log);
            user = null;
            HttpContext.Session.SetObjectAsJson("User", user);

            return RedirectToAction("Index");
        }

        public IActionResult Withdraw()
        {
            var user = HttpContext.Session.GetObjectFromJson<User>("User");
            if (user != null)
            {
                ViewData["Saldo"] = user.AccountBalance;
                var bills = HttpContext.Session.GetObjectFromJson<CaixaModel>("Notas");
                var caixaModel = new CaixaModel
                {
                    N100 = bills.N100,
                    N50 = bills.N50,
                    N20 = bills.N20,
                    N10 = bills.N10,
                    N5 = bills.N5,
                    N2 = bills.N2,
                    User = user
                };
                return View(caixaModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult WithdrawAjax(double valor)
        {
            var bills = HttpContext.Session.GetObjectFromJson<CaixaModel>("Notas");
            var user = HttpContext.Session.GetObjectFromJson<User>("User");

            var result = Caixa.CaixaEletronico(valor, bills, user.AccountBalance);

            var caixaModel = new CaixaModel
            {
                N100 = result.Item2.Item1,
                N50 = result.Item2.Item2,
                N20 = result.Item2.Item3,
                N10 = result.Item2.Item4,
                N5 = result.Item2.Item5,
                N2 = result.Item2.Item6,
                Resultado = result.Item2.Item7,
                User = user
            };

            if (Convert.ToString(valor) == String.Empty || valor <= 0)
            {
                caixaModel.Resultado = "Valor inválido";
                return Json(caixaModel);

            }
            else
            {
                user.AccountBalance = result.Item1;

                repo.Update(user.Id, user.AccountBalance);

                HttpContext.Session.SetObjectAsJson("Notas", caixaModel);
                HttpContext.Session.SetObjectAsJson("User", user);
                ViewData["Saldo"] = user.AccountBalance;

                var log = new LogModel
                {
                    UserId = user.Id,
                    Details = string.Format("Valor sacado R$ {0}", valor),
                    Date = DateTime.Now
                };
                repo.SaveLog(log);

                return Json(caixaModel);
            }
        }

        public IActionResult Deposit()
        {
            var bills = HttpContext.Session.GetObjectFromJson<CaixaModel>("Notas");
            var user = HttpContext.Session.GetObjectFromJson<User>("User");
            if (user != null)
            {
                ViewData["Saldo"] = user.AccountBalance;
                var caixaModel = new CaixaModel();
                return View(caixaModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult DepositAjax(double value)
        {

            var bills = HttpContext.Session.GetObjectFromJson<CaixaModel>("Notas");
            var user = HttpContext.Session.GetObjectFromJson<User>("User");

            var result = Caixa.Deposit(value, user.AccountBalance, bills);
            var possib = result.Item2;

            var caixaModel = new CaixaModel
            {
                N100 = result.Item3.Item1,
                N50 = result.Item3.Item2,
                N20 = result.Item3.Item3,
                N10 = result.Item3.Item4,
                N5 = result.Item3.Item5,
                N2 = result.Item3.Item6,
                Deposit = result.Item3.Item7,
                User = user
            };

            if (Convert.ToString(value) == String.Empty || value <= 0)
            {
                caixaModel.Deposit = "Valor inválido";
                return Json(caixaModel);
            }

            else if (possib)
            {
                var log = new LogModel
                {
                    UserId = user.Id,
                    Details = string.Format("Valor depositado R$ {0}", value),
                    Date = DateTime.Now
                };
                repo.SaveLog(log);
                repo.Update(user.Id, result.Item1);
                user.AccountBalance = result.Item1;
            }

            ViewData["Saldo"] = user.AccountBalance;
            HttpContext.Session.SetObjectAsJson("Notas", caixaModel);
            HttpContext.Session.SetObjectAsJson("User", user);

            return Json(caixaModel);
        }

        public IActionResult Options()
        {
            var user = HttpContext.Session.GetObjectFromJson<User>("User");
            if (user != null)
            {
                ViewData["Saldo"] = user.AccountBalance;
                return View(user);
            }
            else
            {
                return View(user);
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string login, string psw, string confpsw)
        {
            var user = new User();
            if (login.Length < 5)
            {
                ViewData["Error"] = "Login precisa ter mais que 5 caracteres";
            }
            else if (psw.Length < 6)
            {
                ViewData["Error"] = "Senha precisa ter mais que 6 caracteres";
            }
            else if (psw != confpsw)
            {
                ViewData["Error"] = "Senhas não conferem";
            }
            else
            {
                Random rnd = new Random();
                user = new User
                {
                    Login = login,
                    Password = psw,
                    AccountBalance = rnd.Next(1000, 10000)
                };

                repo.Save(new User() { Login = user.Login, Password = user.Password, AccountBalance = user.AccountBalance });
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public IActionResult LogInfo()
        {
            var user = HttpContext.Session.GetObjectFromJson<User>("User");
            if (user != null)
            {
                var log = HttpContext.Session.GetObjectFromJson<LogModel>("log");

                var result = repo.UserLog(user.Id);

                return View(result);
            }
            else
            {
                
                return RedirectToAction("Index");
            }

        }

    }
}
