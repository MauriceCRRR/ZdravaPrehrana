using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Entitete
{
    public class Sestavina
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        public double Kalorije { get; set; }
        public double Beljakovine { get; set; }
        public double Mascobe { get; set; }
        public double OgljikoviHidrati { get; set; }

        // Update the navigation property
        public virtual ICollection<ReceptSestavina> ReceptSestavine { get; set; } = new List<ReceptSestavina>();
    }
}