﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using testandoBancodDo0.Context;
using testandoBancodDo0.Models;

namespace testandoBancodDo0.Controllers
{
    public class CadastroController : Controller
    {
        private readonly AproveDbContext _dbContext;

        public CadastroController(AproveDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        public IActionResult Cadastrar([Bind("Name,Email,Senha")] UsuarioModel model, string confirmPassword)
        {
            try
            {
                // Verificar se as senhas coincidem
                if (model.Senha != confirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "As senhas não São iguais. Por favor, insira senhas iguais.");
                    return View("/Views/Site/Cadastro.cshtml", model);
                }

                //Verifica se ja existe um email parecido
                var UsuarioExistente = _dbContext.usuarios.FirstOrDefault(u => u.Email == model.Email);
                if (UsuarioExistente != null)
                {
                    ModelState.AddModelError(string.Empty, "O E-mail digitado já esta sendo utilizado, Tente outro novamente.");
                    return View("/Views/Site/Cadastro.cshtml", model);
                }

                //verifica se contem caracter especial
                if (!CaracterEspecial(model.Senha))
                {
                    ModelState.AddModelError(string.Empty, "As senhas precisam conter um caracter especial, Ex:'@'");
                    return View("/Views/Site/Cadastro.cshtml", model);
                }


                if (ModelState.IsValid)
                {
                    // gera nome, email e senha para jogar no banco de dados
                    var novoUsuario = new UsuarioModel
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Senha = model.Senha
                    };
                    //adicionando hash na senha do usuario
                    novoUsuario.SetSenhaHash();

                    // Adicione o novo usuário ao banco de dados
                    _dbContext.usuarios.Add(novoUsuario);

                    // Salva as alterações no banco de dados
                    _dbContext.SaveChanges();

                    // Redireciona para a página login após cadastrar
                    return RedirectToAction("Login", "Site");
                }

                // Se houver erros de validação, retorna a página de cadastro com os erros
                Console.WriteLine("Deu erro aqui camarada"); // ajustar essa mensagem de erro.
                return View("/Views/Site/Cadastro.cshtml", model);
            }
            catch (Exception ex)
            {
                // erros para ajudar na depuração
                Console.WriteLine($"Erro ao cadastrar: {ex.Message}");
                return View();
            }
        }

        //funçao para caracteres especiais
        public bool CaracterEspecial(string senha)
        {
            char[] caracteresEspeciais = { '@', '#', '$', '%', '&', '*', '(', ')', '[', ']', '{', '}', '!', '?' };
            return senha.Any(c => caracteresEspeciais.Contains(c));
        }




        [HttpPost]
        public IActionResult SolicitarReceita([Bind("Titulo,Descricao,Ingredientes")] ReceitaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuarioID = HttpContext.Session.GetString("UserId");


                    // gera titulo,descriçao e ingredientes para jogar no banco de dados
                    var novaReceita = new ReceitaModel
                    {
                        Titulo = model.Titulo,
                        Descricao = model.Descricao,
                        Ingredientes = model.Ingredientes,
                        IdUsuario = usuarioID

                    };

                    // Adicione o novo usuário ao banco de dados
                    _dbContext.receitas.Add(novaReceita);

                    // Salva as alterações no banco de dados
                    _dbContext.SaveChanges();


                    // Se as credenciais forem válidas, redireciona para a página principal
                    return RedirectToAction("Home", "Site");
                }

                // Se houver erros de validação, retorna a página de cadastro com os erros
                Console.WriteLine("Deu erro aqui camarada"); //ajustar essa mensagem de erro.
                return View("/Views/Site/CadastrarReceita.cshtml", model);
            }
            catch (Exception ex)
            {
                // erros para ajudar na depuração
                Console.WriteLine($"Erro ao cadastrar: {ex.Message}");
                return View();
            }
        }

    }
}



