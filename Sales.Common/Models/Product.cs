

namespace Sales.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool IsAvialable { get; set; }
        public DateTime PublishOn { get; set; }
    }
}
