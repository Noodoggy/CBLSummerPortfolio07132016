using System;
using System.ComponentModel.DataAnnotations;


namespace CBLSummerPortfolio07132016.Models


{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateTime MessageSent { get; set; }
    }
}
