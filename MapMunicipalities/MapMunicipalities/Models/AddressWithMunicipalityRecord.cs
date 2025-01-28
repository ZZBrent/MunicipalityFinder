﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMunicipalities.Models
{
    internal class AddressWithMunicipalityRecord
    {
        public AddressWithMunicipalityRecord(string address, string municipality)
        {
            Address = address;
            Municipality = municipality;
        }
        public string Address { get; set; }
        public string Municipality { get; set; }
    }
}
