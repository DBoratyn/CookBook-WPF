using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookBook.Models
{
    
    public class Recipe
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Ingredients { get; set; }

        public string Method { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Category}";
        }
    }
}
