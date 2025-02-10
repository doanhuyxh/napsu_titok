using System.ComponentModel.DataAnnotations;

namespace napsu_titok.Models;

public class WebConfig
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
}