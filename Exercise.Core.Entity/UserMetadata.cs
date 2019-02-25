using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Exercise.Core.Entity
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
    }

    public class UserMetadata
    {
        [JsonIgnore]
        public Avatar Avatar { get; set; }
    }
}