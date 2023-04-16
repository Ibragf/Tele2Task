using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace test_task.Models
{
    public class Resident
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sex")]
        public string Sex { get; set; }

        [JsonPropertyName("age")]
        public int? Age { get; set; }

        public Resident(string id, string name, string sex)
        {
            Name = name;
            Id= id;
            Sex = sex;
        }

        public Resident()
        {

        }
    }
}
