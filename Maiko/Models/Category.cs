using System;
using System.Collections.Generic;
using System.Text;

namespace Maiko.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Budget { get; set; } = 0d;
    }
}
