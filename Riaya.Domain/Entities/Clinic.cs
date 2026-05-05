using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Riaya.Domain.Entities
{
    public class Clinic : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
