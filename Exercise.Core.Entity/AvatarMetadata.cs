using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Exercise.Core.Entity
{
    [MetadataType(typeof(AvatarMetadata))]
    public partial class Avatar
    {
    }

    public class AvatarMetadata
    {
        [JsonIgnore]
        public byte[] Image { get; set; }
    }
}