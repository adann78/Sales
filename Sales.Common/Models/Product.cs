

namespace Sales.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(50)]
        public string Descripcion { get; set; }
        [DataType(DataType.MultilineText)]
        public string Remarks{ get; set; }
        [Display(Name ="Image")]
        public string ImagePath{ get; set; }
        [DisplayFormat(DataFormatString ="{0:C2}", ApplyFormatInEditMode =false)]
        public decimal Precio { get; set; }
        [Display(Name = "Is Avaliable")]
        public bool IsAvialable { get; set; }
        [Display(Name = "Publish On")]
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }
        [NotMapped]
        public byte[] ImageArray { get; set; }
        public string ImageFullPath
       

        {
            get
            {
                if(string.IsNullOrEmpty(this.ImagePath))
                {
                    return "noproduct";
                }
                return $"https://salesbackend20181117125333.azurewebsites.net/{this.ImagePath.Substring(1)}";
                
            }
        }

        public override string ToString()
        {
            return this.Descripcion;
        }
    }
}
