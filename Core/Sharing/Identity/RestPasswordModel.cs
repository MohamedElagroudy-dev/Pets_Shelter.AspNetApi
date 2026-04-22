﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing.Identity
{
    public record ActiveAccountModel
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}