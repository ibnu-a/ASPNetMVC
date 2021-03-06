﻿using ASPNetMVC.Context;
using ASPNetMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ASPNetMVC.Controllers
{
    public class AccountsController : Controller
    {
        private MyContext myContext = new MyContext();
        // GET: Accounts
        public ActionResult Index()
        {
            if(Session["Id"] != null)
            {
                return RedirectToAction("Index","Home", new { area = "" });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //GET : Register
        public ActionResult Register()
        {
            List<Division> list = myContext.Divisions.ToList();
            ViewBag.DivisionList = new SelectList(list, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account account, Employee employee)
        {
            if (ModelState.IsValid)
            {
                var check = myContext.Employees.FirstOrDefault(s => s.Email == employee.Email);
                if(check == null)
                {
                    account.Password = GetMD5(account.Password);
                    myContext.Configuration.ValidateOnSaveEnabled = false;
                    myContext.Employees.Add(employee);
                    myContext.Accounts.Add(account);
                    myContext.SaveChanges();
                    return RedirectToAction("Login","Accounts", new { area = "" });
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var l_password = GetMD5(password);
                var dEmployees = myContext.Employees.Where(e => e.Email.Equals(email)).ToList();
                var dAccounts = myContext.Accounts.Where(s => s.Password.Equals(l_password)).ToList();
                if(dEmployees.Count() > 0 && dAccounts.Count() > 0)
                {
                    //add session
                    Session["FullName"] = dEmployees.FirstOrDefault().Name;
                    Session["Email"] = dEmployees.FirstOrDefault().Email;
                    Session["Id"] = dAccounts.FirstOrDefault().Id;
                    return RedirectToAction("Index","Employees", new { area = ""});
                }
                else
                {
                    ViewBag.error = "Login Failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

        private string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for(int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}