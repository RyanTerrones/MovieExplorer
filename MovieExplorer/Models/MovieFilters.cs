using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieExplorer.Models;

public class MovieFilters
{
    // any / Movie / Series
    public string Type { get; set; } = "Any"; 
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }

    // title / year
    public string Sort { get; set; } = "Title";
}
