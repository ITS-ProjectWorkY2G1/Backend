using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.AuthModels
{
    public class ClaimsModel : MongoClaim
    {
        public Guid Id { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
