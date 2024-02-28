﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClassConnect.Models
{
    public class LoginorSignUp
    {
        
        [Remote(action:"UsernameExists",controller:"Instructor")]
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string FullName { get; set; }
        
        public string Password { get; set; }
        
        public bool Remember { get; set; }
    }
}