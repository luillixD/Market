﻿namespace Market.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Subcategory> Subcategories { get; set; }
    }
}
