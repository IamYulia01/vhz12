using System;
using System.Collections.Generic;

namespace VHZ_App.Models;

public partial class Information
{
    public int IdInformation { get; set; }

    public string SectionName { get; set; } = null!;

    public string Description { get; set; } = null!;
}
