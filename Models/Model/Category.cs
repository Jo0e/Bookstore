﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; } 
        public ICollection<Book> Books { get; set; } 
    }
}
